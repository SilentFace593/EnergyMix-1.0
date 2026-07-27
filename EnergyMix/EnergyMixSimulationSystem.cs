using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game;
using Game.Buildings;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace EnergyMix
{
    /// <summary>
    /// Système de simulation : échantillonnage, catégorisation, et
    /// persistance via ISerializable implémenté DIRECTEMENT sur ce système
    /// (mécanisme "au niveau système", confirmé empiriquement par la trace
    /// de pile de PrefabSystem.Serialize / ComponentSystemSerializer).
    /// GameSystemBase suit correctement le cycle de vie de la partie
    /// (recréé à chaque nouvelle partie chargée — confirmé empiriquement).
    /// </summary>
    public partial class EnergyMixSimulationSystem : GameSystemBase, IPreDeserialize, IPostDeserialize, IDefaultSerializable
    {
        private static readonly ILog Log = LogManager.GetLogger("EnergyMix");

        private const float SampleIntervalSeconds = 60f;
        private float _timeSinceLastSample = 0f;

        /// <summary>Données du mod. Accessible en lecture par EnergyMixUISystem.</summary>
        public readonly EnergyMixData Data = new EnergyMixData();

        private TimeSystem _timeSystem;
        private PrefabSystem _prefabSystem;

        private EntityQuery _producerQuery;

        private ComponentLookup<WindPoweredData> _windLookup;
        private ComponentLookup<SolarPoweredData> _solarLookup;
        private ComponentLookup<WaterPoweredData> _waterLookup;
        private ComponentLookup<GroundWaterPoweredData> _groundWaterLookup;
        private ComponentLookup<GarbagePoweredData> _garbageLookup;
        private ComponentLookup<PowerPlantData> _powerPlantLookup;

        // ── Sérialisation (au niveau système) ────────────────────────────────

        /// <summary>Nécessaire côté UI pour calculer les fenêtres temporelles du graphique.</summary>
        public int DaysPerYear => _timeSystem.daysPerYear;

        public void PreDeserialize(Context context) { }

        public void PostDeserialize(Context context) { }

        public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            Data.Serialize(writer);
            Log.Info($"EnergyMix: sauvegarde écrite (Fine={Data.Fine.Count})");
        }

        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            Data.Deserialize(reader);
            Log.Info($"EnergyMix: sauvegarde chargée (Fine={Data.Fine.Count})");
        }

        /// <summary>
        /// Appelé pour initialiser les données par défaut — typiquement pour
        /// une nouvelle partie (aucune donnée EnergyMix préexistante à charger).
        /// </summary>
        public void SetDefaults(Context context)
        {
            Data.Current = default;
            Data.Fine.Clear();
            Data.Medium.Clear();
            Data.Coarse.Clear();
        }

        // ── Cycle de vie ──────────────────────────────────────────────────────

        protected override void OnCreate()
        {
            base.OnCreate();

            _timeSystem = World.GetOrCreateSystemManaged<TimeSystem>();
            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            _producerQuery = GetEntityQuery(
                ComponentType.ReadOnly<ElectricityProducer>(),
                ComponentType.ReadOnly<PrefabRef>(),
                ComponentType.Exclude<Temp>(),
                ComponentType.Exclude<Deleted>()
            );

            _windLookup = GetComponentLookup<WindPoweredData>(isReadOnly: true);
            _solarLookup = GetComponentLookup<SolarPoweredData>(isReadOnly: true);
            _waterLookup = GetComponentLookup<WaterPoweredData>(isReadOnly: true);
            _groundWaterLookup = GetComponentLookup<GroundWaterPoweredData>(isReadOnly: true);
            _garbageLookup = GetComponentLookup<GarbagePoweredData>(isReadOnly: true);
            _powerPlantLookup = GetComponentLookup<PowerPlantData>(isReadOnly: true);

            Log.Info("EnergyMixSimulationSystem.OnCreate — système instancié dans le monde de simulation.");
        }

        protected override void OnUpdate()
        {
            _timeSinceLastSample += World.Time.DeltaTime;
            if (_timeSinceLastSample < SampleIntervalSeconds)
                return;

            _timeSinceLastSample = 0f;

            _windLookup.Update(this);
            _solarLookup.Update(this);
            _waterLookup.Update(this);
            _groundWaterLookup.Update(this);
            _garbageLookup.Update(this);
            _powerPlantLookup.Update(this);

            EnergySnapshot snapshot = CollectSnapshot();
            Data.PushSnapshot(snapshot);

            Log.Info($"EnergyMix: échantillon @ gameDays={snapshot.GameDays:F2}, " +
                     $"Fine={Data.Fine.Count} Medium={Data.Medium.Count} Coarse={Data.Coarse.Count}");
        }

        // ── Collecte ──────────────────────────────────────────────────────────

        private EnergySnapshot CollectSnapshot()
        {
            double gameDays = (double)_timeSystem.year * _timeSystem.daysPerYear
                            + (double)(_timeSystem.normalizedDate * _timeSystem.daysPerYear);

            var snapshot = new EnergySnapshot { GameDays = gameDays };

            NativeArray<Entity> entities = _producerQuery.ToEntityArray(Allocator.Temp);
            try
            {
                foreach (Entity entity in entities)
                {
                    if (!EntityManager.HasComponent<ElectricityProducer>(entity))
                        continue;
                    if (!EntityManager.HasComponent<PrefabRef>(entity))
                        continue;

                    float production = EntityManager
                        .GetComponentData<ElectricityProducer>(entity).m_LastProduction;
                    Entity prefab = EntityManager
                        .GetComponentData<PrefabRef>(entity).m_Prefab;

                    EnergyCategory cat = Categorize(entity, prefab);
                    snapshot.SetValue(cat, snapshot.GetValue(cat) + production);
                }
            }
            finally
            {
                entities.Dispose();
            }

            return snapshot;
        }

        private EnergyCategory Categorize(Entity buildingEntity, Entity prefabEntity)
        {
            if (_windLookup.HasComponent(prefabEntity))
                return EnergyCategory.Wind;

            if (_solarLookup.HasComponent(prefabEntity))
                return EnergyCategory.Solar;

            if (_waterLookup.HasComponent(prefabEntity))
                return EnergyCategory.Hydro;

            if (_groundWaterLookup.HasComponent(prefabEntity))
                return EnergyCategory.Geothermal;

            if (_garbageLookup.HasComponent(prefabEntity))
                return EnergyCategory.Incineration;

            if (_powerPlantLookup.HasComponent(prefabEntity)
                && EntityManager.HasComponent<Game.Buildings.ResourceConsumer>(buildingEntity))
            {
                string name = GetPrefabName(prefabEntity);
                if (name.Contains("Coal")) return EnergyCategory.Coal;
                if (name.Contains("Gas")) return EnergyCategory.Gas;

                Log.Warn($"Prefab '{name}' non reconnu (ResourceConsumer présent). " +
                          "Catégorisé en Coal par défaut.");
                return EnergyCategory.Coal;
            }

            return EnergyCategory.Nuclear;
        }

        private string GetPrefabName(Entity prefabEntity)
        {
            try
            {
                if (EntityManager.HasComponent<PrefabData>(prefabEntity))
                {
                    PrefabBase pb = _prefabSystem.GetPrefab<PrefabBase>(
                        EntityManager.GetComponentData<PrefabData>(prefabEntity));
                    return pb?.name ?? "";
                }
            }
            catch (System.Exception e)
            {
                Log.Warn($"GetPrefabName exception : {e.Message}");
            }
            return "";
        }
    }
}
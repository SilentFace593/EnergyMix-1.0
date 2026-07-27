using System.Collections.Generic;
using Colossal.Logging;
using Colossal.UI.Binding;
using Game.UI;

namespace EnergyMix
{
    /// <summary>
    /// Système UI : expose les données via des bindings JSON pour le frontend.
    /// Ne cache jamais de référence au système de simulation entre deux
    /// appels : on va la rechercher via World.GetOrCreateSystemManaged à
    /// chaque écriture de binding, pour être sûr de toujours lire les
    /// données de la partie actuellement active (UISystemBase ayant un
    /// cycle de vie plus persistant que GameSystemBase across saves).
    /// </summary>
    public partial class EnergyMixUISystem : UISystemBase
    {
        private static readonly ILog Log = LogManager.GetLogger("EnergyMix");

        private RawValueBinding _currentBinding;
        private RawValueBinding _historyFineBinding;
        private RawValueBinding _historyMediumBinding;
        private RawValueBinding _historyCoarseBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            const string group = "energyMixMod";

            _currentBinding = new RawValueBinding(
                group, "currentMix", WriteCurrentMixJson);
            _historyFineBinding = new RawValueBinding(
                group, "historyFine", w => WriteHistoryJson(w, GetSimSystem()?.Data.Fine));
            _historyMediumBinding = new RawValueBinding(
                group, "historyMedium", w => WriteHistoryJson(w, GetSimSystem()?.Data.Medium));
            _historyCoarseBinding = new RawValueBinding(
                group, "historyCoarse", w => WriteHistoryJson(w, GetSimSystem()?.Data.Coarse));

            AddBinding(_currentBinding);
            AddBinding(_historyFineBinding);
            AddBinding(_historyMediumBinding);
            AddBinding(_historyCoarseBinding);

            Log.Info("EnergyMixUISystem.OnCreate — bindings enregistrés.");
        }

        protected override void OnUpdate()
        {
            _currentBinding.Update();
            _historyFineBinding.Update();
            _historyMediumBinding.Update();
            _historyCoarseBinding.Update();
        }

        private EnergyMixSimulationSystem GetSimSystem()
        {
            return World?.GetOrCreateSystemManaged<EnergyMixSimulationSystem>();
        }

        private void WriteCurrentMixJson(IJsonWriter writer)
        {
            var sim = GetSimSystem();
            EnergySnapshot s = sim?.Data.Current ?? default;
            int daysPerYear = sim?.DaysPerYear ?? 4;

            writer.TypeBegin("currentMix");
            writer.PropertyName("gameDays"); writer.Write(s.GameDays);
            writer.PropertyName("wind"); writer.Write(s.Wind);
            writer.PropertyName("coal"); writer.Write(s.Coal);
            writer.PropertyName("gas"); writer.Write(s.Gas);
            writer.PropertyName("incineration"); writer.Write(s.Incineration);
            writer.PropertyName("solar"); writer.Write(s.Solar);
            writer.PropertyName("geothermal"); writer.Write(s.Geothermal);
            writer.PropertyName("nuclear"); writer.Write(s.Nuclear);
            writer.PropertyName("hydro"); writer.Write(s.Hydro);
            writer.PropertyName("daysPerYear"); writer.Write(daysPerYear);
            writer.TypeEnd();
        }

        private void WriteHistoryJson(IJsonWriter writer, List<EnergySnapshot> list)
        {
            if (list == null)
            {
                writer.ArrayBegin(0);
                writer.ArrayEnd();
                return;
            }
            writer.ArrayBegin((uint)list.Count);
            for (int i = 0; i < list.Count; i++)
                WriteSnapshotObject(writer, "snapshot", list[i]);
            writer.ArrayEnd();
        }

        private static void WriteSnapshotObject(IJsonWriter writer, string typeName, EnergySnapshot s)
        {
            writer.TypeBegin(typeName);
            writer.PropertyName("gameDays"); writer.Write(s.GameDays);
            writer.PropertyName("wind"); writer.Write(s.Wind);
            writer.PropertyName("coal"); writer.Write(s.Coal);
            writer.PropertyName("gas"); writer.Write(s.Gas);
            writer.PropertyName("incineration"); writer.Write(s.Incineration);
            writer.PropertyName("solar"); writer.Write(s.Solar);
            writer.PropertyName("geothermal"); writer.Write(s.Geothermal);
            writer.PropertyName("nuclear"); writer.Write(s.Nuclear);
            writer.PropertyName("hydro"); writer.Write(s.Hydro);
            writer.TypeEnd();
        }
    }
}
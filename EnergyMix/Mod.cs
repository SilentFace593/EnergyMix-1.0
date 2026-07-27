using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace EnergyMix
{
    public class Mod : IMod
    {
        public static ILog log = LogManager
            .GetLogger($"{nameof(EnergyMix)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            // Système de simulation : échantillonnage + sérialisation.
            updateSystem.UpdateAt<EnergyMixSimulationSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<EnergyMixSimulationSystem>(SystemUpdatePhase.Serialize);
            updateSystem.UpdateAt<EnergyMixSimulationSystem>(SystemUpdatePhase.Deserialize);

            // Système UI : bindings uniquement.
            updateSystem.UpdateAt<EnergyMixUISystem>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
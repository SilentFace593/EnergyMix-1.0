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

            GameManager.instance.localizationManager.AddSource("en-US", new EnergyMixLocaleEN());
            GameManager.instance.localizationManager.AddSource("fr-FR", new EnergyMixLocaleFR());

            updateSystem.UpdateAt<EnergyMixSimulationSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<EnergyMixSimulationSystem>(SystemUpdatePhase.Serialize);
            updateSystem.UpdateAt<EnergyMixSimulationSystem>(SystemUpdatePhase.Deserialize);

            updateSystem.UpdateAt<EnergyMixUISystem>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}
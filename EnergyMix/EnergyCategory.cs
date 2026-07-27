namespace EnergyMix
{
    /// <summary>
    /// Les 8 catégories de sources d'énergie affichées par le mod.
    /// Charbon et Gaz sont distingués (contrairement aux "Énergies Fossiles"
    /// génériques du jeu), la distinction étant fiable via le nom du prefab.
    ///
    /// L'ordre définit l'ordre d'affichage par défaut (camembert + légende).
    /// Les valeurs numériques sont figées une fois publiées : elles sont
    /// utilisées telles quelles dans la sérialisation de sauvegarde
    /// (voir EnergyMixData.Serialize). Ne jamais réordonner ou insérer
    /// une valeur au milieu après publication — ajouter uniquement en fin
    /// de liste si besoin, sous peine de corrompre la lecture des
    /// sauvegardes existantes.
    /// </summary>
    public enum EnergyCategory : byte
    {
        Wind = 0,
        Coal = 1,
        Gas = 2,
        Incineration = 3,
        Solar = 4,
        Geothermal = 5,
        Nuclear = 6,
        Hydro = 7,
    }

    public static class EnergyCategoryExtensions
    {
        public const int Count = 8;

        /// <summary>
        /// Couleur associée à chaque source, choisie par l'utilisateur.
        /// Format hexadécimal, exploitable directement côté UI (React/CSS).
        /// </summary>
        public static string GetColorHex(this EnergyCategory category)
        {
            switch (category)
            {
                case EnergyCategory.Incineration: return "#6F4E37"; // Marron
                case EnergyCategory.Coal: return "#33303B";         // Gris très foncé
                case EnergyCategory.Gas: return "#574B94";          // Violet-gris
                case EnergyCategory.Hydro: return "#1E6FD9";        // Bleu
                case EnergyCategory.Wind: return "#F2F2F2";         // Blanc
                case EnergyCategory.Solar: return "#F5C518";        // Jaune
                case EnergyCategory.Geothermal: return "#D62828";   // Rouge
                case EnergyCategory.Nuclear: return "#2E8B57";      // Vert
                default: return "#999999";
            }
        }
    }
}

namespace EnergyMix
{
    /// <summary>
    /// Constantes centralisant toutes les clés de traduction du mod.
    /// Utilisées à la fois par EnergyMixLocaleEN/FR (C#) et, sous forme de
    /// chaînes littérales répliquées côté TS (voir localeKeys.ts), par le
    /// frontend React via useLocalization().translate(key, fallback).
    /// </summary>
    public static class LocaleKeys
    {
        // Panel
        public const string Panel_Title = "EnergyMix.PANEL_TITLE";

        // Tabs
        public const string Tab_Mix = "EnergyMix.TAB[Mix]";
        public const string Tab_History = "EnergyMix.TAB[History]";

        // History window selector
        public const string Window_Month = "EnergyMix.WINDOW[Month]";
        public const string Window_Years2 = "EnergyMix.WINDOW[Years2]";
        public const string Window_Years5 = "EnergyMix.WINDOW[Years5]";

        // Empty states
        public const string NoData_Pie = "EnergyMix.NO_DATA_PIE";
        public const string NoData_Line = "EnergyMix.NO_DATA_LINE";

        // Energy categories
        public const string Category_Wind = "EnergyMix.CATEGORY[Wind]";
        public const string Category_Coal = "EnergyMix.CATEGORY[Coal]";
        public const string Category_Gas = "EnergyMix.CATEGORY[Gas]";
        public const string Category_Incineration = "EnergyMix.CATEGORY[Incineration]";
        public const string Category_Solar = "EnergyMix.CATEGORY[Solar]";
        public const string Category_Geothermal = "EnergyMix.CATEGORY[Geothermal]";
        public const string Category_Nuclear = "EnergyMix.CATEGORY[Nuclear]";
        public const string Category_Hydro = "EnergyMix.CATEGORY[Hydro]";
    }
}
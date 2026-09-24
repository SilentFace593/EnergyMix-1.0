using System.Collections.Generic;
using Colossal;

namespace EnergyMix
{
    public class EnergyMixLocaleEN : IDictionarySource
    {
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Panel
                { LocaleKeys.Panel_Title, "EnergyMix" },

                // Tabs
                { LocaleKeys.Tab_Mix, "Current mix" },
                { LocaleKeys.Tab_History, "History" },

                // History window selector
                { LocaleKeys.Window_Month, "1 month" },
                { LocaleKeys.Window_Years2, "2 years" },
                { LocaleKeys.Window_Years5, "5 years" },

                // Empty states
                { LocaleKeys.NoData_Pie, "No electricity production detected at the moment." },
                { LocaleKeys.NoData_Line, "Not enough data yet for this period." },

                // Energy categories
                { LocaleKeys.Category_Wind, "Wind Energy" },
                { LocaleKeys.Category_Coal, "Coal" },
                { LocaleKeys.Category_Gas, "Gas" },
                { LocaleKeys.Category_Incineration, "Incinerators" },
                { LocaleKeys.Category_Solar, "Solar" },
                { LocaleKeys.Category_Geothermal, "Geothermal" },
                { LocaleKeys.Category_Nuclear, "Nuclear" },
                { LocaleKeys.Category_Hydro, "Hydroelectric" },
            };
        }

        public void Unload() { }
    }
}
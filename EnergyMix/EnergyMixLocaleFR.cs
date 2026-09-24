using System.Collections.Generic;
using Colossal;

namespace EnergyMix
{
    public class EnergyMixLocaleFR : IDictionarySource
    {
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Panel
                { LocaleKeys.Panel_Title, "EnergyMix" },

                // Tabs
                { LocaleKeys.Tab_Mix, "Mix actuel" },
                { LocaleKeys.Tab_History, "Historique" },

                // History window selector
                { LocaleKeys.Window_Month, "1 mois" },
                { LocaleKeys.Window_Years2, "2 ans" },
                { LocaleKeys.Window_Years5, "5 ans" },

                // Empty states
                { LocaleKeys.NoData_Pie, "Aucune production électrique détectée pour le moment." },
                { LocaleKeys.NoData_Line, "Pas encore assez de données pour cette période." },

                // Energy categories
                { LocaleKeys.Category_Wind, "Éolien" },
                { LocaleKeys.Category_Coal, "Charbon" },
                { LocaleKeys.Category_Gas, "Gaz" },
                { LocaleKeys.Category_Incineration, "Incinérateurs" },
                { LocaleKeys.Category_Solar, "Solaire" },
                { LocaleKeys.Category_Geothermal, "Géothermie" },
                { LocaleKeys.Category_Nuclear, "Nucléaire" },
                { LocaleKeys.Category_Hydro, "Hydroélectrique" },
            };
        }

        public void Unload() { }
    }
}
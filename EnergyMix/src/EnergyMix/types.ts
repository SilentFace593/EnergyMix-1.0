// Doit rester synchronisé avec EnergyCategory.cs (ordre, clés, couleurs).

export interface EnergySnapshot {
  gameDays: number;
  wind: number;
  coal: number;
  gas: number;
  incineration: number;
  solar: number;
  geothermal: number;
  nuclear: number;
  hydro: number;
}

// currentMix ajoute daysPerYear en plus des champs de EnergySnapshot.
export interface CurrentMix extends EnergySnapshot {
  daysPerYear: number;
}

export type EnergyCategoryKey =
  | "wind"
  | "coal"
  | "gas"
  | "incineration"
  | "solar"
  | "geothermal"
  | "nuclear"
  | "hydro";

export interface CategoryMeta {
  key: EnergyCategoryKey;
  labelKey: string;   // clé de traduction, ex. "EnergyMix.CATEGORY[Wind]"
  fallback: string;   // libellé anglais utilisé comme fallback si la clé manque
  color: string;
}

export const CATEGORIES: CategoryMeta[] = [
  { key: "wind",         labelKey: "EnergyMix.CATEGORY[Wind]",         fallback: "Wind Energy",   color: "#F2F2F2" },
  { key: "coal",         labelKey: "EnergyMix.CATEGORY[Coal]",         fallback: "Coal",          color: "#33303B" },
  { key: "gas",          labelKey: "EnergyMix.CATEGORY[Gas]",          fallback: "Gas",           color: "#574B94" },
  { key: "incineration", labelKey: "EnergyMix.CATEGORY[Incineration]", fallback: "Incinerators",  color: "#6F4E37" },
  { key: "solar",        labelKey: "EnergyMix.CATEGORY[Solar]",        fallback: "Solar",         color: "#F5C518" },
  { key: "geothermal",   labelKey: "EnergyMix.CATEGORY[Geothermal]",   fallback: "Geothermal",    color: "#D62828" },
  { key: "nuclear",      labelKey: "EnergyMix.CATEGORY[Nuclear]",      fallback: "Nuclear",       color: "#2E8B57" },
  { key: "hydro",        labelKey: "EnergyMix.CATEGORY[Hydro]",        fallback: "Hydroelectric", color: "#1E6FD9" },
];

export const EMPTY_SNAPSHOT: EnergySnapshot = {
  gameDays: 0, wind: 0, coal: 0, gas: 0, incineration: 0,
  solar: 0, geothermal: 0, nuclear: 0, hydro: 0,
};

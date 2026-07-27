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
  label: string;
  color: string;
}

// Couleurs choisies par l'utilisateur — garder synchronisé avec
// EnergyCategoryExtensions.GetColorHex côté C#.
export const CATEGORIES: CategoryMeta[] = [
  { key: "wind",         label: " Wind Energy   ",          color: "#F2F2F2" },
  { key: "coal",         label: " Coal    ",         color: "#33303B" },
  { key: "gas",          label: " Gas    ",             color: "#574B94" },
  { key: "incineration", label: " Incinérators    ",   color: "#6F4E37" },
  { key: "solar",        label: " Solar    ",         color: "#F5C518" },
  { key: "geothermal",   label: " Geothermal    ",      color: "#D62828" },
  { key: "nuclear",      label: " Nuclear    ",       color: "#2E8B57" },
  { key: "hydro",        label: " Hydroelectric    ", color: "#1E6FD9" },
];

export const EMPTY_SNAPSHOT: EnergySnapshot = {
  gameDays: 0, wind: 0, coal: 0, gas: 0, incineration: 0,
  solar: 0, geothermal: 0, nuclear: 0, hydro: 0,
};

import { bindValue } from "cs2/api";
import { CurrentMix, EnergySnapshot } from "./types";

const GROUP = "energyMixMod";

export const currentMix$ = bindValue<CurrentMix>(GROUP, "currentMix");
export const historyFine$ = bindValue<EnergySnapshot[]>(GROUP, "historyFine");
export const historyMedium$ = bindValue<EnergySnapshot[]>(GROUP, "historyMedium");
export const historyCoarse$ = bindValue<EnergySnapshot[]>(GROUP, "historyCoarse");

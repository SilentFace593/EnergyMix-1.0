import { ModRegistrar } from "cs2/modding";
import { EnergyMixPanel } from "./EnergyMix/EnergyMixPanel";

const register: ModRegistrar = (moduleRegistry) => {
    moduleRegistry.append("GameTopLeft", EnergyMixPanel);
};

export default register;
import { Component, useState } from "react";
import { useValue } from "cs2/api";
import { useTranslate } from "./translate";
import { Button, Panel } from "cs2/ui";
import { currentMix$, historyFine$, historyMedium$, historyCoarse$ } from "./bindings";
import { PieChart } from "./PieChart";
import { LineChart } from "./LineChart";
import { EMPTY_SNAPSHOT } from "./types";
import buttonStyle from "./EnergyMixButton.module.scss";

type TabKey = "mix" | "history";

// Error Boundary — une erreur de rendu ici ne doit jamais faire planter le
// reste du Universal Mod Menu ou du jeu.
class SafeBoundary extends Component<{ children: any }, { crashed: boolean }> {
  constructor(props: any) {
    super(props);
    this.state = { crashed: false };
  }
  static getDerivedStateFromError() {
    return { crashed: true };
  }
  render() {
    if (this.state.crashed) return null;
    return this.props.children;
  }
}

function EnergyMixContent() {
  const translate = useTranslate();
  const [tab, setTab] = useState<TabKey>("mix");

  const current = useValue(currentMix$);
  const fine = useValue(historyFine$);
  const medium = useValue(historyMedium$);
  const coarse = useValue(historyCoarse$);

  const snapshot = current ?? EMPTY_SNAPSHOT;
  const daysPerYear = current?.daysPerYear ?? 4;

  return (
   <div
    style={{
        padding: "10rem",
        width: "550rem",
        height: "410rem",
        boxSizing: "border-box",
        overflow: "hidden",
        display: "flex",
        flexDirection: "column"
    }}
>
     <div style={{ display: "flex", gap: "4rem", marginBottom: "10rem" }}>
  <TabButton
    label={translate("EnergyMix.TAB[Mix]", "Current mix") ?? "Current mix"}
    active={tab === "mix"}
    onClick={() => setTab("mix")}
  />
  <TabButton
    label={translate("EnergyMix.TAB[History]", "History") ?? "History"}
    active={tab === "history"}
    onClick={() => setTab("history")}
  />
</div>

            <div
        style={{
          flex: 1,
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          overflow: "hidden"
        }}
      >
        {tab === "mix" ? (
          <PieChart snapshot={snapshot} />
        ) : (
          <LineChart
            fine={fine ?? []}
            medium={medium ?? []}
            coarse={coarse ?? []}
            daysPerYear={daysPerYear}
          />
        )}
      </div>
    </div>
  );
}

function TabButton({ label, active, onClick }: { label: string; active: boolean; onClick: () => void }) {
  return (
    <button
      onClick={onClick}
      style={{
        flex: 1,
        padding: "6rem 8rem",
        fontSize: "12rem",
        borderRadius: "4rem",
        border: "none",
        cursor: "pointer",
        background: active ? "rgba(255,255,255,0.15)" : "rgba(255,255,255,0.05)",
        color: active ? "white" : "rgba(255,255,255,0.6)",
      }}
    >
      {label}
    </button>
  );
}

// Composant ajouté à la barre d'outils du jeu (GameTopLeft).
// Il ne doit occuper qu'un petit espace (un bouton), le vrai panneau
// s'affichant en superposition via Portal, indépendamment de la zone
// exiguë réservée aux entrées du menu.
function EnergyMixEntry() {
  const translate = useTranslate();
  const [open, setOpen] = useState(false);
  return (
    <>
      <Button variant="flat" onSelect={() => setOpen(o => !o)}>
        <div className={buttonStyle.icon} />
      </Button>
      {open && (
        <Panel
          draggable
          initialPosition={{ x: 0.5, y: 0.3 }}
          header={
            <div style={{ display: "flex", alignItems: "center", gap: "6rem" }}>
              <span>{translate("EnergyMix.TITLE", "EnergyMix")}</span>
            </div>
          }
          onClose={() => setOpen(false)}
        >
          <EnergyMixContent />
        </Panel>
      )}
    </>
  );
}

export const EnergyMixPanel = () => (
  <SafeBoundary>
    <EnergyMixEntry />
  </SafeBoundary>
);

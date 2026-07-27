import { useMemo, useState } from "react";
import { CATEGORIES, EnergySnapshot } from "./types";

type WindowOption = "month" | "years2" | "years5";

const WINDOW_LABELS: Record<WindowOption, string> = {
  month: "1 mois",
  years2: "2 ans",
  years5: "5 ans",
};

interface LineChartProps {
  fine: EnergySnapshot[];
  medium: EnergySnapshot[];
  coarse: EnergySnapshot[];
  daysPerYear: number;
}

export function LineChart({ fine, medium, coarse, daysPerYear }: LineChartProps) {
  const [selectedWindow, setSelectedWindow] = useState<WindowOption>("month");

  // Les 3 étages RRD sont chronologiques : Coarse (le plus ancien) puis
  // Medium puis Fine (le plus récent). On les concatène pour former une
  // seule série temporelle continue avant de filtrer par fenêtre.
  const allPoints = useMemo(
    () => [...coarse, ...medium, ...fine],
    [fine, medium, coarse]
  );

  const windowDays = useMemo(() => {
    switch (selectedWindow) {
      case "month": return 1;
      case "years2": return 2 * daysPerYear;
      case "years5": return 5 * daysPerYear;
    }
  }, [selectedWindow, daysPerYear]);

  const points = useMemo(() => {
    if (allPoints.length === 0) return [];
    const latest = allPoints[allPoints.length - 1].gameDays;
    const cutoff = latest - windowDays;
    return allPoints.filter((p) => p.gameDays >= cutoff);
  }, [allPoints, windowDays]);

  const width = 500;
  const height = 290;
  const paddingLeft = 8;
  const paddingRight = 8;
  const paddingTop = 8;
  const paddingBottom = 8;
  const plotWidth = width - paddingLeft - paddingRight;
  const plotHeight = height - paddingTop - paddingBottom;

  const minX = points.length > 0 ? points[0].gameDays : 0;
  const maxX = points.length > 0 ? points[points.length - 1].gameDays : 1;
  const spanX = Math.max(maxX - minX, 1e-6);

  const maxY = useMemo(() => {
    let m = 0;
    for (const p of points) {
      for (const c of CATEGORIES) {
        const v = (p as any)[c.key] as number;
        if (v > m) m = v;
      }
    }
    return m > 0 ? m : 1;
  }, [points]);

  function toX(gameDays: number) {
    return paddingLeft + ((gameDays - minX) / spanX) * plotWidth;
  }

  function toY(value: number) {
    return paddingTop + plotHeight - (value / maxY) * plotHeight;
  }

  const polylines = CATEGORIES.map((c) => {
    const d = points
      .map((p, i) => {
        const x = toX(p.gameDays);
        const y = toY((p as any)[c.key] as number);
        return `${i === 0 ? "M" : "L"} ${x.toFixed(2)} ${y.toFixed(2)}`;
      })
      .join(" ");
    return (
      <path key={c.key} d={d} fill="none" stroke={c.color} strokeWidth={1.5} />
    );
  });

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "8rem" }}>
      <div style={{ display: "flex", gap: "6rem" }}>
        {(Object.keys(WINDOW_LABELS) as WindowOption[]).map((opt) => (
          <button
            key={opt}
            onClick={() => setSelectedWindow(opt)}
            style={{
              padding: "5rem 10rem",
              fontSize: "12rem",
              borderRadius: "4rem",
              border: "none",
              cursor: "pointer",
              background: selectedWindow === opt ? "rgba(255,255,255,0.2)" : "rgba(200,200,200,0.15)",
              color: selectedWindow === opt ? "white" : "rgba(255,255,255,0.7)",
            }}
          >
            {WINDOW_LABELS[opt]}
          </button>
        ))}
      </div>

      {points.length === 0 ? (
        <div style={{
          width, height, display: "flex", alignItems: "center", justifyContent: "center",
          color: "rgba(255,255,255,0.5)", fontSize: "12rem", textAlign: "center", padding: "0 16rem",
        }}>
          Pas encore assez de données pour cette période.
        </div>
      ) : (
        <svg viewBox={`0 0 ${width} ${height}`} width={width} height={height}>
          <rect x={0} y={0} width={width} height={height} fill="rgba(255,255,255,0.03)" />
          {polylines}
        </svg>
      )}

      <div style={{ display: "flex", flexWrap: "wrap", gap: "8rem" }}>
        {CATEGORIES.map((c) => (
          <div key={c.key} style={{ display: "flex", alignItems: "center", gap: "4rem", fontSize: "11rem" }}>
            <span style={{
              width: "8rem", height: "8rem", borderRadius: "2rem",
              background: c.color, flexShrink: 0,
              border: "1rem solid rgba(255,255,255,0.3)",
            }} />
            <span style={{ color: "rgba(255,255,255,0.75)" }}>{c.label}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

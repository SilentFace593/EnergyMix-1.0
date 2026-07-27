import { CATEGORIES, EnergySnapshot } from "./types";

interface Slice {
  key: string;
  label: string;
  color: string;
  value: number;
  percent: number;
}

function polarToCartesian(cx: number, cy: number, r: number, angleDeg: number) {
  const angleRad = ((angleDeg - 90) * Math.PI) / 180;
  return {
    x: cx + r * Math.cos(angleRad),
    y: cy + r * Math.sin(angleRad),
  };
}

function describeSlice(cx: number, cy: number, r: number, startAngle: number, endAngle: number) {
  const start = polarToCartesian(cx, cy, r, endAngle);
  const end = polarToCartesian(cx, cy, r, startAngle);
  const largeArcFlag = endAngle - startAngle <= 180 ? "0" : "1";
  return [
    "M", cx, cy,
    "L", start.x, start.y,
    "A", r, r, 0, largeArcFlag, 0, end.x, end.y,
    "Z",
  ].join(" ");
}

export function PieChart({ snapshot }: { snapshot: EnergySnapshot }) {
  const slices: Slice[] = CATEGORIES
    .map((c) => ({
      key: c.key,
      label: c.label,
      color: c.color,
      value: (snapshot as any)[c.key] as number,
    }))
    .filter((s) => s.value > 0)
    .map((s, _i, arr) => {
      const total = arr.reduce((sum, x) => sum + x.value, 0);
      return { ...s, percent: total > 0 ? s.value / total : 0 };
    });

  const total = slices.reduce((sum, s) => sum + s.value, 0);

  const size = 220;
  const cx = size / 2;
  const cy = size / 2;
  const r = size / 2 - 8;

  let cumulativeAngle = 0;
  const paths = slices.map((s) => {
    const startAngle = cumulativeAngle;
    const sweep = s.percent * 360;
    cumulativeAngle += sweep;
    const endAngle = cumulativeAngle;

    // Cas dégénéré : une seule source à 100% -> cercle plein, pas d'arc.
    if (sweep >= 359.999) {
      return (
        <circle key={s.key} cx={cx} cy={cy} r={r} fill={s.color} stroke="rgba(0,0,0,0.3)" strokeWidth={1} />
      );
    }

    return (
      <path
        key={s.key}
        d={describeSlice(cx, cy, r, startAngle, endAngle)}
        fill={s.color}
        stroke="rgba(0,0,0,0.3)"
        strokeWidth={1}
      />
    );
  });

  return (
    <div style={{ display: "flex", flexDirection: "column", alignItems: "center", gap: "8rem" }}>
      {total <= 0 ? (
        <div style={{
          width: size, height: size, display: "flex", alignItems: "center",
          justifyContent: "center", color: "rgba(255,255,255,0.5)", fontSize: "12rem",
          textAlign: "center", padding: "0 16rem",
        }}>
          Aucune production électrique détectée pour le moment.
        </div>
      ) : (
        <svg viewBox={`0 0 ${size} ${size}`} width={size} height={size}>
          {paths}
        </svg>
      )}

      <div style={{ width: "100%", display: "flex", flexDirection: "column", gap: "4rem" }}>
        {slices
          .slice()
          .sort((a, b) => b.value - a.value)
          .map((s) => (
            <div key={s.key} style={{ display: "flex", flexDirection: "row", alignItems: "center", gap: "6rem", fontSize: "12rem" }}>
              <span style={{
                width: "10rem", height: "10rem", borderRadius: "2rem",
                background: s.color, flexShrink: 0,
                border: "1rem solid rgba(255,255,255,0.3)",
              }} />
              <span style={{ color: "rgba(255,255,255,0.85)", flex: 1 }}>{s.label}</span>
              <span style={{ color: "rgba(255,255,255,0.6)" }}>
                {(s.percent * 100).toFixed(1)}%
              </span>
            </div>
          ))}
      </div>
    </div>
  );
}

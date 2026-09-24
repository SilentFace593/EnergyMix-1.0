import { useLocalization } from "cs2/l10n";

/**
 * Wrapper autour de useLocalization().translate qui garantit un retour
 * string non-null, pour éviter TS2322 sur les props typées `string`
 * (translate() du SDK CS2 est typé `string | null`).
 */
export function useTranslate() {
  const { translate } = useLocalization();
  return (key: string, fallback: string): string => translate(key, fallback) ?? fallback;
}
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Récord local en PlayerPrefs (único punto de I/O de M3). Se escribe SÓLO al entrar en
    /// GameOver y sólo si el score supera el récord. ShouldReplace es puro y testeable.
    /// </summary>
    public static class HighScoreStore
    {
        public const string Key = "galaga1981.v2.record";

        /// <summary>Récord vigente; sanitiza negativos (PlayerPrefs es editable a mano).</summary>
        public static int Load()
        {
            return Mathf.Max(0, PlayerPrefs.GetInt(Key, 0));
        }

        /// <summary>Comparación estricta: sólo un score mayor (y no negativo) reemplaza.</summary>
        public static bool ShouldReplace(int score, int record)
        {
            return score > record && score >= 0;
        }

        /// <summary>Guarda si supera el récord. Save() explícito: WebGL usa IndexedDB.</summary>
        public static bool SaveIfHigher(int score)
        {
            if (!ShouldReplace(score, Load())) return false;
            PlayerPrefs.SetInt(Key, score);
            PlayerPrefs.Save();
            return true;
        }
    }
}

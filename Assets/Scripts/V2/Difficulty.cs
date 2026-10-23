using UnityEngine;

namespace V2
{
    /// <summary>Curva de amenaza para un nivel (valores ya clampeados).</summary>
    public readonly struct DifficultySettings
    {
        public readonly float DiveCooldownMin;
        public readonly float DiveCooldownMax;
        public readonly float DiveSpeedMul;
        public readonly float SpawnInterval;
        public readonly int MaxDivers;
        public readonly float TractorCooldownMin;
        public readonly float TractorCooldownMax;

        public DifficultySettings(
            float diveCooldownMin,
            float diveCooldownMax,
            float diveSpeedMul,
            float spawnInterval,
            int maxDivers,
            float tractorCooldownMin,
            float tractorCooldownMax)
        {
            DiveCooldownMin = diveCooldownMin;
            DiveCooldownMax = diveCooldownMax;
            DiveSpeedMul = diveSpeedMul;
            SpawnInterval = spawnInterval;
            MaxDivers = maxDivers;
            TractorCooldownMin = tractorCooldownMin;
            TractorCooldownMax = tractorCooldownMax;
        }
    }

    /// <summary>
    /// Escalado M3 puro (sin estado ni I/O): sólo eje de amenaza, con clamps que saturan
    /// a nivel 20. HP y densidad NO escalan (eso es del roguelite).
    /// </summary>
    public static class Difficulty
    {
        public static DifficultySettings ForLevel(int level)
        {
            int t = Mathf.Max(0, level - 1);
            return new DifficultySettings(
                diveCooldownMin: Mathf.Max(2.5f, 5f * Mathf.Pow(0.92f, t)),
                diveCooldownMax: Mathf.Max(4.0f, 8f * Mathf.Pow(0.92f, t)),
                diveSpeedMul: Mathf.Min(1.5f, 1f + 0.05f * t),
                spawnInterval: Mathf.Max(0.20f, 0.35f * Mathf.Pow(0.97f, t)),
                maxDivers: Mathf.Min(4, 2 + t / 4),
                tractorCooldownMin: Mathf.Max(6f, 10f * Mathf.Pow(0.95f, t)),
                tractorCooldownMax: Mathf.Max(8f, 14f * Mathf.Pow(0.95f, t)));
        }
    }
}

using UnityEngine;

namespace V2
{
    /// <summary>
    /// Detección de impacto por distancia (sin física): el player y los enemigos no tienen
    /// Rigidbody2D, así que dos colliders estáticos no emitirían OnTriggerEnter2D.
    /// </summary>
    public static class HitMath
    {
        /// <summary>Radio centro-a-centro del impacto de una picada (tunable, constante de código).</summary>
        public const float PlayerHitRadius = 0.5f;

        /// <summary>dx² + dy² &lt;= r² (borde inclusivo, sin sqrt).</summary>
        public static bool WithinSquared(Vector2 a, Vector2 b, float radius)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return dx * dx + dy * dy <= radius * radius;
        }
    }
}

namespace V2
{
    /// <summary>
    /// Estado de la run M3 (puro, sin MonoBehaviour): vidas. Un único dueño, GameFlow,
    /// es el único que la muta. Nunca baja de 0.
    /// </summary>
    public class RunState
    {
        public const int MaxLives = 3;

        public int Lives { get; private set; } = MaxLives;

        public bool IsDead => Lives <= 0;

        /// <summary>Resta 1 vida. Devuelve false si ya estaba en 0 (no-op).</summary>
        public bool TryLoseLife()
        {
            if (Lives <= 0) return false;
            Lives--;
            return true;
        }

        /// <summary>Retry: vuelve a 3 vidas.</summary>
        public void Reset()
        {
            Lives = MaxLives;
        }
    }
}



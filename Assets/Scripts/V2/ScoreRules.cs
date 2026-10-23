namespace V2
{
    /// <summary>
    /// Reglas de puntaje puras (M3). El bonus por cercanía se aplica en la ruta de kill
    /// (Enemy.Die) y nunca dentro de ScoreManager.Add: el +1000 del challenging no se multiplica.
    /// </summary>
    public static class ScoreRules
    {
        /// <summary>Distancia máxima para pagar el bonus (tunable).</summary>
        public const float NearRadius = 2.5f;

        /// <summary>
        /// base * 1.5 con aritmética entera exacta si distance &lt;= nearRadius; si no, base.
        /// Bases múltiplos de 50: 150→225, 100→150, 50→75.
        /// </summary>
        public static int ApplyProximity(int basePoints, float distance, float nearRadius = NearRadius)
        {
            return distance <= nearRadius ? (basePoints * 3) / 2 : basePoints;
        }
    }
}

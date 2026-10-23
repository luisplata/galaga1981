using System.Collections;
using TMPro;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Flujo de estados Home -> Playing -> GameOver con UN solo panel visible por estado.
    /// M1: StartGame lanza olas infinitas (WaveManager); matar enemigos ya NO termina la
    /// partida (el kill notifica a WaveManager para el conteo de la grilla). GameOver queda
    /// como estado sin disparo automático — M3 lo conecta con vidas.
    /// </summary>
    public class GameFlow : MonoBehaviour
    {
        public enum State
        {
            Home,
            Playing,
            GameOver
        }

        public State CurrentState { get; private set; } = State.Home;

        [SerializeField] private GameObject homePanel;
        [SerializeField] private GameObject playingPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text gameOverScoreText;
        [SerializeField] private string gameOverScorePrefix = "PUNTOS: ";
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private string levelPrefix = "NIVEL ";
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private PlayerController player;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private InputDrag inputDrag;
        [SerializeField] private Autofire autofire;
        [SerializeField] private float respawnDelay = 1.2f; // tunable: M3 lo reemplaza por vidas

        private Enemy currentCarrier;
        private Coroutine respawnRoutine;

        /// <summary>Transform del jugador para el tractor (Enemy.PlayerTransform).</summary>
        public Transform PlayerTransform => player != null ? player.transform : null;

        private void Start()
        {
            if (waveManager != null) waveManager.LevelChanged += HandleLevelChanged;
            SetState(State.Home);
        }

        private void OnDestroy()
        {
            if (waveManager != null) waveManager.LevelChanged -= HandleLevelChanged;
        }

        private void HandleLevelChanged(int level)
        {
            UpdateLevelText(level);
        }

        public void StartGame()
        {
            scoreManager?.ResetScore();

            // Run limpia: sin respawn pendiente, sin carrier y sin dual.
            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }
            if (currentCarrier != null)
            {
                // Restart limpio: el carrier viejo no puede quedar estacionado y matable.
                currentCarrier.Discard();
                currentCarrier = null;
            }
            autofire?.SetDual(false);
            player?.ResetForRun();

            waveManager?.StartWaves();
            UpdateLevelText();
            SetState(State.Playing);
        }

        public void OnEnemyKilled(Enemy killedEnemy)
        {
            // M1/M2: el kill no termina la partida — WaveManager rutea al conteo de la
            // grilla o al challenging (nunca ambos).
            waveManager?.OnEnemyKilled(killedEnemy);
        }

        /// <summary>Un enemigo de challenging cruzó el viewport (escape sin castigo).</summary>
        public void OnEnemyEscaped(Enemy escapedEnemy)
        {
            waveManager?.OnEnemyEscaped(escapedEnemy);
        }

        /// <summary>
        /// Un Boss capturó al jugador. NUNCA dispara GameOver: apaga fuego, pierde dual,
        /// esconde la nave y programa el respawn simple (1.2 s).
        /// </summary>
        public void OnPlayerCaptured(Enemy carrier)
        {
            if (CurrentState != State.Playing) return;

            // A lo sumo una nave retenida: si ya había otro carrier, se descarta.
            if (currentCarrier != null && currentCarrier != carrier)
            {
                // El anterior no queda inerte ni superpuesto: Dead + inactivo (el pool lo
                // recicla en la próxima ola). No puntúa ni bloquea el cierre de la ola.
                currentCarrier.Discard();
            }
            currentCarrier = carrier;

            waveManager?.OnPlayerCaptured(carrier);
            autofire?.SetDual(false);
            if (autofire != null) autofire.SetFireEnabled(false);
            player?.Capture();

            if (respawnRoutine != null) StopCoroutine(respawnRoutine);
            respawnRoutine = StartCoroutine(RespawnRoutine());
        }

        /// <summary>
        /// El carrier murió: rescate → Dual Fighter. Sólo si era el carrier vigente.
        /// </summary>
        public void OnCarrierDestroyed(Enemy carrier)
        {
            if (currentCarrier != carrier) return;
            currentCarrier = null;
            autofire?.SetDual(true);
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            respawnRoutine = null;
            player?.Respawn();
            if (autofire != null) autofire.SetFireEnabled(CurrentState == State.Playing);
        }

        private void SetState(State newState)
        {
            CurrentState = newState;

            if (homePanel != null) homePanel.SetActive(newState == State.Home);
            if (playingPanel != null) playingPanel.SetActive(newState == State.Playing);
            if (gameOverPanel != null) gameOverPanel.SetActive(newState == State.GameOver);

            if (newState == State.GameOver && gameOverScoreText != null && scoreManager != null)
            {
                gameOverScoreText.text = gameOverScorePrefix + scoreManager.Score;
            }

            bool playing = newState == State.Playing;
            if (inputDrag != null) inputDrag.enabled = playing;
            if (autofire != null) autofire.SetFireEnabled(playing);
        }

        private void UpdateLevelText()
        {
            if (waveManager != null) UpdateLevelText(waveManager.CurrentLevel);
        }

        private void UpdateLevelText(int level)
        {
            if (levelText != null) levelText.text = levelPrefix + level;
        }
}
}
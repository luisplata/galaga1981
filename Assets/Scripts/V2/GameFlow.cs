using System.Collections;
using TMPro;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Flujo de estados Home -> Playing -> GameOver con UN solo panel visible por estado.
    /// M3: 3 vidas, daño por picada/captura, i-frames, GameOver real, limpieza total y retry
    /// limpio. GameFlow es el único dueño de las vidas (RunState) y del estado de la run.
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
        [SerializeField] private TMP_Text recordText;
        [SerializeField] private string recordPrefix = "RÉCORD: ";
        [SerializeField] private string newRecordPrefix = "¡NUEVO RÉCORD!: ";
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private string levelPrefix = "NIVEL ";
        [SerializeField] private TMP_Text livesText;
        [SerializeField] private string livesPrefix = "VIDAS ";
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private PlayerController player;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private InputDrag inputDrag;
        [SerializeField] private Autofire autofire;
        [SerializeField] private FxPool fxPool;
        [SerializeField] private CameraShake cameraShake;
        [SerializeField] private AudioSource deathAudioSource;
        [SerializeField] private AudioClip playerDeathClip;
        [SerializeField] private float respawnDelay = 1.2f;      // captura (M2)
        [SerializeField] private float bodyHitDelay = 1.0f;      // impacto de picada
        [SerializeField] private float invulnerabilitySeconds = 1.5f;

        private readonly RunState runState = new();
        private Enemy currentCarrier;
        private Coroutine respawnRoutine;
        private float invulnerableUntil;
        private int highScore;

        /// <summary>Transform del jugador para el tractor (Enemy.PlayerTransform).</summary>
        public Transform PlayerTransform => player != null ? player.transform : null;

        /// <summary>Vidas restantes de la run (HUD y verificación).</summary>
        public int Lives => runState.Lives;

        /// <summary>Predicado único de daño: Playing y fuera de la ventana de i-frames.</summary>
        public bool CanTakeDamage => CurrentState == State.Playing && Time.time >= invulnerableUntil;

        private void Start()
        {
            if (deathAudioSource == null) deathAudioSource = GetComponent<AudioSource>();
            highScore = HighScoreStore.Load();
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

        /// <summary>
        /// Start / REVANCHA: deja la run limpia (8 invariantes del retry) y arranca las olas.
        /// </summary>
        public void StartGame()
        {
            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }

            runState.Reset();
            invulnerableUntil = 0f;

            if (currentCarrier != null)
            {
                // Restart limpio: el carrier viejo no puede quedar estacionado y matable.
                currentCarrier.Discard();
                currentCarrier = null;
            }

            // Limpieza total antes de reconstruir: formación (pool completo), challenging,
            // schedulers y balas en vuelo. StartWaves repite StopWaves (idempotente).
            waveManager?.StopWaves();
            autofire?.ClearBullets();
            autofire?.SetDual(false);

            player?.ResetForRun();
            scoreManager?.ResetScore();

            waveManager?.StartWaves();
            UpdateLivesText();
            UpdateLevelText();
            SetState(State.Playing);
        }

        public void OnEnemyKilled(Enemy killedEnemy)
        {
            // Juice: 1 flash por kill (el audio del kill lo pone el propio Enemy: sin duplicar).
            if (killedEnemy != null) fxPool?.PlayKill(killedEnemy.Type, killedEnemy.transform.position);

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
        /// Impacto de picada (Enemy.UpdateDiving). Devuelve true si el golpe se aceptó.
        /// Fija la invulnerabilidad ANTES de mutar nada: un segundo diver en el mismo frame
        /// ve CanTakeDamage == false y no consume vida. Al llegar a 0 dispara GameOver.
        /// </summary>
        public bool OnPlayerHit(Enemy source)
        {
            if (!CanTakeDamage) return false;

            invulnerableUntil = Time.time + bodyHitDelay + invulnerabilitySeconds;

            autofire?.SetDual(false); // toda muerte pierde el dual (las balas en vuelo siguen)
            runState.TryLoseLife();
            UpdateLivesText();
            PlayPlayerDeathFx();
            cameraShake?.ShakeHit();

            if (runState.IsDead)
            {
                TriggerGameOver();
                return true;
            }

            StartHitRoutine(bodyHitDelay);
            return true;
        }

        private void StartHitRoutine(float delay)
        {
            if (respawnRoutine != null) StopCoroutine(respawnRoutine);
            respawnRoutine = StartCoroutine(HitRoutine(delay));
        }

        /// <summary>Death delay: nave oculta, input y autofire off; al volver, blink de i-frames.</summary>
        private IEnumerator HitRoutine(float delay)
        {
            if (inputDrag != null) inputDrag.enabled = false;
            if (autofire != null) autofire.SetFireEnabled(false);
            player?.Capture();

            yield return new WaitForSeconds(delay);

            respawnRoutine = null;
            if (CurrentState != State.Playing) yield break;

            player?.Respawn();
            player?.BeginInvulnerable(invulnerabilitySeconds);
            if (inputDrag != null) inputDrag.enabled = true;
            if (autofire != null) autofire.SetFireEnabled(true);
        }

        /// <summary>
        /// Un Boss capturó al jugador: -1 vida. Con vidas restantes conserva el flujo M2
        /// (nave retenida + respawn 1.2 s); con 1 vida descarta carrier/nave y entra GameOver.
        /// Una captura durante i-frames no se consume (Enemy ya no estaciona el carrier).
        /// </summary>
        public void OnPlayerCaptured(Enemy carrier)
        {
            if (!CanTakeDamage) return;

            invulnerableUntil = Time.time + respawnDelay + invulnerabilitySeconds;
            autofire?.SetDual(false);

            runState.TryLoseLife();
            UpdateLivesText();
            PlayPlayerDeathFx();
            cameraShake?.ShakeHit();

            if (runState.IsDead)
            {
                // 1 vida -> 0: no hay respawn ni nave retenida; carrier descartado y GameOver.
                if (carrier != null) carrier.Discard();
                if (currentCarrier != null && currentCarrier != carrier) currentCarrier.Discard();
                currentCarrier = null;
                TriggerGameOver();
                return;
            }

            // A lo sumo una nave retenida: si ya había otro carrier, se descarta.
            if (currentCarrier != null && currentCarrier != carrier)
            {
                currentCarrier.Discard();
            }
            currentCarrier = carrier;

            waveManager?.OnPlayerCaptured(carrier);
            if (autofire != null) autofire.SetFireEnabled(false);
            if (inputDrag != null) inputDrag.enabled = false;
            player?.Capture();

            if (respawnRoutine != null) StopCoroutine(respawnRoutine);
            respawnRoutine = StartCoroutine(RespawnRoutine());
        }

        /// <summary>
        /// El carrier murió: rescate → Dual Fighter. Sólo si era el carrier vigente y la run
        /// sigue en Playing (no se regala dual desde GameOver).
        /// </summary>
        public void OnCarrierDestroyed(Enemy carrier)
        {
            if (CurrentState != State.Playing) return;
            if (currentCarrier != carrier) return;
            currentCarrier = null;
            autofire?.SetDual(true);
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            respawnRoutine = null;
            if (CurrentState != State.Playing) yield break;

            player?.Respawn();
            player?.BeginInvulnerable(invulnerabilitySeconds);
            if (inputDrag != null) inputDrag.enabled = true;
            if (autofire != null) autofire.SetFireEnabled(true);
        }

        /// <summary>
        /// GameOver terminal. Idempotente: la primera línea corta la re-entrada y fija la
        /// invulnerabilidad en +∞. Limpia olas/formación/challenging, balas, dual y nave;
        /// SetState(GameOver) va ÚLTIMO (es el que apaga input/fire y escribe el score).
        /// </summary>
        public void TriggerGameOver()
        {
            if (CurrentState == State.GameOver) return;

            invulnerableUntil = float.PositiveInfinity;
            cameraShake?.ShakeGameOver();

            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }

            if (currentCarrier != null)
            {
                currentCarrier.Discard();
                currentCarrier = null;
            }

            if (inputDrag != null) inputDrag.enabled = false;
            waveManager?.StopWaves();
            autofire?.ClearBullets();
            autofire?.SetDual(false);
            if (autofire != null) autofire.SetFireEnabled(false);
            player?.HideForGameOver();

            // Récord: única escritura de la run (PlayerPrefs.Save explícito para WebGL).
            int finalScore = scoreManager != null ? scoreManager.Score : 0;
            bool isNewRecord = HighScoreStore.SaveIfHigher(finalScore);
            highScore = HighScoreStore.Load();
            if (recordText != null)
            {
                recordText.text = (isNewRecord ? newRecordPrefix : recordPrefix) + highScore;
            }

            SetState(State.GameOver);
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

        /// <summary>Un golpe aceptado = 1 flash naranja + 1 audio (nunca duplica el del enemigo).</summary>
        private void PlayPlayerDeathFx()
        {
            if (fxPool != null && player != null) fxPool.PlayPlayerDeath(player.transform.position);
            if (deathAudioSource != null && playerDeathClip != null)
            {
                deathAudioSource.PlayOneShot(playerDeathClip);
            }
        }

        private void UpdateLivesText()
        {
            if (livesText != null) livesText.text = livesPrefix + runState.Lives;
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

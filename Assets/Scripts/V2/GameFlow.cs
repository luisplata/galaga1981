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
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private PlayerController player;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private InputDrag inputDrag;
        [SerializeField] private Autofire autofire;

        private void Start()
        {
            SetState(State.Home);
        }

        public void StartGame()
        {
            scoreManager?.ResetScore();
            player?.ResetPosition();
            waveManager?.StartWaves();
            SetState(State.Playing);
        }

        public void OnEnemyKilled(Enemy killedEnemy)
        {
            // M1: el kill ya no termina la partida — notifica a WaveManager para el conteo
            // de la grilla (ola siguiente automática cuando queda vacía).
            waveManager?.OnEnemyKilled(killedEnemy);
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
    }
}
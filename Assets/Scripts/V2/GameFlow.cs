using TMPro;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Flujo de estados Home -> Playing -> GameOver con UN solo panel visible por estado.
    /// M0: al matar al único enemigo la partida termina (sin vidas ni muerte del jugador).
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
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private PlayerController player;
        [SerializeField] private EnemySimple enemy;
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
            enemy?.Respawn();
            SetState(State.Playing);
        }

        public void OnEnemyKilled(EnemySimple killedEnemy)
        {
            // M0: sin vidas ni muerte del jugador, el kill del único enemigo valida GameOver.
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
                gameOverScoreText.text = "PUNTOS: " + scoreManager.Score;
            }

            bool playing = newState == State.Playing;
            if (inputDrag != null) inputDrag.enabled = playing;
            if (autofire != null) autofire.SetFireEnabled(playing);
        }
    }
}
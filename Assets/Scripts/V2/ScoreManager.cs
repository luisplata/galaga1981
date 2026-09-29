using System;
using TMPro;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Puntaje acumulado + HUD. Solo cambia por kills en M0.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public event Action<int> ScoreChanged;

        public int Score { get; private set; }

        [SerializeField] private int initialScore = 0;
        [SerializeField] private TMP_Text scoreText;

        public void Add(int points)
        {
            Score += points;
            ScoreChanged?.Invoke(Score);
            UpdateUI();
        }

        public void ResetScore()
        {
            Score = initialScore;
            ScoreChanged?.Invoke(Score);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (scoreText != null)
            {
                scoreText.text = Score.ToString();
            }
        }
    }
}
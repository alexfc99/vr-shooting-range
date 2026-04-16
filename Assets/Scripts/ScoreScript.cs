using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class ScoreScript : MonoBehaviour
    {
        public static ScoreScript Instance { get; private set; }

        [Header("UI")] [SerializeField] private TMP_Text scoreText;

        [Header("Score")] [SerializeField] private int currentScore = 0;
        [SerializeField] private string prefix = "SCORE: ";

        // Evento para que otros scripts puedan escuchar cambios de score
        public event Action<int> OnScoreChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            UpdateScoreUI();
        }

        public void AddScore(int amount)
        {
            currentScore += amount;
            UpdateScoreUI();
        }

        public void RemoveScore(int amount)
        {
            currentScore -= amount;
            if (currentScore < 0)
                currentScore = 0;

            UpdateScoreUI();
        }

        public void SetScore(int newScore)
        {
            currentScore = Mathf.Max(0, newScore);
            UpdateScoreUI();
        }

        public int GetScore()
        {
            return currentScore;
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
                scoreText.text = prefix + currentScore;

            OnScoreChanged?.Invoke(currentScore);
        }
    }
}
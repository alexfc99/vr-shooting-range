using UnityEngine;
using UnityEngine.UIElements;

public class InterfaceScript : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private Label scoreLabel;
    private int score = 0;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        VisualElement root = uiDocument.rootVisualElement;
        scoreLabel = root.Q<Label>("ScoreText");

        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    public void SetScore(int newScore)
    {
        score = newScore;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreLabel != null)
            scoreLabel.text = "Score: " + score;
    }
}

using TMPro;
using UnityEngine;

public class ScoreCanvaCard : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score;
    void Start()
    {
        scoreText.SetText(scoreText.text  + "\n" + score);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.SetText(scoreText.text  + "\n" + score);
    }
}

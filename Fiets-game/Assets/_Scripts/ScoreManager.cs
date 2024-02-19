using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreText; // Reference to the UI Text component to display the score
    public TextMeshProUGUI highScoreText; // Reference to the UI Text component to display the high score

    private int score = 0;
    private int highScore = 0;

    private float scoreIncreaseTimer = 0f;
    private float scoreIncreaseInterval = 0.05f; // Adjust this to control the speed of the score increase

    void Awake()
    {
        // Singleton pattern to ensure only one instance of the ScoreManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Load the high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Initialize the score and update the UI
        score = 0;
        UpdateScoreUI();
        UpdateHighScoreUI();
    }

    void Update()
    {
        // Increase the score over time with a delay
        if (EndlessRunner.Instance.hasStarted)
        {
            scoreIncreaseTimer += Time.deltaTime;

            if (scoreIncreaseTimer >= scoreIncreaseInterval)
            {
                IncreaseScore(1);
                scoreIncreaseTimer = 0f;
            }
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;

        // Update the high score if the current score surpasses it
        if (score > highScore)
        {
            highScore = score;
            SaveHighScore(); // Save the new high score to PlayerPrefs
            UpdateHighScoreUI();
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        // Update the UI Text to display the current score with leading zeros
        scoreText.text = score.ToString("D6");
    }

    void UpdateHighScoreUI()
    {
        // Update the UI Text to display the high score with leading zeros
        highScoreText.text = "Best: " + highScore.ToString("D6");
    }

    void SaveHighScore()
    {
        // Save the high score to PlayerPrefs
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
}
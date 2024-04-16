using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int score;                                // The player's score
    [SerializeField] private TextMeshProUGUI scoreText;                // Reference to the Text component

    void Awake()
    {
        // Set up the reference
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();

        // Reset the score
        score = 0;
    }
    
    void Update()
    {
        // Set the displayed text to be the word "Score" followed by the score value
        scoreText.text = "Score: " + score;
    }
}
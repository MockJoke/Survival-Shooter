using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static int score;                                // The player's score
    [SerializeField] private Text scoreText;                // Reference to the Text component

    void Awake()
    {
        // Set up the reference
        if (scoreText == null)
            scoreText = GetComponent<Text>();

        // Reset the score
        score = 0;
    }
    
    void Update()
    {
        // Set the displayed text to be the word "Score" followed by the score value
        scoreText.text = "Score: " + score;
    }
}
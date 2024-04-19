using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score;                                            // The player's score
    [SerializeField] private TextMeshProUGUI scoreText;

    public static Action<int> OnScoreChange;

    void Awake()
    {
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();

        OnScoreChange += UpdateScore;
    }

    void Start()
    {
        score = 0;
        DisplayScoreUI();
    }

    void OnDestroy()
    {
        OnScoreChange -= UpdateScore;
    }

    private void UpdateScore(int val)
    {
        LeanTween.cancel(scoreText.gameObject);
        
        LeanTween.value(scoreText.gameObject, SetScaleCallback, 1f, 1.35f, 0.15f).
            setOnStart(()=>
            {
                LeanTween.value(scoreText.gameObject, SetColorCallback, new Color(1, 1, 1, 1), new Color(1, 1, 0, 1), 0.15f);
                score += val;
                DisplayScoreUI();
            })
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                LeanTween.value(scoreText.gameObject, SetScaleCallback, 1.35f, 1f, 0.5f);
                LeanTween.value(scoreText.gameObject, SetColorCallback, new Color(1, 1, 0, 1), new Color(1, 1, 1, 1), 0.5f);
            })
            .setEase(LeanTweenType.easeInOutSine);
    }

    private void SetColorCallback(Color c)
    {
        scoreText.color = c;
    }
    
    private void SetScaleCallback(float s)
    {
        scoreText.transform.localScale = new Vector3(s, s, s);
    }

    private void DisplayScoreUI()
    {
        scoreText.text = $"{score}";
    }
}
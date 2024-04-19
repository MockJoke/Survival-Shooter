using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Image screenFader;
    [SerializeField] private PlayerHealth playerHealth;
    // [SerializeField] private Animator anim;

    public static Action OnGameOver;
    
    // private static readonly int gameOverAnim = Animator.StringToHash("GameOver");

    void Awake()
    {
        if (gameOverText == null)
            gameOverText = GetComponent<TextMeshProUGUI>();
        
        // if (anim == null)
        //     anim = GetComponent<Animator>();

        if (playerHealth == null)
            playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        OnGameOver += DisplayGameOverScreen;
    }

    void OnDestroy()
    {
        OnGameOver -= DisplayGameOverScreen;
    }

    // void Update()
    // {
    //     if(playerHealth.currentHealth <= 0)
    //     {
    //         anim.SetTrigger(gameOverAnim);
    //     }
    // }

    private void DisplayGameOverScreen()
    {
        LeanTween.cancel(screenFader.gameObject);
        LeanTween.cancel(gameOverText.gameObject);
        
        LeanTween.value(screenFader.gameObject, SetImageColorCallback, new Color(0, 0, 0, 0), new Color(0.1f, 0.05f, 0.1f, 1), 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnStart(() =>
            {
                LeanTween.value(gameOverText.gameObject, SetTextColorCallback, new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 0.5f);
            })
            .setEase(LeanTweenType.easeInOutSine);
        
        LeanTween.scale(gameOverText.gameObject, new Vector3(1.85f, 1.85f, 1.85f), 0.45f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(()=>
            {
                LeanTween.scale(gameOverText.gameObject, new Vector3(1.35f, 1.35f, 1.35f), 0.25f);
            })
            .setEase(LeanTweenType.easeInOutSine);
    }
    
    private void SetImageColorCallback(Color c)
    {
        screenFader.color = c;
    }
    
    private void SetTextColorCallback(Color c)
    {
        gameOverText.color = c;
    }
}
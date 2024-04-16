using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;       // Reference to the player's health
    [SerializeField] private Animator anim;                   // Reference to the animator component
    
    private static readonly int gameOverAnim = Animator.StringToHash("GameOver");

    void Awake()
    {
        // Set up the reference
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    void Update()
    {
        // If the player has run out of health...
        if(playerHealth.currentHealth <= 0)
        {
            // ... tell the animator the game is over
            anim.SetTrigger(gameOverAnim);
        }
    }
}
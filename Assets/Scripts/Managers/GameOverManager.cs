using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Animator anim;
    
    private static readonly int gameOverAnim = Animator.StringToHash("GameOver");

    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(playerHealth.currentHealth <= 0)
        {
            anim.SetTrigger(gameOverAnim);
        }
    }
}
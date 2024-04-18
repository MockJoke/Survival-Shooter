using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack
    [SerializeField] private int attackDamage = 10;               // The amount of health taken away per attack
    
    [Header("Components")]
    [SerializeField] private Animator anim;                              // Reference to the animator component
    [SerializeField] private GameObject player;                          // Reference to the player GameObject
    [SerializeField] private PlayerHealth playerHealth;                  // Reference to the player's health
    [SerializeField] private EnemyHealth enemyHealth;                    // Reference to this enemy's health
    
    private bool playerInRange;                                 // Whether player is within the trigger collider and can be attacked
    private float timer;                                        // Timer for counting up to the next attack
    
    private static readonly int playerDeadAnim = Animator.StringToHash("PlayerDead");

    void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        
        if (playerHealth == null)
            playerHealth = player.GetComponent<PlayerHealth>();
        
        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();
        
        if (anim == null)
            anim = GetComponent<Animator>();
    }
    
    void OnTriggerEnter(Collider collision)
    {
        // If the entering collider is the player...
        if(collision.gameObject == player)
        {
            // ... the player is in range
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        // If the exiting collider is the player...
        if(collision.gameObject == player)
        {
            // ... the player is no longer in range
            playerInRange = false;
        }
    }

    void Update()
    {
        // Add the time since Update was last called to the timer
        timer += Time.deltaTime;

        // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
        if(timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
        {
            // ... attack
            Attack();
        }

        // If the player has zero or less health...
        if(playerHealth.currentHealth <= 0)
        {
            // ... tell the animator the player is dead
            anim.SetTrigger(playerDeadAnim);
        }
    }
    
    void Attack()
    {
        // Reset the timer
        timer = 0f;

        // If the player has health to lose...
        if(playerHealth.currentHealth > 0)
        {
            // ... damage the player
            playerHealth.TakeDamage(attackDamage);
        }
    }
}
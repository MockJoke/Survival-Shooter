using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private UnityEngine.AI.NavMeshAgent nav;
    
    private Transform player;
    private PlayerHealth playerHealth;
    
    void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (playerHealth == null)
            playerHealth = player.GetComponent<PlayerHealth>();
        
        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();
        
        if (nav == null)
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    
    void Update()
    {
        if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
        {
            nav.SetDestination(player.position);
        }
        else
        {
            nav.enabled = false;
        }
    }
}
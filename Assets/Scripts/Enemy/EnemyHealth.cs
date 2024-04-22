using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int initHealth = 100;                // The amount of health the enemy starts the game with
    public int currentHealth { get; private set; }                // The current health the enemy has
    // [SerializeField] private float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead
    [SerializeField] private int scoreValue = 10;                 // The amount added to the player's score when the enemy dies
    [SerializeField] private AudioClip deathClip;                 // The sound to play when the enemy dies
    [SerializeField] private AudioClip burnClip;                  // The sound to play when the enemy is burning

    [Header("HealthBar UI")] 
    [SerializeField] private GameObject healthBarPrefab;
    private Slider healthBarSlider;
    private GameObject enemyHealthCanvas;
    
    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource enemyAudio;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
    
    private Camera mainCamera;
    private static readonly int deadAnim = Animator.StringToHash("Dead");

    void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
        
        if (enemyAudio == null)
            enemyAudio = GetComponent<AudioSource>();
        
        if (capsuleCollider == null)
            capsuleCollider = GetComponent<CapsuleCollider>();

        if (enemyMovement == null)
            enemyMovement = GetComponent<EnemyMovement>();

        if (enemyHealthCanvas == null)
            enemyHealthCanvas = GameObject.FindGameObjectWithTag("EnemyHealthCanvas");

        if (bodyMeshRenderer == null)
            bodyMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void OnEnable()
    {
        currentHealth = initHealth;

        healthBarSlider = Instantiate(healthBarPrefab, gameObject.transform.position, Quaternion.identity, enemyHealthCanvas.transform).GetComponent<Slider>();
        healthBarSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!IsDead())
        {
            UpdateHealthBarPos();
        }
    }

    public void SetInitHealthBasedOnDifficulty(int diffFactor)
    {
        initHealth *= diffFactor;
    }
    
    public void SetScoreValueBasedOnDifficulty(int diffFactor)
    {
        scoreValue *= diffFactor;
    }
    
    public bool IsDead()
    {
        return (currentHealth <= 0f);
    }
    
    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        ApplyDamage(amount);
        
        // Set the position of the particle system to where the hit was sustained
        hitParticles.transform.position = hitPoint;

        // And play the particles
        hitParticles.Play();
    }
    
    public void TakeDamage(int amount)
    {
        ApplyDamage(amount);
    }
    
    private void ApplyDamage(int amount)
    {
        if (!IsDead())
        {
            // Play the hurt sound effect
            enemyAudio.Play();
            
            currentHealth -= amount;
            
            healthBarSlider.gameObject.SetActive(true);
            healthBarSlider.value = (int)Mathf.Round(((float)currentHealth / (float)initHealth) * 100);

            if (IsDead())
            {
                Death();
            }
        }
    }
    
    void Death()
    {
        // Tell the animator that the enemy is dead
        anim.SetTrigger(deadAnim);
        
        // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing)
        enemyAudio.clip = deathClip;
        enemyAudio.Play();
        
        // Find and disable the Nav Mesh Agent
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        
        // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy)
        GetComponent<Rigidbody>().isKinematic = true;
        
        // Turn the collider into a trigger so shots can pass through it
        capsuleCollider.isTrigger = true;
        
        // Increase the score by the enemy's score value
        ScoreManager.OnScoreChange(scoreValue);
        
        Destroy(healthBarSlider.gameObject);
        
        StartCoroutine(StartBurning());
    }
    
    IEnumerator StartBurning()
    {
        yield return new WaitForSeconds(1f);

        // while (transform.position.y > -10f)
        // {
        //     // ... move the enemy down by the sinkSpeed per second
        //     transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
        // }
        
        deathParticles.Play();
        
        // Change the audio clip of the audio source to the burn clip and play it (this will stop the death clip playing)
        enemyAudio.clip = burnClip;
        enemyAudio.Play();

        WaveManager.DeduceAliveEnemies();
        
        // After 2 seconds destroy the enemy
        Destroy(gameObject, 1.5f);
    }

    private void UpdateHealthBarPos()
    {
        if (mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            healthBarSlider.transform.position = screenPos + new Vector3(0, 100, 0);
        }
    }
}
using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 100;            // The amount of health the enemy starts the game with
    public int currentHealth;                                     // The current health the enemy has
    [SerializeField] private float sinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead
    [SerializeField] private int scoreValue = 10;                 // The amount added to the player's score when the enemy dies
    [SerializeField] private AudioClip deathClip;                 // The sound to play when the enemy dies
    
    [SerializeField] private Animator anim;                              // Reference to the animator
    [SerializeField] private AudioSource enemyAudio;                     // Reference to the audio source
    [SerializeField] private ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged
    [SerializeField] private CapsuleCollider capsuleCollider;            // Reference to the capsule collider
    [SerializeField] private EnemyMovement enemyMovement;
    
    private bool isDead;                                // Whether the enemy is dead
    private bool isSinking;                             // Whether the enemy has started sinking through the floor

    void Awake()
    {
        // Setting up the references
        if (anim == null)
            anim = GetComponent<Animator>();
        
        if (enemyAudio == null)
            enemyAudio = GetComponent<AudioSource>();
        
        if (hitParticles == null)
            hitParticles = GetComponentInChildren<ParticleSystem>();
        
        if (capsuleCollider == null)
            capsuleCollider = GetComponent<CapsuleCollider>();

        if (enemyMovement == null)
            enemyMovement = GetComponent<EnemyMovement>();
    }

    void OnEnable()
    {
        // Setting the current health when the enemy first spawns
        currentHealth = startingHealth;
    }

    void Update()
    {
        // If the enemy should be sinking...
        if(isSinking)
        {
            // ... move the enemy down by the sinkSpeed per second
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
            
            if (transform.position.y < -10f)
            {
                Destroy(this.gameObject);
            }
        }
    }
    
    public bool IsDead()
    {
        return (currentHealth <= 0f);
    }
    
    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        // If the enemy is dead...
        // if(isDead)
            // ... no need to take damage so exit the function
            // return;

        // Play the hurt sound effect
        // enemyAudio.Play();

        // Reduce the current health by the amount of damage sustained
        // currentHealth -= amount;
        
        ApplyDamage(amount);
        
        // Set the position of the particle system to where the hit was sustained
        hitParticles.transform.position = hitPoint;

        // And play the particles
        hitParticles.Play();

        // If the current health is less than or equal to zero...
        // if(currentHealth <= 0)
        // {
        //     // ... the enemy is dead
        //     Death();
        // }
    }
    
    public void TakeDamage(int amount)
    {
        ApplyDamage(amount);
    }
    
    private void ApplyDamage(int amount)
    {
        if (!IsDead())
        {
            enemyAudio.Play();
            currentHealth -= amount;

            if (IsDead())
            {
                Death();
            }
        }
    }
    
    void Death()
    {
        // The enemy is dead
        // isDead = true;

        // Turn the collider into a trigger so shots can pass through it
        capsuleCollider.isTrigger = true;

        // Tell the animator that the enemy is dead
        anim.SetTrigger("Dead");

        // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing)
        enemyAudio.clip = deathClip;
        enemyAudio.Play();
    }
    
    public void StartSinking()
    {
        // Find and disable the Nav Mesh Agent
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy)
        GetComponent<Rigidbody>().isKinematic = true;

        // The enemy should not sink
        isSinking = true;

        // Increase the score by the enemy's score value
        ScoreManager.score += scoreValue;

        // After 2 seconds destroy the enemy
        Destroy(gameObject, 2f);
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 100;                                      // The amount of health the player starts the game with
    public int currentHealth { get; private set; }
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image damageImage;                                             // Reference to an image to flash on the screen on being hurt
    [SerializeField] private float flashSpeed = 5f;                                         // The speed the damageImage will fade at
    [SerializeField] private Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash
    [SerializeField] private AudioClip deathClip;
    
    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerShooting playerShooting;
    
    private bool isDead;
    private bool damaged;
    private static readonly int dieAnim = Animator.StringToHash("Die");

    void Awake()
    {
        if (anim == null)
            anim = GetComponent <Animator>();
        
        if (playerAudio == null)
            playerAudio = GetComponent <AudioSource>();
        
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
        
        if (playerShooting == null)
            playerShooting = GetComponentInChildren<PlayerShooting>();
    }

    void Start()
    {
        currentHealth = startingHealth;
    }

    void Update()
    {
        // If the player has just been damaged...
        if(damaged)
        {
            // ... set the colour of the damageImage to the flash colour
            damageImage.color = flashColour;
        }
        else
        {
            // ... transition the colour back to clear
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag
        damaged = false;
    }
    
    public void TakeDamage(int amount)
    {
        // Set the damaged flag so the screen will flash
        damaged = true;

        currentHealth -= amount;

        healthSlider.value = currentHealth;

        // Play the hurt sound effect
        playerAudio.Play();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if(currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }
    
    void Death()
    {
        // Set the death flag so this function won't be called again
        isDead = true;

        GameOverManager.OnGameOver();
        
        // Turn off any remaining shooting effects
        playerShooting.DisableEffects();

        anim.SetTrigger(dieAnim);

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing)
        playerAudio.clip = deathClip;
        playerAudio.Play();

        // Turn off the movement and shooting scripts
        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}
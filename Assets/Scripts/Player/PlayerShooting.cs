using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private int damagePerShot = 20;                        // The damage inflicted by each bullet
    [SerializeField] private float timeBetweenBullets = 0.15f;              // The time between each shot
    [SerializeField] private float angleBetweenBullets = 10f;
    [SerializeField] private float range = 100f;                            // The distance the gun can fire
    [SerializeField] private LayerMask shootableMask;                       // A layer mask so the raycast only hits things on the shootable layer
    [SerializeField] private float effectsDisplayTime = 0.2f;               // The proportion of the timeBetweenBullets that the effects will display for
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnAnchor;
    [SerializeField] private Transform bulletHolder;
    [SerializeField] private int bulletsCnt = 1;
    
    [SerializeField] private float bounceDuration = 10f;
    [SerializeField] private float pierceDuration = 10f;

    [SerializeField] private Color[] bulletColors;

    [SerializeField] private bool lineBasedShooting = true;
    
    [Header("UI")]
    [SerializeField] private Image bounceImg;
    [SerializeField] private Image pierceImg;
    
    [Header("Components")]
    [SerializeField] private ParticleSystem gunParticles;                    // Reference to the particle system
    [SerializeField] private LineRenderer gunLine;                           // Reference to the line renderer
    [SerializeField] private AudioSource gunAudio;                           // Reference to the audio source
    [SerializeField] private Light gunLight;                                 // Reference to the light component
    [SerializeField] private Light faceLight;
    
    private float timer = 0f;                                   // A timer to determine when to fire
    private Ray shootRay = new Ray();                           // A ray from the gun end forwards
    private RaycastHit shootHit;                                // A raycast hit to get information about what was hit

    private Color bulletColor;
    
    private bool bouncing = false;
    private float bounceTimer = 0f;
    private bool piercing = false;
    private float pierceTimer = 0f;
    
    void Awake()
    {
        if (gunParticles == null)
            gunParticles = GetComponent<ParticleSystem>();
        
        if (gunLine == null)
            gunLine = GetComponent<LineRenderer>();
        
        if (gunAudio == null)
            gunAudio = GetComponent<AudioSource>();
        
        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;
    }
    
    void Update()
    {
        // Add the time since Update was last called to the timer
        timer += Time.deltaTime;
        
#if !MOBILE_INPUT
        // If the Fire1 button is being pressed and it's time to fire...
        if(Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            // ... shoot the gun
            if (lineBasedShooting)
            {
                ShootLine();
            }
            else
            {
                ShootBullet();
            }
        }
#else
        // If there is input on the shoot direction stick and it's time to fire...
        if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
        {
            // ... shoot the gun
            if (lineBasedShooting)
            {
                ShootLine();
            }
            else
            {
                ShootBullet();
            }
        }
#endif
        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if(timer >= timeBetweenBullets * effectsDisplayTime)
        {
            // ... disable the effects
            DisableEffects();
        }
        
        bulletColor = bulletColors[0];
        
        if (!lineBasedShooting)
        {
            bounceTimer += Time.deltaTime;
            pierceTimer += Time.deltaTime;
            
            bouncing = bounceTimer < bounceDuration;

            piercing = pierceTimer < pierceDuration;
            
            if (piercing && bouncing)
            {
                bulletColor = bulletColors[3];
                bounceImg.color = bulletColors[3];
                pierceImg.color = bulletColors[3];
            }
            else
            {
                if (bouncing) 
                {
                    bulletColor = bulletColors[1];
                    bounceImg.color = bulletColors[1];
                }
                
                if (piercing) 
                {
                    bulletColor = bulletColors[2];
                    pierceImg.color = bulletColors[2];
                }
            }
            
            bounceImg.gameObject.SetActive(bouncing);
            pierceImg.gameObject.SetActive(piercing);
        }
        
        // gunParticles.startColor = bulletColor;
        ChangeParticlesColor(gunParticles, bulletColor);
        gunLight.color = bulletColor;

        // For some reason the color I had selected originally looked extremely
        // reddish after I switched to deferred rendering and linear mode so 
        // I'm hardcoding in a lighter, more yellow light color if you have
        // both the pierce and bounce power-up active.
        // gunLight.color = (piercing && bouncing) ? new Color(1, 140f / 255f, 30f / 255f, 1) : bulletColor;
    }
    
    public void DisableEffects()
    {
        // Disable the line renderer and the light
        if (lineBasedShooting)
        {
            gunLine.enabled = false;
        }
        faceLight.enabled = false;
        gunLight.enabled = false;
    }
    
    private void ShootLine()
    {
        // Reset the timer
        timer = 0f;

        // Play the gun shot audio-clip
        gunAudio.Play();

        // Enable the lights
        gunLight.enabled = true;
        faceLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if(Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            // Try and find an EnemyHealth script on the gameobject hit
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

            // If the EnemyHealth component exist...
            if(enemyHealth != null)
            {
                // ... the enemy should take damage
                enemyHealth.TakeDamage(damagePerShot, shootHit.point);
            }

            // Set the second position of the line renderer to the point the raycast hit
            gunLine.SetPosition(1, shootHit.point);
        }
        // If the raycast didn't hit anything on the shootable layer...
        else
        {
            // ... set the second position of the line renderer to the fullest extent of the gun's range
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
    }
    
    [ContextMenu("Shoot Bullet")]
    private void ShootBullet()
    {
        // Reset the timer.
        timer = 0f;

        // Play the gun shot audioclip
        gunAudio.pitch = Random.Range(1.2f, 1.3f);
        
        if (piercing && bouncing) 
        {
            gunAudio.pitch = Random.Range(0.9f, 1.0f);
        }
        else
        {
            if (bouncing) 
            {
                gunAudio.pitch = Random.Range(1.1f, 1.2f);
            }
            
            if (piercing) 
            {
                gunAudio.pitch = Random.Range(1.0f, 1.1f);
            }
        }
        
        gunAudio.Play();

        gunLight.intensity = 2 + (0.25f * (bulletsCnt - 1));
        gunLight.enabled = true;

        gunParticles.Stop();
        // gunParticles.startSize = 1 + (0.1f * (bulletsCnt - 1));
        ChangeParticlesSize(gunParticles, (1 + (0.1f * (bulletsCnt - 1))));
        gunParticles.Play();

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel
        // shootRay.origin = transform.position;
        // shootRay.direction = transform.forward;

        for (int i = 0; i < bulletsCnt; i++) 
        {
            // Make sure our bullets spread out in an even pattern
            float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (bulletsCnt - 1));
            Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
            GameObject instantiatedBullet = Instantiate(bulletPrefab, bulletSpawnAnchor.transform.position, rot, bulletHolder) as GameObject;
            instantiatedBullet.GetComponent<PlayerBullet>().piercing = piercing;
            instantiatedBullet.GetComponent<PlayerBullet>().bouncing = bouncing;
            instantiatedBullet.GetComponent<PlayerBullet>().bulletColor = bulletColor;
        }
    }
    
    private void ChangeParticlesSize(ParticleSystem particles, float s)
    {
        var main = particles.main;
        main.startSize = s;
    }
    
    private void ChangeParticlesColor(ParticleSystem particles, Color color)
    {
        var main = particles.main;
        main.startColor = color;
    }
}
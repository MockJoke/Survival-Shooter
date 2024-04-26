using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float speed = 600.0f;
    [SerializeField] private float life = 3;
    [SerializeField] private ParticleSystem normalTrailParticles;
    [SerializeField] private ParticleSystem bounceTrailParticles;
    [SerializeField] private ParticleSystem pierceTrailParticles;
    [SerializeField] private ParticleSystem ImpactParticles;
    [SerializeField] private int damage = 20;
    [SerializeField] private LayerMask shootableMask;                 // A layer mask so the raycast only hits things on the shootable layer
    
    [HideInInspector] public Color bulletColor;
    [HideInInspector] public bool piercing = false;
    [HideInInspector] public bool bouncing = false;
    
    [Header("Audio")]
    [SerializeField] private AudioSource bulletAudio;  
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip hitSound;

	private Vector3 velocity;
	private Vector3 force;
	private Vector3 newPos;
	private Vector3 oldPos;
	private Vector3 direction;
	private bool hasHit = false;
	private RaycastHit lastHit;
	// Reference to the audio source.
	private float timer;

	void Awake() 
	{
		if (bulletAudio == null)
			bulletAudio = GetComponent<AudioSource> ();
	}

	void Start() 
	{
		newPos = transform.position;
		oldPos = newPos;

		// Set our particle colors.
		// normalTrailParticles.startColor = bulletColor;
		// bounceTrailParticles.startColor = bulletColor;
		// pierceTrailParticles.startColor = bulletColor;
		// ImpactParticles.startColor = bulletColor;
		ChangeParticlesColor(normalTrailParticles, bulletColor);
		ChangeParticlesColor(bounceTrailParticles, bulletColor);
		ChangeParticlesColor(pierceTrailParticles, bulletColor);
		ChangeParticlesColor(ImpactParticles, bulletColor);

		normalTrailParticles.gameObject.SetActive(true);
		
		if (bouncing) 
		{
			bounceTrailParticles.gameObject.SetActive(true);
			normalTrailParticles.gameObject.SetActive(false);
			life = 1;
			speed = 20;
		}
		
		if (piercing) 
		{
			pierceTrailParticles.gameObject.SetActive(true);
			normalTrailParticles.gameObject.SetActive(false);
			speed = 40;
		}
	}
	
	void Update() 
	{
		if (hasHit) 
		{
			return;
		}

		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;

		// Schedule for destruction if bullet never hits anything.
		if (timer >= life) 
		{
			Dissipate();
		}

        velocity = transform.forward;
		velocity.y = 0;
		velocity = velocity.normalized * speed;

		// assume we move all the way
		newPos += velocity * Time.deltaTime;
	
		// Check if we hit anything on the way
		direction = newPos - oldPos;
		float distance = direction.magnitude;

		if (distance > 0) 
		{
            // RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance);
            RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance, shootableMask);

		    // Find the first valid hit
		    for (int i = 0; i < hits.Length; i++) 
		    {
		        RaycastHit hit = hits[i];

				if (ShouldIgnoreHit(hit)) 
				{
					continue;
				}

				// notify hit
				OnHit(hit);

				lastHit = hit;

				if (hasHit) 
				{
					newPos = hit.point;
					break;
				}
		    }
		}

		oldPos = transform.position;
		transform.position = newPos;
	}

	/**
	 * So we don't hit the same enemy twice with the same raycast when we have
	 * piercing shots. The shot can still bounce on a wall, come back and hit
	 * the enemy again if we have both bouncing and piercing shots.
	 */
	bool ShouldIgnoreHit (RaycastHit hit) 
	{
		if (lastHit.point == hit.point || lastHit.collider == hit.collider)
			return true;
		
		return false;
	}
	
	void OnHit(RaycastHit hit) 
	{
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        if (hit.transform.CompareTag("Environment")) 
        {
			newPos = hit.point;
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();
			
			if (bouncing) 
			{
				Vector3 reflect = Vector3.Reflect(direction, hit.normal);
				transform.forward = reflect;
				bulletAudio.clip = bounceSound;
				bulletAudio.pitch = Random.Range(0.8f, 1.2f);
				bulletAudio.Play();
			}
			else 
			{
				hasHit = true;
				bulletAudio.clip = hitSound;
				bulletAudio.volume = 0.5f;
				bulletAudio.pitch = Random.Range(1.2f, 1.3f);
				bulletAudio.Play();
				DelayedDestroy();
			}
        }

        if (hit.transform.CompareTag("Enemy")) 
        {
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();

			// Try and find an EnemyHealth script on the gameobject hit.
			EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
			
			// If the EnemyHealth component exist...
			if (enemyHealth != null) 
			{
				// ... the enemy should take damage.
				enemyHealth.TakeDamage(damage, hit.point);
			}
			
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(1.2f, 1.3f);
			bulletAudio.Play();
			
			if (!piercing) 
			{
            	hasHit = true;
				DelayedDestroy();
			}
        }
	}

	// Just a method for destroying the game object, but which
	// first detaches the particle effect and leaves it for a
	// second. Called if the bullet end its life in midair
	// so we get an effect of the bullet fading out instead
	// of disappearing immediately.
	void Dissipate() 
	{
		// normalTrailParticles.enableEmission = false;
		ToggleEmission(normalTrailParticles, false);
		normalTrailParticles.transform.parent = null;
		// Destroy(normalTrailParticles.gameObject, normalTrailParticles.duration);
		Destroy(normalTrailParticles.gameObject, GetDuration(normalTrailParticles));

		if (bouncing) 
		{
			// bounceTrailParticles.enableEmission = false;
			ToggleEmission(bounceTrailParticles, false);
			bounceTrailParticles.transform.parent = null;
			// Destroy(bounceTrailParticles.gameObject, bounceTrailParticles.duration);
			Destroy(bounceTrailParticles.gameObject, GetDuration(bounceTrailParticles));
		}
		if (piercing) 
		{
			// pierceTrailParticles.enableEmission = false;
			ToggleEmission(pierceTrailParticles, false);
			pierceTrailParticles.transform.parent = null;
			// Destroy(pierceTrailParticles.gameObject, pierceTrailParticles.duration);
			Destroy(pierceTrailParticles.gameObject, GetDuration(pierceTrailParticles));
		}

		Destroy(gameObject);
	}

	void DelayedDestroy() 
	{
		normalTrailParticles.gameObject.SetActive(false);
		
		if (bouncing) 
		{
			bounceTrailParticles.gameObject.SetActive(false);
		}
		if (piercing) 
		{
			pierceTrailParticles.gameObject.SetActive(false);
		}
		
		Destroy(gameObject, hitSound.length);
	}
	
	private void ChangeParticlesColor(ParticleSystem particles, Color color)
	{
		var main = particles.main;
		main.startColor = color;
	}

	private void ToggleEmission(ParticleSystem particles, bool toggle)
	{
		var em = particles.emission;
		em.enabled = toggle;
	}

	private float GetDuration(ParticleSystem particles)
	{
		var main = particles.main;
		return main.duration;
	}
}
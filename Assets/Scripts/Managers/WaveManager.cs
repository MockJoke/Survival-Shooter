using System;
using UnityEngine;
using System.Collections;
using TMPro;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour 
{
	[SerializeField] private PlayerHealth playerHealth;
	[SerializeField] private float bufferDistance = 200;		// The distance from our Camera View Frustum we want to spawn enemies to make sure they are not visible when they spawn
	[SerializeField] private float timeBwWaves = 3.5f;			// The time in seconds between each wave
    [SerializeField] private float spawnTime = 3f;				// The time in seconds between each spawn in a wave
    [SerializeField] private int startingWave = 1;
    [SerializeField] private int startingDifficulty = 1;
    
	[SerializeField] private TextMeshProUGUI waveNoText; 
	
	private int enemiesAlive = 0;								// The number of enemies left alive for the current wave

    // A class depicting one wave with x number of entries.
    [Serializable]
    public class Wave 
    {
        public Entry[] entries;

        // A class depicting one wave entry
        [Serializable]
        public class Entry 
        {
            public GameObject enemy;									// The enemy type to spawn
            public int enemiesToSpawnCount;								// The number of enemies to spawn
            [NonSerialized] public int spawnedEnemiesCount;				// A counter telling us how many have been spawned so far
        }
    }

    [SerializeField] private Wave[] waves;
    [SerializeField] private bool spawnAccToCameraBlindSpots = false;
    [SerializeField] private Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from

    private Vector3 spawnPosition = Vector3.zero;		// pos at which the enemy will be spawned
    private int currWaveNumber;
    private float timer; 
    private Wave currWave;
    private int noOfSpawnedEnemies = 0;
    private int totalEnemiesToSpawn;
    private bool shouldSpawn = false;
    private int difficulty;

    private Camera mainCamera;
    public static Action DeduceAliveEnemies;

    void Awake()
    {
	    mainCamera = Camera.main;
	    DeduceAliveEnemies += DecreaseEnemiesAlive;
    }

    void Start() 
	{
		waveNoText.text = "0";
		
		// Let us start on a higher wave and difficulty if we wish
		currWaveNumber = startingWave > 0 ? startingWave - 1 : 0;
		
		difficulty = startingDifficulty;

		// Start the next ie. the first wave
		StartCoroutine(nameof(StartNextWave));
	}
	
	void Update() 
	{
		// This is false while we're setting up the next wave
		if (!shouldSpawn) 
		{
			return;
        }

		// Start the next wave when we've spawned all our enemies and the player has killed them all
		if (noOfSpawnedEnemies == totalEnemiesToSpawn && enemiesAlive == 0) 
		{
			StartCoroutine(nameof(StartNextWave));
			return;
		}

        // Add the time since Update was last called to the timer.
		timer += Time.deltaTime;
        
        // If the timer exceeds the time between attacks, the player is in range and this enemy is alive attack
		if (timer >= spawnTime) 
		{
			// Spawn one enemy from each of the entries in this wave
            // The difficulty multiplies the number of spawned enemies for each loop, that is each full run through all the waves
			foreach (Wave.Entry entry in currWave.entries) 
			{
				if (entry.spawnedEnemiesCount < (entry.enemiesToSpawnCount * difficulty)) 
				{
					Spawn(entry);
				}
			}
		}
	}

	void OnDestroy()
	{
		DeduceAliveEnemies -= DecreaseEnemiesAlive;
	}

	IEnumerator StartNextWave() 
	{
		shouldSpawn = false;

		yield return new WaitForSeconds(timeBwWaves);

		if (currWaveNumber == waves.Length) 
		{
			currWaveNumber = 0;
			difficulty++;
		}

		currWave = waves[currWaveNumber];

        // The difficulty multiplies the number of spawned enemies for each loop
        // that is each full run through all the waves
        totalEnemiesToSpawn = 0;
		foreach (Wave.Entry entry in currWave.entries) 
		{
			totalEnemiesToSpawn += (entry.enemiesToSpawnCount * difficulty);
		}

		noOfSpawnedEnemies = 0;
		shouldSpawn = true;

		currWaveNumber++;

		UpdateWaveNo(currWaveNumber + ((difficulty - 1) * waves.Length));
	}
	
	private void UpdateWaveNo(int val)
	{
		LeanTween.cancel(waveNoText.gameObject);
        
		LeanTween.value(waveNoText.gameObject, SetScaleCallback, 1f, 1.35f, 0.15f).
			setOnStart(()=>
			{
				LeanTween.value(waveNoText.gameObject, SetColorCallback, new Color(1, 1, 1, 1), new Color(1, 1, 0, 1), 0.15f);
				waveNoText.text = $"{val}";
			})
			.setEase(LeanTweenType.easeInOutSine)
			.setOnComplete(() =>
			{
				LeanTween.value(waveNoText.gameObject, SetScaleCallback, 1.35f, 1f, 0.5f);
				LeanTween.value(waveNoText.gameObject, SetColorCallback, new Color(1, 1, 0, 1), new Color(1, 1, 1, 1), 0.5f);
			})
			.setEase(LeanTweenType.easeInOutSine);
	}
	
	private void SetColorCallback(Color c)
	{
		waveNoText.color = c;
	}
    
	private void SetScaleCallback(float s)
	{
		waveNoText.transform.localScale = new Vector3(s, s, s);
	}
	
	void Spawn(Wave.Entry entry) 
	{
		// Reset the timer
		timer = 0f;
		
		// If the player has no health left, stop spawning
		if (playerHealth.currentHealth <= 0f) 
		{
			return;
		}

		GameObject enemy = null;
		while (!enemy)
		{
			enemy = spawnAccToCameraBlindSpots ? SpawnBasedOnCameraBlindSpots(entry.enemy) : SpawnBasedOnSpawnPoints(entry.enemy);
		}
		
		// Multiply health and score value by the current difficulty.
		enemy.GetComponent<EnemyHealth>().SetInitHealthBasedOnDifficulty(difficulty);
		enemy.GetComponent<EnemyHealth>().SetScoreValueBasedOnDifficulty(difficulty);
		
		entry.spawnedEnemiesCount++;
		noOfSpawnedEnemies++;
		enemiesAlive++;
	}

	private GameObject SpawnBasedOnSpawnPoints(GameObject enemy)
	{
		// Find a random index between zero and one less than the number of spawn points
		int spawnPointIndex = Random.Range(0, spawnPoints.Length);

		// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation
		return Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
	}

	private GameObject SpawnBasedOnCameraBlindSpots(GameObject enemy)
	{
		// Find a random position roughly on the level
		Vector3 randomPosition = Random.insideUnitSphere * 35;
		randomPosition.y = 0;
		
		// Find the closest position on the nav mesh to our random position
		// If we can't find a valid position return and try again
		if (!UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out UnityEngine.AI.NavMeshHit hit, 5, 1)) 
		{
			return null;
		}
		
		// We have a valid spawn position on the nav mesh.
		spawnPosition = hit.position;
		
		// Check if this position is visible on the screen, if it is we return and try again.
		if (mainCamera != null)
		{
			Vector3 screenPos = mainCamera.WorldToScreenPoint(spawnPosition);
			if ((screenPos.x > -bufferDistance && screenPos.x < (Screen.width + bufferDistance)) && 
			    (screenPos.y > -bufferDistance && screenPos.y < (Screen.height + bufferDistance))) 
			{
				return null;
			}
		}
		
		return Instantiate(enemy, spawnPosition, Quaternion.identity) as GameObject;
	}

	private void DecreaseEnemiesAlive()
	{
		enemiesAlive--;
	}
}
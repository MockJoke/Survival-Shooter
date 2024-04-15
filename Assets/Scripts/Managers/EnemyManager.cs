using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;       // Reference to the player's health
    [SerializeField] private GameObject enemy;                // The enemy prefab to be spawned
    [SerializeField] private float spawnTime = 3f;            // How long between each spawn
    [SerializeField] private Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from
    private float timer = 0;
    
    void Start()
    {
        timer = spawnTime;
        
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time
        // InvokeRepeating(nameof(Spawn), spawnTime, spawnTime);
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Spawn();
            timer = spawnTime;
        }
    }

    void Spawn()
    {
        // If the player has no health left...
        if(playerHealth.currentHealth <= 0f)
        {
            // ... exit the function
            return;
        }

        // Find a random index between zero and one less than the number of spawn points
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation
        Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
}
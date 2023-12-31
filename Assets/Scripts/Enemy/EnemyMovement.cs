﻿using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform player;                              // Reference to the player's position
    [SerializeField] private PlayerHealth playerHealth;                     // Reference to the player's health
    [SerializeField] private EnemyHealth enemyHealth;                       // Reference to this enemy's health
    [SerializeField] private UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent
    
    void Awake()
    {
        // Set up the references
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
        // If the enemy and the player have health left...
        if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
        {
            // ... set the destination of the nav mesh agent to the player
            nav.SetDestination(player.position);
        }
        // Otherwise...
        else
        {
            // ... disable the nav mesh agent
            nav.enabled = false;
        }
    }
}
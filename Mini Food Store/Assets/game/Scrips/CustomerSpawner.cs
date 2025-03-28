using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer Prefabs")]
    [Tooltip("List of customer prefabs to spawn randomly")]
    public List<GameObject> customerPrefabs;

    [Header("Spawn Settings")]
    [Tooltip("Minimum time between spawns (in seconds)")]
    public float minSpawnTime = 2f;
    
    [Tooltip("Maximum time between spawns (in seconds)")]
    public float maxSpawnTime = 5f;
    
    [Tooltip("Minimum number of customers to spawn each time")]
    public int minCustomersPerSpawn = 1;
    
    [Tooltip("Maximum number of customers to spawn each time")]
    public int maxCustomersPerSpawn = 3;

    [Header("Spawn Area")]
    [Tooltip("The area where customers will spawn (X position)")]
    public float spawnAreaMinX = -5f;
    public float spawnAreaMaxX = 5f;
    
    [Tooltip("The area where customers will spawn (Z position)")]
    public float spawnAreaMinZ = -5f;
    public float spawnAreaMaxZ = 5f;
    
    [Tooltip("The Y position for spawning customers")]
    public float spawnY = 0f;

    private bool isSpawning = false;

    void Start()
    {
        // Check if we have customer prefabs
        if (customerPrefabs.Count == 0)
        {
            Debug.LogError("No customer prefabs assigned! Please assign prefabs in the inspector.");
            return;
        }

        // Start spawning customers
        StartCoroutine(SpawnCustomers());
    }

    IEnumerator SpawnCustomers()
    {
        isSpawning = true;

        while (isSpawning)
        {
            // Wait for a random time before spawning
            float spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(spawnDelay);

            // Determine random number of customers to spawn this time
            int numberOfCustomersToSpawn = Random.Range(minCustomersPerSpawn, maxCustomersPerSpawn + 1);

            // Spawn the random number of customers
            for (int i = 0; i < numberOfCustomersToSpawn; i++)
            {
                SpawnRandomCustomer();
                
                // Small delay between individual spawns
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    void SpawnRandomCustomer()
    {
        // Get a random customer prefab from the list
        int randomIndex = Random.Range(0, customerPrefabs.Count);
        GameObject customerPrefab = customerPrefabs[randomIndex];

        // Generate a random position within the spawn area
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnAreaMinX, spawnAreaMaxX),
            spawnY,
            Random.Range(spawnAreaMinZ, spawnAreaMaxZ)
        );

        // Spawn the customer
        GameObject newCustomer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
        
        // You can add additional setup for the new customer here
        // For example, assign behaviors, initialize stats, etc.
    }

    // Call this function to stop spawning
    public void StopSpawning()
    {
        isSpawning = false;
    }

    // Call this function to resume spawning
    public void ResumeSpawning()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnCustomers());
        }
    }

    // Optional: Visualization of the spawn area in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(
            (spawnAreaMinX + spawnAreaMaxX) / 2f,
            spawnY,
            (spawnAreaMinZ + spawnAreaMaxZ) / 2f
        );
        Vector3 size = new Vector3(
            spawnAreaMaxX - spawnAreaMinX,
            0.1f,
            spawnAreaMaxZ - spawnAreaMinZ
        );
        Gizmos.DrawWireCube(center, size);
    }
}
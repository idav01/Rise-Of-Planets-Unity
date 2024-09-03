using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 5;
    public float timeBetweenWaves = 5f;
    public int totalWaves = 4;
    public float spawnDistanceFromPlayer = 20f; // Adjust this value based on your game

    private int enemiesRemaining;
    private int currentWave = 0;
    private int intelGathered = 0;
    private int requiredIntel = 1;
    private GameObject playerSpaceship;

    void Start()
    {
        playerSpaceship = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < totalWaves)
        {
            currentWave++;
            enemiesRemaining = enemiesPerWave;

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(0.5f); // Slight delay between enemy spawns
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Select a random spawn point within the array
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPoint = spawnPoints[spawnIndex].position;

        // Ensure the spawn point is within a reasonable distance from the player
        Vector3 directionFromPlayer = (spawnPoint - playerSpaceship.transform.position).normalized;
        Vector3 spawnPosition = playerSpaceship.transform.position + directionFromPlayer * spawnDistanceFromPlayer;

        // Ensure the spawn position is within the bounds of the spawn point
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, spawnPoints[spawnIndex].position.x - 10f, spawnPoints[spawnIndex].position.x + 10f);
        spawnPosition.y = Mathf.Clamp(spawnPosition.y, spawnPoints[spawnIndex].position.y - 10f, spawnPoints[spawnIndex].position.y + 10f);
        spawnPosition.z = 0;

        return spawnPosition;
    }

    public void EnemyDestroyed()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0 && currentWave >= totalWaves)
        {
            CheckMissionComplete();
        }
    }

    public void GatherIntel()
    {
        intelGathered++;
        Debug.Log("Intel gathered: " + intelGathered);
        if (intelGathered >= requiredIntel)
        {
            CheckMissionComplete();
        }
    }

    void CheckMissionComplete()
    {
        if (intelGathered >= requiredIntel)
        {
            // Mission complete, return to main scene
            Debug.Log("Mission complete, returning to main scene.");
            SceneManager.LoadScene("MainScene");
        }
    }
}

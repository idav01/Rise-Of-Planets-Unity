using UnityEngine;

public class EnemyClickHandler : MonoBehaviour
{
    private ShipStats enemyStats;
    private LaserBeam laserBeam;
    private Transform playerTransform;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        enemyStats = GetComponent<ShipStats>();
        if (enemyStats == null)
        {
            Debug.LogError("ShipStats component not found on enemy!");
        }

        playerTransform = FindObjectOfType<PlayerMovement>().transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player object not found!");
        }

        laserBeam = playerTransform.GetComponentInChildren<LaserBeam>();
        if (laserBeam == null)
        {
            Debug.LogError("LaserBeam component not found on player!");
        }
    }

    public void HandleClick(float playerPower)
    {
        if (enemyStats != null && gameObject != null) // Ensure the enemy is not null
        {
            // Ensure components are initialized
            if (laserBeam == null || playerTransform == null)
            {
                InitializeComponents();
            }

            // Trigger the laser beam effect
            if (laserBeam != null && playerTransform != null)
            {
                Vector3 startPosition = playerTransform.position;
                Vector3 endPosition = transform.position;
                Debug.Log("Firing laser from: " + startPosition + " to: " + endPosition);
                laserBeam.FireLaser(startPosition, endPosition);
                Debug.Log("Laser fired from player to enemy.");
            }

            // Deal damage to the enemy
            enemyStats.TakeDamage(playerPower);
        }
        else
        {
            Debug.LogError("Enemy ShipStats component is null or the enemy has been destroyed.");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionController : MonoBehaviour
{
    public TextMeshProUGUI narrativeText;
    public Button startMissionButton;
    public GameObject playerSpaceship;
    public GameObject enemyPrefab;
    public GameObject[] enemyAI;
    public GameObject damageDialogPrefab;

    void Start()
    {
        narrativeText.text = "Welcome to Mission 1. Your objective is to lock onto the enemy ships and gather information.";
        startMissionButton.onClick.AddListener(StartMission);

        // Initially hide the player spaceship and enemy AI
        playerSpaceship.SetActive(false);
        foreach (GameObject enemy in enemyAI)
        {
            enemy.SetActive(false);
        }

        // Initialize player and enemies with ShipStats
        InitializeShipStats(playerSpaceship, 2000, 500);
        foreach (GameObject enemy in enemyAI)
        {
            InitializeEnemyStatsFromPrefab(enemy);
        }

        Debug.Log("MissionController Start - Setup Complete");
    }

    void StartMission()
    {
        narrativeText.gameObject.SetActive(false);
        startMissionButton.gameObject.SetActive(false);
        playerSpaceship.SetActive(true);

        foreach (GameObject enemy in enemyAI)
        {
            enemy.SetActive(true);
        }

        Debug.Log("Mission Started - Player spaceship and enemies activated");
    }

    void InitializeShipStats(GameObject ship, float health, float power)
    {
        ShipStats stats = ship.GetComponent<ShipStats>();
        if (stats != null)
        {
            stats.health = health;
            stats.power = power;
            stats.damageDialogPrefab = damageDialogPrefab;
        }
    }

    void InitializeEnemyStatsFromPrefab(GameObject enemy)
    {
        ShipStats enemyPrefabStats = enemyPrefab.GetComponent<ShipStats>();
        ShipStats enemyStats = enemy.GetComponent<ShipStats>();
        if (enemyPrefabStats != null && enemyStats != null)
        {
            enemyStats.health = enemyPrefabStats.health;
            enemyStats.power = enemyPrefabStats.power;
            enemyStats.damageDialogPrefab = damageDialogPrefab;
        }
    }
}

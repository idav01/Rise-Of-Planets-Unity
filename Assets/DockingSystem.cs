using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DockingSystem : MonoBehaviour
{
    public Transform dockingPoint;
    public float dockingDuration = 5f;
    public float dockingRange = 5f; // The range within which docking can occur
    public float proximityDistance = 2f; // The distance to maintain from the docking point
    public TextMeshProUGUI dockingTimerText; // UI Text to display the docking timer

    private bool isDocking = false;
    private float remainingDockingTime;
    private bool isPlayerInRange = false;
    private GameObject playerShip;

    void Start()
    {
        remainingDockingTime = dockingDuration;
        dockingTimerText.gameObject.SetActive(false); // Hide the timer text initially
    }

    void Update()
    {
        playerShip = GameObject.FindGameObjectWithTag("Player");
        if (playerShip != null)
        {
            float distance = Vector3.Distance(playerShip.transform.position, dockingPoint.position);
            Debug.Log($"Distance to docking point: {distance}");
            if (distance <= dockingRange)
            {
                if (!isPlayerInRange)
                {
                    isPlayerInRange = true;
                    dockingTimerText.gameObject.SetActive(true);
                    StartCoroutine(Dock(playerShip));
                }
            }
            else
            {
                if (isPlayerInRange)
                {
                    isPlayerInRange = false;
                    StopCoroutine(Dock(playerShip));
                    dockingTimerText.text = "Docking paused. Return to range.";
                    Debug.Log("Player left docking range, docking paused.");
                }
            }
        }
    }

    IEnumerator Dock(GameObject playerShip)
    {
        isDocking = true;
        Debug.Log("Docking initiated.");
        dockingTimerText.text = "Breached Enemy Data\n" + remainingDockingTime.ToString("F1") + "s remaining";

        // Move player to proximity distance from docking point
        Vector3 direction = (dockingPoint.position - playerShip.transform.position).normalized;
        Vector3 dockingPosition = dockingPoint.position - direction * proximityDistance;
        playerShip.transform.position = dockingPosition;

        while (remainingDockingTime > 0 && isPlayerInRange)
        {
            remainingDockingTime -= Time.deltaTime;
            dockingTimerText.text = "Breached Enemy Data\n" + remainingDockingTime.ToString("F1") + "s remaining";
            yield return null;
        }

        if (remainingDockingTime <= 0)
        {
            isDocking = false;
            dockingTimerText.text = "Docking complete. Intel gathered.";
            Debug.Log("Docking complete, intel gathered.");

            // Inform the spawner that intel has been gathered
            EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
            if (spawner != null)
            {
                spawner.GatherIntel();
            }
            else
            {
                Debug.LogError("EnemySpawner not found in the scene.");
            }

            yield return new WaitForSeconds(2f); // Display the complete message for 2 seconds
            dockingTimerText.gameObject.SetActive(false);
            remainingDockingTime = dockingDuration; // Reset the timer for the next docking
        }
    }
}

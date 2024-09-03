using UnityEngine;

public class ShipStats : MonoBehaviour
{
    public float health = 1000f;
    public float power = 250f;
    public GameObject damageDialogPrefab; // Prefab for displaying damage dialog
    public GameObject explosionPrefab; // Prefab for the explosion effect

    void Start()
    {
        Debug.Log($"Initialized {gameObject.name} with Health: {health}, Power: {power}");
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Enemy took {damage} damage");
        Debug.Log($"Enemy health before damage: {health}");
        health -= damage;
        DisplayDamageDialog(damage);
        if (health <= 0 && gameObject != null)
        {
            Explode();
            Destroy(gameObject);
            Debug.Log("Enemy destroyed");
        }
        else
        {
            Debug.Log($"Enemy health after damage: {health}");
        }
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Debug.Log("Explosion prefab is set, instantiating explosion.");
            // Instantiate the explosion at the ship's position
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            // Destroy the explosion after its duration
            float explosionDuration = explosion.GetComponent<ParticleSystem>().main.duration;
            Destroy(explosion, explosionDuration);
            Debug.Log("Explosion instantiated and will be destroyed after " + explosionDuration + " seconds.");
        }
        else
        {
            Debug.LogError("Explosion prefab is not set.");
        }
    }

    public void DealDamage(ShipStats target)
    {
        target.TakeDamage(power);
    }

    void DisplayDamageDialog(float damage)
    {
        // Create a dialog to display the damage taken
        GameObject dialog = Instantiate(damageDialogPrefab, transform.position, Quaternion.identity);
        dialog.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Enemy took " + damage + " damage";
        Destroy(dialog, 2f); // Destroy the dialog after 2 seconds
    }
}

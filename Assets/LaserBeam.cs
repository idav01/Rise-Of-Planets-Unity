using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour
{
    public float laserDuration = 0.2f; // Duration the laser is visible
    public float laserWidth = 0.1f; // Width of the laser beam
    public LayerMask enemyLayerMask; // Layer mask to detect enemies
    public Color laserColor = Color.red; // Color of the laser

    private LineRenderer lineRenderer;
    private bool isFiring = false;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.sortingOrder = 5; // Ensure the line is rendered in front

        // Set the material and color of the laser beam
        lineRenderer.material = new Material(Shader.Find("Unlit/Glow"));

        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;

        lineRenderer.enabled = false; // Start with the laser beam disabled
    }

    public void FireLaser(Vector3 startPosition, Vector3 endPosition)
    {
        Debug.Log("FireLaser called with start: " + startPosition + " and end: " + endPosition);
        if (!isFiring)
        {
            StartCoroutine(FireLaserRoutine(startPosition, endPosition));
        }
    }

    private IEnumerator FireLaserRoutine(Vector3 startPosition, Vector3 endPosition)
    {
        Debug.Log("Starting laser routine...");
        isFiring = true;
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        Debug.Log("LineRenderer enabled and positions set: Start - " + startPosition + ", End - " + endPosition);

        Debug.DrawLine(startPosition, endPosition, Color.green, laserDuration); // Visual debug line

        yield return new WaitForSeconds(laserDuration);

        lineRenderer.enabled = false;
        isFiring = false;
        Debug.Log("Laser routine finished.");
    }
}

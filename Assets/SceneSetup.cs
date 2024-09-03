using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject background;
    public GameObject[] objectsToAdjust;
    public float backgroundZPosition = 3000f; // Desired Z position for the background
    public float objectZPosition = -100f; // Desired Z position for 3D objects
    public float nearClip = 0.1f;
    public float farClip = 5000f;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Adjust camera clipping planes
        mainCamera.nearClipPlane = nearClip;
        mainCamera.farClipPlane = farClip;

        Debug.Log("Camera clipping planes set to: Near = " + nearClip + ", Far = " + farClip);

        // Move and scale the background
        if (background != null)
        {
            Debug.Log("Original background position: " + background.transform.position);
            background.transform.position = new Vector3(background.transform.position.x, background.transform.position.y, backgroundZPosition);
            Debug.Log("New background position: " + background.transform.position);

            float backgroundScaleFactor = backgroundZPosition / background.transform.position.z;
            background.transform.localScale = new Vector3(background.transform.localScale.x * backgroundScaleFactor, background.transform.localScale.y * backgroundScaleFactor, background.transform.localScale.z);
            Debug.Log("Background scale adjusted with factor: " + backgroundScaleFactor);
        }
        else
        {
            Debug.LogError("Background GameObject is not assigned.");
        }

        // Adjust the position of 3D objects
        if (objectsToAdjust != null && objectsToAdjust.Length > 0)
        {
            foreach (GameObject obj in objectsToAdjust)
            {
                if (obj != null)
                {
                    Debug.Log("Original position of " + obj.name + ": " + obj.transform.position);
                    obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, objectZPosition);
                    Debug.Log("New position of " + obj.name + ": " + obj.transform.position);
                }
                else
                {
                    Debug.LogError("An object in objectsToAdjust array is not assigned.");
                }
            }
        }
        else
        {
            Debug.LogError("Objects to adjust array is empty or not assigned.");
        }

        Debug.Log("Scene setup complete.");
    }
}

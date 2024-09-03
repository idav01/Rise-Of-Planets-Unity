using UnityEngine;

public class BackgroundSetup : MonoBehaviour
{
    public Camera mainCamera;
    public float distanceFromCamera = 10f;
    public Vector3 cameraAngleOffset = new Vector3(45, 0, 0); // Adjust this to match your camera's angle

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Set the position of the background
        Vector3 backgroundPosition = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
        transform.position = backgroundPosition;

        // Adjust the rotation to match the camera's rotation with offset
        transform.rotation = Quaternion.Euler(mainCamera.transform.eulerAngles + cameraAngleOffset);

        // Adjust the size of the background to cover the camera's view
        float frustumHeight = 2.0f * distanceFromCamera * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * mainCamera.aspect;

        transform.localScale = new Vector3(frustumWidth / 10.0f, 1, frustumHeight / 10.0f); // Assuming the plane's size is 10x10 units
    }
} 
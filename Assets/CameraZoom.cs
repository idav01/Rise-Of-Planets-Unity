using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera mainCamera; // The main camera
    public float zoomSpeed = 10f; // Speed of zoom
    public float minZoom = 20f; // Minimum field of view for perspective camera
    public float maxZoom = 60f; // Maximum field of view for perspective camera
    public float nearClipPlane = 0.1f; // Near clipping plane distance
    public float farClipPlane = 1000f; // Far clipping plane distance
    public float panSpeed = 0.5f; // Speed of panning
    public float panMultiplier = 2.0f; // Multiplier to increase the panning distance
    public float clickDurationThreshold = 0.2f; // Duration to distinguish between click and hold

    private Vector3 dragOrigin;
    private float clickStartTime;
    private bool isPanning = false;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        mainCamera.clearFlags = CameraClearFlags.SolidColor; // Ensure background is cleared
        mainCamera.nearClipPlane = nearClipPlane;
        mainCamera.farClipPlane = farClipPlane;
    }

    void Update()
    {
        // Zoom with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0.0f)
        {
            // For perspective camera
            mainCamera.fieldOfView -= scroll * zoomSpeed;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minZoom, maxZoom);
        }

        if (Input.GetMouseButtonDown(0))
        {
            clickStartTime = Time.time;
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            float clickDuration = Time.time - clickStartTime;

            if (clickDuration > clickDurationThreshold)
            {
                Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                Vector3 move = new Vector3(pos.x * panSpeed * panMultiplier, pos.y * panSpeed * panMultiplier, 0) * -1;

                mainCamera.transform.Translate(move, Space.Self);
                dragOrigin = Input.mousePosition;
                isPanning = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }
    }
}

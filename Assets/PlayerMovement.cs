using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject player; // The player object to move
    public Camera mainCamera; // The main camera
    public float rotationSpeed = 5f;  // Speed of rotation
    public float rotationOffset = 0f; // Offset to adjust the ship's facing direction
    public float topViewOffset = -90f; // Offset to adjust the top view orientation
    public LayerMask enemyLayerMask; // Layer mask for enemy detection
    public float playerPower = 500f; // Power for player's damage
    public float clickDurationThreshold = 0.2f; // Duration to distinguish between click and hold for panning
    public float panSpeed = 0.5f; // Speed of panning
    public float panMultiplier = 2.0f; // Multiplier to increase the panning distance

    public Transform firePoint; // Fire point where the laser is spawned

    private Vector3 targetPosition;
    private bool isMoving = false;
    private float playerInitialZ; // Store the initial Z position of the player
    private Quaternion initialRotation; // Store the initial rotation of the player
    private float clickStartTime;
    private bool isPanning = false;
    private Vector3 dragOrigin;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player object is not assigned.");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned.");
            return;
        }

        playerInitialZ = player.transform.position.z; // Store the initial Z position of the player
        initialRotation = player.transform.rotation; // Store the initial rotation of the player
        targetPosition = player.transform.position; // Initialize target position
        Debug.Log("Initial target position: " + targetPosition);

        // Add LaserBeam component to the fire point
        if (firePoint != null)
        {
            LaserBeam laserBeam = firePoint.gameObject.AddComponent<LaserBeam>();
            laserBeam.enemyLayerMask = enemyLayerMask;
        }
    }

    void Update()
    {
        // Check for panning
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
            float clickDuration = Time.time - clickStartTime;

            if (!isPanning && clickDuration < clickDurationThreshold)
            {
                if (!IsPointerOverUIObject())
                {
                    Vector3 screenPosition = Input.mousePosition;
                    Debug.Log("Screen position: " + screenPosition);
                    Ray ray = mainCamera.ScreenPointToRay(screenPosition);

                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, enemyLayerMask))
                    {
                        // Clicked on an enemy
                        hit.collider.GetComponent<EnemyClickHandler>()?.HandleClick(playerPower);
                    }
                    else
                    {
                        // Clicked on the ground
                        Vector3 worldPosition = GetWorldPosition(screenPosition);
                        Debug.Log("World position from mouse: " + worldPosition);
                        if (worldPosition != Vector3.zero)
                        {
                            SetTargetPosition(worldPosition);
                        }
                    }
                }
                else
                {
                    Debug.Log("Click is on a UI element.");
                }
            }
            isPanning = false;
        }

        if (isMoving)
        {
            MoveToPosition();
            RotateTowardsTarget();
        }
    }

    Vector3 GetWorldPosition(Vector3 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane xyPlane = new Plane(Vector3.forward, new Vector3(0, 0, playerInitialZ)); // Plane parallel to XY plane at player's initial Z level
        if (xyPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            worldPosition.z = playerInitialZ; // Keep the player's Z position
            return worldPosition;
        }
        return Vector3.zero;
    }

    void SetTargetPosition(Vector3 position)
    {
        targetPosition = new Vector3(position.x, position.y, playerInitialZ); // Keep the same Z level as the player
        Debug.Log("Target position set to: " + targetPosition);
        isMoving = true; // Start moving towards the target position
    }

    void MoveToPosition()
    {
        Vector3 currentPosition = player.transform.position;
        Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);
        player.transform.position = newPosition;
        Debug.Log("Moving player to: " + player.transform.position);

        // Check if the player has reached the target position
        if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f) // Adjust the threshold to be more precise
        {
            isMoving = false; // Stop moving
            player.transform.position = targetPosition; // Snap to the exact target position
            Debug.Log("Reached target position");
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = (targetPosition - player.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + topViewOffset + rotationOffset;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle); // Adjust the rotation for top view
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

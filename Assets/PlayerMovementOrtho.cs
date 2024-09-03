using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementOrtho : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject playerHolder; // The parent object holding the player ship
    public GameObject player; // The player ship model
    public Camera mainCamera; // The main camera
    public float rotationSpeed = 5f;  // Speed of rotation
    public float rotationOffset = 0f; // Offset to adjust the ship's facing direction
    public float topViewOffset = -90f; // Offset to adjust the top view orientation
    public LayerMask enemyLayerMask; // Layer mask for enemy detection
    public float playerPower = 500f; // Power for player's damage

    private Vector3 targetPosition;
    private bool isMoving = false;
    private float playerInitialZ; // Store the initial Z position of the playerHolder
    private Quaternion initialRotation; // Store the initial rotation of the playerHolder

    void Start()
    {
        if (playerHolder == null)
        {
            Debug.LogError("PlayerHolder object is not assigned.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player ship object is not assigned.");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned.");
            return;
        }

        playerInitialZ = playerHolder.transform.position.z; // Store the initial Z position of the playerHolder
        initialRotation = playerHolder.transform.rotation; // Store the initial rotation of the playerHolder
        targetPosition = playerHolder.transform.position; // Initialize target position
        Debug.Log("Initial target position: " + targetPosition);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

        if (isMoving)
        {
            MoveToPosition();
            RotateTowardsTarget();
        }
    }

    Vector3 GetWorldPosition(Vector3 screenPosition)
    {
        if (mainCamera.orthographic)
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -mainCamera.transform.position.z));
            worldPosition.z = playerInitialZ; // Keep the player's Z position
            return worldPosition;
        }
        else
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
    }

    void SetTargetPosition(Vector3 position)
    {
        targetPosition = new Vector3(position.x, position.y, playerInitialZ); // Keep the same Z level as the player
        Debug.Log("Target position set to: " + targetPosition);
        isMoving = true; // Start moving towards the target position
    }

    void MoveToPosition()
    {
        Vector3 currentPosition = playerHolder.transform.position;
        Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);
        playerHolder.transform.position = newPosition;
        Debug.Log("Moving player to: " + playerHolder.transform.position);

        // Check if the player has reached the target position
        if (Vector3.Distance(playerHolder.transform.position, targetPosition) < 0.01f) // Adjust the threshold to be more precise
        {
            isMoving = false; // Stop moving
            playerHolder.transform.position = targetPosition; // Snap to the exact target position
            Debug.Log("Reached target position");
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = (targetPosition - playerHolder.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
            Quaternion targetRotation = Quaternion.Euler(topViewOffset, 0, -targetAngle); // Adjust the rotation for top view and invert the angle for correct orientation
            playerHolder.transform.rotation = Quaternion.Slerp(playerHolder.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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

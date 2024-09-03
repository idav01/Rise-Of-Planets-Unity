using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position; // Initialize target position
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    LockOnToEnemy(hit.transform.gameObject);
                }
                else
                {
                    SetTargetPosition(hit.point);
                }
            }
        }

        if (isMoving)
        {
            MoveToPosition();
        }
    }

    void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        isMoving = true; // Start moving towards the target position
    }

    void MoveToPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // Check if the player has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false; // Stop moving
        }
    }

    void LockOnToEnemy(GameObject enemy)
    {
        Debug.Log("Locked on to enemy: " + enemy.name);
        // Show dialog or additional interaction here
    }
}

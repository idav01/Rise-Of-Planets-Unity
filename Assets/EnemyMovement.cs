using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Vector2 xBounds = new Vector2(-10f, 10f);
    public Vector2 yBounds = new Vector2(-10f, 10f);
    public float followDistance = 15f;

    private Vector3 moveDirection;
    private GameObject player;
    private bool isFollowingPlayer = false;
    private float enemyInitialZ;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyInitialZ = transform.position.z; // Store the initial Z position of the enemy
        SetRandomDirection();
    }

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= followDistance)
        {
            moveDirection = (player.transform.position - transform.position).normalized;
            isFollowingPlayer = true;
        }
        else
        {
            isFollowingPlayer = false;
        }

        MoveEnemy();
        CheckBounds();
    }

    void SetRandomDirection()
    {
        if (!isFollowingPlayer)
        {
            moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        }
    }

    void MoveEnemy()
    {
        Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        newPosition.z = enemyInitialZ; // Keep enemy on the same 2D plane
        transform.position = newPosition;
    }

    void CheckBounds()
    {
        bool changedDirection = false;

        if (transform.position.x < xBounds.x || transform.position.x > xBounds.y)
        {
            moveDirection.x = -moveDirection.x;
            changedDirection = true;
        }

        if (transform.position.y < yBounds.x || transform.position.y > yBounds.y)
        {
            moveDirection.y = -moveDirection.y;
            changedDirection = true;
        }

        if (changedDirection && !isFollowingPlayer)
        {
            SetRandomDirection();
        }

        float clampedX = Mathf.Clamp(transform.position.x, xBounds.x, xBounds.y);
        float clampedY = Mathf.Clamp(transform.position.y, yBounds.x, yBounds.y);
        transform.position = new Vector3(clampedX, clampedY, enemyInitialZ);
    }
}

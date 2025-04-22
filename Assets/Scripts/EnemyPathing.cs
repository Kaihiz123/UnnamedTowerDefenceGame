using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] private GameObject sparkEffectPrefab;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float moveSpeed;

    [Header("References")]
    [SerializeField] private Transform spriteTransform; // Reference to child sprite

    // Simple bool to check if path is complete
    public bool HasReachedEnd { get; private set; } = false;

    public void Initialize(float speed, GameObject waypointsParent)
    {
        moveSpeed = speed;
        currentWaypointIndex = 1;
        HasReachedEnd = false;
        
        if (waypointsParent != null)
        {
            // Get all child transforms as waypoints
            int childCount = waypointsParent.transform.childCount;
            waypoints = new Transform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                waypoints[i] = waypointsParent.transform.GetChild(i);
            }
        }
        else
        {
            Debug.LogWarning("Waypoints parent not found: " + waypointsParent.name);
            return;
        }

        RotateSprite();
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0 || HasReachedEnd)
            return;

        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        float enemyDistanceFromPreviousTarget = Vector2.Distance(transform.position, waypoints[currentWaypointIndex - 1].position);
        float distanceBetweenPreviousAndCurrentTarget = Vector2.Distance(waypoints[currentWaypointIndex - 1].position, waypoints[currentWaypointIndex].position);
        if (enemyDistanceFromPreviousTarget >= distanceBetweenPreviousAndCurrentTarget - 0.02f)
        {
            // Check if this was the last waypoint
            if (currentWaypointIndex == waypoints.Length - 1)
            {
                // We've reached the final waypoint
                HasReachedEnd = true;
                return;
            }

            // Move to next waypoint
            currentWaypointIndex++;

            RotateSprite();
        }
    }

    // Rotate sprite based on movement direction
    private void RotateSprite()
    {
        Vector3 moveDirection = (waypoints[currentWaypointIndex].position - waypoints[currentWaypointIndex - 1].position).normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg; //hyi vittu
        spriteTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); //-90 if facing up by default
    }
}
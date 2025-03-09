using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] private string waypointsParentName = "Waypoints";
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float moveSpeed;
    
    // Simple bool to check if path is complete
    public bool HasReachedEnd { get; private set; } = false;

    void Awake()
    {
        // Find waypoints parent by name
        GameObject waypointsParent = GameObject.Find(waypointsParentName);
        
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
            Debug.LogWarning("Waypoints parent not found: " + waypointsParentName);
        }
    }

    public void Initialize(float speed)
    {
        moveSpeed = speed;
        currentWaypointIndex = 0;
        HasReachedEnd = false;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || HasReachedEnd)
            return;

        Vector2 targetPosition = waypoints[currentWaypointIndex].position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
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
        }
    }
}
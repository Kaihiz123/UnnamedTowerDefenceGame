using UnityEngine;

public class DrawPath : MonoBehaviour
{
    [SerializeField] private string waypointsParentName = "Waypoints";
    [SerializeField] private Material pathMaterial;    
    private Transform[] waypoints;
    private LineRenderer lineRenderer;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        
        lineRenderer.material = pathMaterial;
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        
        Transform waypointsParent = GameObject.Find(waypointsParentName)?.transform;
        if (waypointsParent != null)
        {
            int waypointCount = waypointsParent.childCount;
            waypoints = new Transform[waypointCount];
            
            for (int i = 0; i < waypointCount; i++)
            {
                waypoints[i] = waypointsParent.GetChild(i);
            }

            lineRenderer.positionCount = waypointCount;
            for (int i = 0; i < waypointCount; i++)
            {
                lineRenderer.SetPosition(i, waypoints[i].position);
            }
        }
        else
        {
            Debug.LogWarning($"Waypoints parent '{waypointsParentName}' not found!");
        }
    }
}
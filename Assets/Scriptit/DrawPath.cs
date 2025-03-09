using UnityEngine;
using UnityEngine.U2D;

// WIP DO NOT USE
public class DrawPath : MonoBehaviour
{
    [SerializeField] private string waypointsParentName = "Waypoints";
    private Transform[] waypoints;
    private SpriteShapeController spriteShape;

    void Awake()
    {
        GameObject waypointsParent = GameObject.Find(waypointsParentName);
        
        if (waypointsParent != null)
        {
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

    void Start()
    {
        spriteShape = GetComponent<SpriteShapeController>();
        DrawWaypointPath();        
    }

    void DrawWaypointPath()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Spline spline = spriteShape.spline;
        spline.Clear();

        for (int i = 0; i < waypoints.Length; i++)
        {
            spline.InsertPointAt(i, waypoints[i].position);
        }
    }
}
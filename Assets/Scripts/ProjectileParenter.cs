using UnityEngine;

public class ProjectileParenter : MonoBehaviour
{
    private static GameObject _projectilesParentInstance;
    
    public static GameObject ProjectilesParent
    {
        get
        {
            if (_projectilesParentInstance == null)
            {
                _projectilesParentInstance = GameObject.Find("ProjectilesParent");
            }
            return _projectilesParentInstance;
        }
    }

    void Start()
    {
        if (ProjectilesParent != null)
        {
            transform.SetParent(ProjectilesParent.transform);
        }
    }
}

using UnityEngine;

public class TowerPlacementGrid : MonoBehaviour
{
    // This scripts idea is to be able to place tower on a grid

    public Vector2 GridSize;
    
    public Vector2 ElementSize; // if Towers and Ghosts localScale == ElementSize everything fits nicely

    public GameObject Ghost;

    Camera mainCamera;
    public LayerMask ObjectSelectLayerMask;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    GameObject selectedGameObject;

    public float distance = 1f;
    public float minDepth = -1f;

    private void Update()
    {

        if (Input.GetMouseButtonDown(0)) // left mouse button pressed down
        {
            // mouse position converted to world space
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // raycast to check if a tower is beneath the cursor
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, distance, ObjectSelectLayerMask, minDepth);

            if (hit.collider != null)
            {
                // raycast hit a tower
                selectedGameObject = hit.collider.gameObject;

                // snap ghost to closest grid element
                float snapX, snapY;
                if (GridSize.x % 2 == 0)
                {
                    snapX = Mathf.Round((mousePosition.x - ElementSize.x / 2f) / ElementSize.x) * ElementSize.x + ElementSize.x / 2f;
                }
                else
                {
                    snapX = Mathf.Round((mousePosition.x - ElementSize.x) / ElementSize.x) * ElementSize.x + ElementSize.x;
                }
                if (GridSize.y % 2 == 0)
                {
                    snapY = Mathf.Round((mousePosition.y - ElementSize.y / 2f) / ElementSize.y) * ElementSize.y + ElementSize.y / 2f;
                }
                else
                {
                    snapY = Mathf.Round((mousePosition.y - ElementSize.y) / ElementSize.y) * ElementSize.y + ElementSize.y;
                }
                Ghost.transform.position = new Vector3(snapX, snapY, 0f);

                // show ghost
                Ghost.SetActive(true);
            }
            else
            {
                // raycast didn't hit any tower
                selectedGameObject = null;
            }
        }
        else if (Input.GetMouseButton(0)) // left mouse button held down
        {
            // if we have selected a tower previously
            if(selectedGameObject != null)
            {
                // cursor position to world space
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;

                // drag the selected tower with the cursor
                selectedGameObject.transform.position = mousePosition;

                // Snap mouse position to the grid
                float snapX, snapY;
                if (GridSize.x % 2 == 0)
                {
                    snapX = Mathf.Round((mousePosition.x - ElementSize.x / 2f) / ElementSize.x) * ElementSize.x + ElementSize.x / 2f;
                }
                else
                {
                    snapX = Mathf.Round((mousePosition.x - ElementSize.x) / ElementSize.x) * ElementSize.x + ElementSize.x;
                }
                if(GridSize.y % 2 == 0)
                {
                    snapY = Mathf.Round((mousePosition.y - ElementSize.y / 2f) / ElementSize.y) * ElementSize.y + ElementSize.y / 2f;
                }
                else
                {
                    snapY = Mathf.Round((mousePosition.y - ElementSize.y) / ElementSize.y) * ElementSize.y + ElementSize.y;
                }

                // move ghost to snapped position
                Ghost.transform.position = new Vector3(snapX, snapY, 0f);
            }
        }
        else if (Input.GetMouseButtonUp(0)) // left mouse button released
        {
            if(selectedGameObject != null)
            {
                // mouse pos to World space
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;

                // snap selected tower to closest grid element
                float snapX, snapY;
                if (GridSize.x % 2 == 0)
                {
                    snapX = Mathf.Round((mousePosition.x - ElementSize.x / 2f) / ElementSize.x) * ElementSize.x + ElementSize.x / 2f;
                }
                else
                {
                    snapX = Mathf.Round((mousePosition.x - ElementSize.x) / ElementSize.x) * ElementSize.x + ElementSize.x;
                }
                if (GridSize.y % 2 == 0)
                {
                    snapY = Mathf.Round((mousePosition.y - ElementSize.y / 2f) / ElementSize.y) * ElementSize.y + ElementSize.y / 2f;
                }
                else
                {
                    snapY = Mathf.Round((mousePosition.y - ElementSize.y) / ElementSize.y) * ElementSize.y + ElementSize.y;
                }
                selectedGameObject.transform.position = new Vector3(snapX, snapY, 0f);
                selectedGameObject = null;

                // hide ghost
                Ghost.SetActive(false);
            }
        }


    }





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for(int i = 1; i < GridSize.x; i++)
        {
            Gizmos.DrawLine(new Vector3(i * ElementSize.x - GridSize.x * 0.5f * ElementSize.x, GridSize.y * -0.5f * ElementSize.y, 0f), new Vector3(i * ElementSize.x - GridSize.x * 0.5f * ElementSize.x, GridSize.y * ElementSize.y + GridSize.y * -0.5f * ElementSize.y, 0f));
        }
        
        for (int i = 1; i < GridSize.y; i++)
        {
            Gizmos.DrawLine(new Vector3(GridSize.x * -0.5f * ElementSize.x, i * ElementSize.y - GridSize.y * 0.5f * ElementSize.y, 0f), new Vector3(GridSize.x * ElementSize.x + GridSize.x * -0.5f * ElementSize.x, i * ElementSize.y - GridSize.y * 0.5f * ElementSize.y, 0f));
        }
       
    }

}

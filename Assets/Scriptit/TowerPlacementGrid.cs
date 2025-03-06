using System.Collections.Generic;
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
    Vector3 selectedGameObjectStart;
    Vector2 selectedGameObjectStartSnap;

    public float distance = 1f;
    public float minDepth = -1f;

    public Vector3 movement = Vector3.zero;


    private void Update()
    {

        if (Input.GetMouseButtonDown(0)) // left mouse button pressed down
        {
            // mouse position converted to world space
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // raycast to check if a tower is beneath the cursor
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, distance, ObjectSelectLayerMask, minDepth);

            if (hit.collider != null || isDragging)
            {
                // raycast hit a tower
                if (selectedGameObject == null)
                {
                    selectedGameObject = hit.collider.gameObject;
                    selectedGameObjectStart = selectedGameObject.transform.position;
                }

                // snap ghost to closest grid element
                float snapX, snapY;
                if (GridSize.x % 2 == 0)
                {
                    snapX = Mathf.Round((mousePosition.x - movement.x - ElementSize.x / 2f) / ElementSize.x) * ElementSize.x + ElementSize.x / 2f + movement.x;
                }
                else
                {
                    snapX = Mathf.Round((mousePosition.x - movement.x - ElementSize.x) / ElementSize.x) * ElementSize.x + ElementSize.x + movement.x;
                }
                if (GridSize.y % 2 == 0)
                {
                    snapY = Mathf.Round((mousePosition.y - movement.y - ElementSize.y / 2f) / ElementSize.y) * ElementSize.y + ElementSize.y / 2f + movement.y;
                }
                else
                {
                    snapY = Mathf.Round((mousePosition.y - movement.y - ElementSize.y) / ElementSize.y) * ElementSize.y + ElementSize.y + movement.y;
                }
                Ghost.transform.position = new Vector3(snapX, snapY, 0f);

                if (!isDragging)
                {
                    unavailableAreas.Remove(new Vector2(snapX - movement.x, snapY - movement.y));
                    selectedGameObjectStartSnap = new Vector2(snapX, snapY);
                }
                
                // show ghost
                Ghost.GetComponent<SpriteRenderer>().color = Color.green;
                Ghost.SetActive(true);
            }
            else
            {
                // raycast didn't hit any tower
                selectedGameObject = null;
            }
        }
        else if (Input.GetMouseButton(0) || isDragging) // left mouse button held down
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
                    snapX = Mathf.Round((mousePosition.x - movement.x - ElementSize.x / 2f) / ElementSize.x) * ElementSize.x + ElementSize.x / 2f + movement.x;
                }
                else
                {
                    snapX = Mathf.Round((mousePosition.x - movement.x - ElementSize.x) / ElementSize.x) * ElementSize.x + ElementSize.x + movement.x;
                }
                if(GridSize.y % 2 == 0)
                {
                    snapY = Mathf.Round((mousePosition.y - movement.y - ElementSize.y / 2f) / ElementSize.y) * ElementSize.y + ElementSize.y / 2f + movement.y;
                }
                else
                {
                    snapY = Mathf.Round((mousePosition.y - movement.y - ElementSize.y) / ElementSize.y) * ElementSize.y + ElementSize.y + movement.y;
                }

                // move ghost to snapped position
                Ghost.transform.position = new Vector3(snapX, snapY, 0f);

                // check if area is available
                if (isAreaAvailable(snapX - movement.x, snapY - movement.y))
                {
                    Ghost.GetComponent<SpriteRenderer>().color = Color.green;
                }
                else
                {
                    Ghost.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) || endOfDrag) // left mouse button released
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
                    snapX = Mathf.Round((mousePosition.x - movement.x - ElementSize.x / 2f) / ElementSize.x) * ElementSize.x + ElementSize.x / 2f + movement.x;
                }
                else
                {
                    snapX = Mathf.Round((mousePosition.x - movement.x - ElementSize.x) / ElementSize.x) * ElementSize.x + ElementSize.x + movement.x;
                }
                if (GridSize.y % 2 == 0)
                {
                    snapY = Mathf.Round((mousePosition.y - movement.y - ElementSize.y / 2f) / ElementSize.y) * ElementSize.y + ElementSize.y / 2f + movement.y;
                }
                else
                {
                    snapY = Mathf.Round((mousePosition.y - movement.y - ElementSize.y) / ElementSize.y) * ElementSize.y + ElementSize.y + movement.y;
                }
                selectedGameObject.transform.position = new Vector3(snapX, snapY, 0f);
                
                if(isAreaAvailable(snapX - movement.x, snapY - movement.y))
                {
                    unavailableAreas.Add(new Vector2(snapX - movement.x, snapY - movement.y));
                }
                else
                {
                    if (endOfDrag)
                    {
                        Destroy(selectedGameObject);
                    }
                    else
                    {
                        selectedGameObject.transform.position = selectedGameObjectStart;
                        unavailableAreas.Add(selectedGameObjectStartSnap - new Vector2(movement.x, movement.y));
                    }
                    
                }
                
                selectedGameObject = null;

                // hide ghost
                Ghost.SetActive(false);

                endOfDrag = false;
            }
        }
    }

    bool isDragging = false;
    bool endOfDrag = false;

    public void NewTower(GameObject go)
    {
        selectedGameObject = go;
        isDragging = true;
    }

    public void NewTowerDragEnd()
    {
        isDragging = false;
        endOfDrag = true;
    }

    public List<Vector2> unavailableAreas = new List<Vector2>();

    private bool isAreaAvailable(float snapX, float snapY)
    {
        Vector2 snap = new Vector2(snapX, snapY);
        float sqrTolerance = 0.1f * 0.1f;
        foreach (Vector2 v in unavailableAreas)
        {
            if ((snap - v).sqrMagnitude <= sqrTolerance)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for(int i = 1; i < GridSize.x; i++)
        {
            Gizmos.DrawLine(new Vector3(i * ElementSize.x - GridSize.x * 0.5f * ElementSize.x + movement.x, GridSize.y * -0.5f * ElementSize.y + movement.y, 0f), new Vector3(i * ElementSize.x - GridSize.x * 0.5f * ElementSize.x + movement.x, GridSize.y * ElementSize.y + GridSize.y * -0.5f * ElementSize.y + movement.y, 0f));
        }
        
        for (int i = 1; i < GridSize.y; i++)
        {
            Gizmos.DrawLine(new Vector3(GridSize.x * -0.5f * ElementSize.x + movement.x, i * ElementSize.y - GridSize.y * 0.5f * ElementSize.y + movement.y, 0f), new Vector3(GridSize.x * ElementSize.x + GridSize.x * -0.5f * ElementSize.x + movement.x, i * ElementSize.y - GridSize.y * 0.5f * ElementSize.y + movement.y, 0f));
        }
       
    }

}

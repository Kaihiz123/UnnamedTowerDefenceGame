using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementGrid : MonoBehaviour
{
    // This scripts idea is to be able to place tower on a grid

    public Vector2 GridSize; 
    
    public Vector2 ElementSize; // if Towers and Ghosts localScale == ElementSize everything fits nicely

    public GameObject Ghost; // the green/red box that indicates if dragged tower can be placed there

    Camera mainCamera;

    public Bank bank;

    // these are used in the raycast to check if the mouse is above an old tower on the grid
    public LayerMask ObjectSelectLayerMask;
    public float distance = 1f; 
    public float minDepth = -1f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    GameObject selectedGameObject; // currently selected tower
    Vector3 selectedGameObjectStart; // selected tower start position
    Vector2 selectedGameObjectStartSnap; // selected towers start position that is snapped to the grid

    public Vector3 movement = Vector3.zero; // how the grid has been moved

    private void Update()
    {

        if (Input.GetMouseButtonDown(0)) // left mouse button pressed down
        {
            // mouse position converted to world space
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // raycast to check if a tower is beneath the cursor
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, distance, ObjectSelectLayerMask, minDepth);

            // if raycast hit an old tower on the grid or if we are starting to dragging a new tower
            if (hit.collider != null || isDraggingANewTower)
            {
                // raycast hit a tower
                if (selectedGameObject == null)
                {
                    selectedGameObject = hit.collider.gameObject;
                    selectedGameObjectStart = selectedGameObject.transform.position;
                }

                // snap ghost to closest grid element
                GetSnapPosition(mousePosition, out float snapX, out float snapY);

                // Move ghost to snap position
                Ghost.transform.position = new Vector3(snapX, snapY, 0f);

                // if we are not dragging a new tower
                if (!isDraggingANewTower)
                {
                    // remove an area from the list so that a tower can be placed here
                    unavailableAreas.Remove(new Vector2(snapX - movement.x, snapY - movement.y));
                    // save the old snap position of the tower
                    selectedGameObjectStartSnap = new Vector2(snapX, snapY);
                }
                
                // show ghost and turn it green
                Ghost.GetComponent<SpriteRenderer>().color = Color.green;
                Ghost.SetActive(true);
            }
            else
            {
                // raycast didn't hit any tower and a new tower is not being dragged
                selectedGameObject = null;
            }
        }
        else if (Input.GetMouseButton(0) || isDraggingANewTower) // left mouse button held down
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
                GetSnapPosition(mousePosition, out float snapX, out float snapY);
                
                // move ghost to snapped position
                Ghost.transform.position = new Vector3(snapX, snapY, 0f);

                // check if area is available
                if (isAreaAvailable(snapX - movement.x, snapY - movement.y))
                {
                    // change the color of the ghost based on availability of the area
                    Ghost.GetComponent<SpriteRenderer>().color = Color.green;
                }
                else
                {
                    Ghost.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) || endOfDragOfANewTower) // left mouse button released
        {
            // if we have selected a tower previously
            if (selectedGameObject != null)
            {
                // convert mouse position to the World space
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;

                // snap selected tower to closest grid element
                GetSnapPosition(mousePosition, out float snapX, out float snapY);
                
                // check if the area is available
                if(isAreaAvailable(snapX - movement.x, snapY - movement.y))
                {
                    // move the selected tower to position
                    selectedGameObject.transform.position = new Vector3(snapX, snapY, 0f);

                    // change this are to be occupied
                    unavailableAreas.Add(new Vector2(snapX - movement.x, snapY - movement.y));

                    if (endOfDragOfANewTower)
                    {
                        // notify that the just bought tower was placed successfully to finish the transaction
                        bank.NewTowerWasPlacedSuccessfully();
                        endOfDragOfANewTower = false;
                    }
                }
                else
                {
                    // area is not available

                    if (endOfDragOfANewTower) // we were dragging a new tower so we destroy it
                    {
                        Destroy(selectedGameObject);
                        endOfDragOfANewTower = false;
                    }
                    else
                    {
                        // we move the selected tower back where it was
                        selectedGameObject.transform.position = selectedGameObjectStart;
                        unavailableAreas.Add(selectedGameObjectStartSnap - new Vector2(movement.x, movement.y));
                    }                    
                }
                
                // no tower is no longer selected
                selectedGameObject = null;

                // hide ghost
                Ghost.SetActive(false);

            }
        }
    }

    private void GetSnapPosition(Vector3 mousePosition, out float snapX, out float snapY)
    {
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
    }

    bool isDraggingANewTower = false; // if we are dragging a new tower from the store
    bool endOfDragOfANewTower = false; // we stopped dragging a new tower

    // this is called when player successfully buys a new tower from the store
    public void NewTower(GameObject go)
    {
        selectedGameObject = go;
        isDraggingANewTower = true;
    }

    // this is called when we stopped dragging a new tower
    public void NewTowerDragEnd()
    {
        isDraggingANewTower = false;
        endOfDragOfANewTower = true;
    }

    // save the placements of the towers and other areas where towers cannot be placed
    public List<Vector2> unavailableAreas = new List<Vector2>();

    // check if a tower can be placed in the element
    private bool isAreaAvailable(float snapX, float snapY)
    {
        Vector2 snap = new Vector2(snapX, snapY);
        float sqrTolerance = 0.1f * 0.1f;
        foreach (Vector2 v in unavailableAreas)
        {
            // Due to the floating number accuracy error the coordinates might shift a little so we use tolerance around the actual place.
            // Square magnitude is used instead of distance because it's more high-performant but does the same thing in this case
            if ((snap - v).sqrMagnitude <= sqrTolerance)
            {
                return false;
            }
        }

        return true;
    }

    // this is used in unity editor to show grid if gizmos are active
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

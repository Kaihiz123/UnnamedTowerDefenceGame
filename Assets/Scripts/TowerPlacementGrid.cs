using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacementGrid : MonoBehaviour
{
    // This scripts idea is to be able to place tower on a grid

    public bool debug;
    public bool hidePlaceHoldersOfTheUnavailableAreas;

    public Vector2 GridSize; 
    
    public Vector2 ElementSize; // if Towers and Ghosts localScale == ElementSize everything fits nicely

    public GameObject Ghost; // the green/red box that indicates if dragged tower can be placed there
    public GameObject SelectionIndicator; // this show which tower is selected if any

    Camera mainCamera;

    public Bank bank;
    public StoreHandler storeHandler;
    public SelectionWindow selectionWindow;
    public GameObject selectionWindowCanvasObject;

    // these are used in the raycast to check if the mouse is above an old tower on the grid
    public LayerMask ObjectSelectLayerMask;
    public float distance = 1f; 
    public float minDepth = -1f;

    // save the placements of the towers and other areas where towers cannot be placed
    public List<Vector2Int> unavailablePositions = new List<Vector2Int>();
    public Transform UnavailableAreasParent;

    bool isDraggingANewTower = false; // if we are dragging a new tower from the store
    bool endOfDragOfANewTower = false; // we stopped dragging a new tower

    GameObject selectedGameObject; // currently selected tower
    Vector3 selectedGameObjectStart; // selected tower start position
    Vector2 selectedGameObjectStartSnap; // selected towers start position that is snapped to the grid

    public Vector3 movement = Vector3.zero; // how the grid has been moved

    bool isSelecting = true; // whether we are selecting a tower or dragging a tower;
                             // user is dragging if tower is moved outside the original snap area

    private void Start()
    {
        mainCamera = Camera.main;

        Ghost.transform.localScale = ElementSize;
        SelectionIndicator.transform.localScale = ElementSize;

        // add unavailable areas to unavailablePositions
        foreach(Transform t in UnavailableAreasParent)
        {
            unavailablePositions.Add(new Vector2Int(Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y)));
        }

        // deactivate all the placeholders of the unavailable areas
        if (hidePlaceHoldersOfTheUnavailableAreas)
        {
            UnavailableAreasParent.gameObject.SetActive(false);
        }
        
    }

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
                    unavailablePositions.Remove(new Vector2Int(Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y)));
                    // save the old snap position of the tower
                    selectedGameObjectStartSnap = new Vector2(snapX, snapY);
                }
                
                // show ghost and turn it green
                Ghost.GetComponent<SpriteRenderer>().color = Color.green;
                Ghost.SetActive(true);
            }
            else if (EventSystem.current.IsPointerOverGameObject())
            {

            }
            else
            {
                // raycast didn't hit any tower and a new tower is not being dragged
                selectedGameObject = null;

                // close the selection window if it's open
                ShowSelectionWindow(false);
                // hide selection indicator
                SelectionIndicator.SetActive(false);
                
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

                if (isSelecting && !isDraggingANewTower)
                {
                    // check if we are selecting the tower or dragging
                    if (!CheckIfInSameArea(Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y), 
                        Mathf.RoundToInt(selectedGameObjectStartSnap.x - movement.x), Mathf.RoundToInt(selectedGameObjectStartSnap.y - movement.y)))
                    {
                        // we are not selecting the tower but dragging
                        isSelecting = false;
                    }

                    // hide the selection window
                    ShowSelectionWindow(false);
                    // hide selection indicator
                    SelectionIndicator.SetActive(false);
                }

                // move ghost to snapped position
                Ghost.transform.position = new Vector3(snapX, snapY, 0f);

                // check if area is available
                if (isAreaAvailable(Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y)))
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

                // check if we were selecting or dragging a new tower
                if (isSelecting)
                {
                    if (isAreaAvailable(Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y)))
                    {
                        if (endOfDragOfANewTower)
                        {
                            // notify that the just bought tower was placed successfully to finish the transaction
                            bank.NewTowerWasPlacedSuccessfully();
                            storeHandler.NewTowerEnd();
                            endOfDragOfANewTower = false;
                        }

                        // move the selected tower to position
                        selectedGameObject.transform.position = new Vector3(snapX, snapY, 0f);

                        // change this are to be occupied
                        unavailablePositions.Add(new Vector2Int(Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y)));

                        // show selection window
                        ShowSelectionWindow(true);
                        // move the selection indicator to selectedGameObjects position
                        SelectionIndicator.transform.position = selectedGameObject.transform.position;
                        // show selection indicator
                        SelectionIndicator.SetActive(true);
                    }
                    else
                    {
                        if (endOfDragOfANewTower) // we were dragging a new tower so we destroy it
                        {
                            Destroy(selectedGameObject);
                            storeHandler.NewTowerEnd();
                            endOfDragOfANewTower = false;
                        }
                    }

                    selectedGameObjectStartSnap = new Vector2(snapX, snapY);

                }
                else
                {
                    // we are dragging an old tower
                    
                    // check if the area is available
                    if (isAreaAvailable(Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y)))
                    {
                        // move the selected tower to position
                        selectedGameObject.transform.position = new Vector3(snapX, snapY, 0f);

                        // change this are to be occupied
                        unavailablePositions.Add(new Vector2Int(Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y)));
                    }
                    else
                    {
                        // area is not available

                        // we move the selected tower back where it was
                        selectedGameObject.transform.position = selectedGameObjectStart;
                        // this position is now unavailable
                        unavailablePositions.Add(new Vector2Int(Mathf.RoundToInt(selectedGameObjectStartSnap.x - movement.x), Mathf.RoundToInt(selectedGameObjectStartSnap.y - movement.y)));
                    }

                    // show the selection window
                    ShowSelectionWindow(true);
                    // move the selection indicator to selectedGameObjects position
                    SelectionIndicator.transform.position = selectedGameObject.transform.position;
                    // show selection indicator
                    SelectionIndicator.SetActive(true);
                }

                if (debug)
                {
                    DebugCheck(selectedGameObject.name);
                }
                
                // no tower is no longer selected
                selectedGameObject = null;

                // hide ghost
                Ghost.SetActive(false);

                // reset to default
                isSelecting = true;
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

    // this is called when player successfully buys a new tower from the store
    public void NewTower(GameObject go)
    {
        selectedGameObject = go;
        isDraggingANewTower = true;

        // hide selection window
        ShowSelectionWindow(false);
        // hide selection indicator
        SelectionIndicator.SetActive(false);
    }

    // this is called when we stopped dragging a new tower
    public void NewTowerDragEnd()
    {
        isDraggingANewTower = false;
        endOfDragOfANewTower = true;
    }

    

    private bool isAreaAvailable(int snapX, int snapY)
    {
        foreach (Vector2Int unavailablePosition in unavailablePositions)
        {
            if (CheckIfInSameArea(snapX, snapY, unavailablePosition.x, unavailablePosition.y))
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckIfInSameArea(int snapX, int snapY, int areaX, int areaY)
    {
        return (snapX == areaX && snapY == areaY);
    }

    private void ShowSelectionWindow(bool show)
    {
        if (show)
        {
            selectionWindow.Init(selectedGameObject.GetComponent<TowerInfo>());
            selectionWindowCanvasObject.SetActive(show);
        }
        else
        {
            selectionWindow.CloseSelectionWindow();
            selectionWindowCanvasObject.SetActive(show);
        }
    }

    public void SelectedTowerWasSold(TowerInfo towerInfo)
    {
        unavailablePositions.Remove(new Vector2Int(Mathf.RoundToInt(selectedGameObjectStartSnap.x - movement.x), Mathf.RoundToInt(selectedGameObjectStartSnap.y - movement.y)));
        Destroy(towerInfo.gameObject);
        selectedGameObject = null;
        // hide the selection window
        ShowSelectionWindow(false);
        // hide the selection indicator
        SelectionIndicator.SetActive(false);
    }

    private void DebugCheck(string selectedName)
    {
        int towerCount = transform.GetChild(0).childCount - 2;
        if(towerCount != unavailablePositions.Count)
        {
            Debug.Log("towerCount != unavailableAreaCount, selectedName=" + selectedName);
        }

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

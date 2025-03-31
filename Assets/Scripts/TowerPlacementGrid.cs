using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacementGrid : MonoBehaviour
{
    // This scripts idea is to be able to place tower on a grid

    public bool hideUnavailableAreas;
    public bool hideOfflimitsAreas;
    public bool showStoreAllTheTime;
    public bool towersCanBeMoved;

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

    List<Vector2Int> offlimitsPositions = new List<Vector2Int>();
    public Transform OffLimitsAreasParent;

    GameObject selectedGameObject; // currently selected tower
    Vector3 selectedGameObjectStart; // selected tower start position
    //Vector2 selectedGameObjectStartSnap; // selected towers start position that is snapped to the grid

    GameObject raycastedGameObject;
    Vector3 raycastedGameObjectStart;
    Vector2 raycastedGameObjectStartSnap;

    GameObject boughtGameObject;

    public Vector3 movement = Vector3.zero; // how the grid has been moved

    bool isDragging = false;
    public float cooldownTime = 1f;

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

        // add offlimits areas to offlimitsPositions
        foreach(Transform t in OffLimitsAreasParent)
        {
            offlimitsPositions.Add(new Vector2Int(Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y)));
        }

        // deactivate all the placeholders of the unavailable areas
        if (hideUnavailableAreas)
        {
            UnavailableAreasParent.gameObject.SetActive(false);
        }

        if (hideOfflimitsAreas)
        {
            OffLimitsAreasParent.gameObject.SetActive(false);
        }
        
    }

    Vector2 mouseStartPos;

    private void Update()
    {
        // mouse position converted to world space
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // convert mouse position to snapped position
        Vector2 snapPosition = GetSnapPosition(mousePosition);

        if (Input.GetMouseButtonDown(0)) // left mouse button is pressed down
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // this is blocking if user clicked on an ui element
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, distance, ObjectSelectLayerMask, minDepth);

                if (hit.collider != null)
                {
                    // raycast hit a tower
                    raycastedGameObject = hit.collider.gameObject;
                    raycastedGameObjectStart = raycastedGameObject.transform.position;
                    raycastedGameObjectStartSnap = snapPosition;

                    // this is used to determine if user is dragging the tower or selecting it (distance between start and current)
                    mouseStartPos = mousePosition;

                    if (towersCanBeMoved)
                    {
                        // remove an area from the list so that a tower can be placed here
                        unavailablePositions.Remove(new Vector2Int(Mathf.RoundToInt(snapPosition.x - movement.x), Mathf.RoundToInt(snapPosition.y - movement.y)));
                    }

                    // move the indicator to dragged tower and show it
                    SelectionIndicator.transform.position = raycastedGameObject.transform.position;
                    SelectionIndicator.SetActive(true);
                }
                else
                {
                    // raycast didn't hit any tower and a new tower is not being dragged
                    raycastedGameObject = null;

                    // close the selection window if it's open
                    ShowSelectionWindow(false);
                    // hide selection indicator
                    SelectionIndicator.SetActive(false);

                }

                // hide the selection window
                ShowSelectionWindow(false);
            }
        }

        if (Input.GetMouseButton(0)) // left mouse button is held down
        {
            if(boughtGameObject != null)
            {
                // drag the selected tower with the cursor
                boughtGameObject.transform.position = mousePosition;

                // show indicator around the dragged tower
                SelectionIndicator.transform.position = mousePosition;
                SelectionIndicator.SetActive(true);
                
                // move ghost to snapped position
                Ghost.transform.position = new Vector3(snapPosition.x, snapPosition.y, 0f);

                // hide the store
                if (!showStoreAllTheTime)
                {
                    storeHandler.ScreenSpaceOverlayCanvasObject.SetActive(false);
                }

                // check if area is offlimits
                if(isAreaOfflimits(snapPosition, movement))
                {
                    Ghost.SetActive(false);
                }
                else if (isAreaAvailable(snapPosition, movement)) // check if area is available
                {
                    // change the color of the ghost based on availability of the area
                    Ghost.GetComponent<SpriteRenderer>().color = Color.green;
                    Ghost.SetActive(true);
                }
                else
                {
                    Ghost.GetComponent<SpriteRenderer>().color = Color.red;
                    Ghost.SetActive(true);
                }
            }
            else if (raycastedGameObject != null && towersCanBeMoved)
            {
                // determine if we are dragging an old tower or selecting it (if we are selecting we just skip the whole GetMouseButton())
                if (!isDragging)
                {
                    // determination by distance
                    if (Vector2.Distance(mouseStartPos, mousePosition) > 0.1f)
                    {
                        isDragging = true;

                        // tower shouldn't be able to shoot while dragging
                        raycastedGameObject.GetComponent<TowerShooting>().DisableShooting();
                    }
                }
                else
                {
                    // drag the selected tower with the cursor
                    raycastedGameObject.transform.position = mousePosition;

                    // show indicator around the dragged tower
                    SelectionIndicator.transform.position = mousePosition;
                    SelectionIndicator.SetActive(true);
                    
                    // move ghost to snapped position
                    Ghost.transform.position = new Vector3(snapPosition.x, snapPosition.y, 0f);

                    // since we are dragging a tower we hide the store
                    if (!showStoreAllTheTime)
                    {
                        storeHandler.ScreenSpaceOverlayCanvasObject.SetActive(false);
                    }

                    // check if position is offlimits
                    if(isAreaOfflimits(snapPosition, movement))
                    {
                        Ghost.SetActive(false);
                    }
                    else if (isAreaAvailable(snapPosition, movement)) // check if area is available
                    {
                        // change the color of the ghost based on availability of the area
                        Ghost.GetComponent<SpriteRenderer>().color = Color.green;
                        Ghost.SetActive(true);
                    }
                    else
                    {
                        Ghost.GetComponent<SpriteRenderer>().color = Color.red;
                        Ghost.SetActive(true);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // left mouse button is released
        {
            if(boughtGameObject != null)
            {
                // check if the area is available
                if (isAreaAvailable(snapPosition, movement))//Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y)))
                {
                    // move the selected tower to position
                    boughtGameObject.transform.position = new Vector3(snapPosition.x, snapPosition.y, 0f);

                    // change this are to be occupied
                    unavailablePositions.Add(new Vector2Int(Mathf.RoundToInt(snapPosition.x - movement.x), Mathf.RoundToInt(snapPosition.y - movement.y)));

                    // show the selection window
                    ShowSelectionWindow(true);
                    // move the selection indicator to selectedGameObjects position
                    SelectionIndicator.transform.position = boughtGameObject.transform.position;
                    // show selection indicator
                    SelectionIndicator.SetActive(true);
                    // bought object is now selected
                    selectedGameObject = boughtGameObject;

                    // notify the bank that the transaction was successful (money is removed from bank)
                    bank.NewTowerWasPlacedSuccessfully();

                    // update tower's attack, range, firerate, etc. values to match the upgrade index
                    selectedGameObject.GetComponent<TowerUpgrading>().RunWhenTowerUpgrades();
                }
                else
                {
                    // area is not available so we destroy the bought object
                    Destroy(boughtGameObject);

                    // hide the selection indicator
                    SelectionIndicator.SetActive(false);

                    // there is no selected object
                    selectedGameObject = null;
                }

                // show store again
                if (!showStoreAllTheTime)
                {
                    storeHandler.ScreenSpaceOverlayCanvasObject.SetActive(true);
                }

                // hide ghost
                Ghost.SetActive(false);

                // reset bought object
                boughtGameObject = null;
            }
            else if (raycastedGameObject != null && towersCanBeMoved)
            {
                if (isDragging)
                {
                    // check if the area is available
                    if (isAreaAvailable(snapPosition, movement))//Mathf.RoundToInt(snapX - movement.x), Mathf.RoundToInt(snapY - movement.y)))
                    {
                        // move the selected tower to position
                        raycastedGameObject.transform.position = new Vector3(snapPosition.x, snapPosition.y, 0f);

                        // change this are to be occupied
                        unavailablePositions.Add(new Vector2Int(Mathf.RoundToInt(snapPosition.x - movement.x), Mathf.RoundToInt(snapPosition.y - movement.y)));

                        // tower is placed so we add a cooldown timer after which the tower can shoot again
                        raycastedGameObject.GetComponent<TowerShooting>().EnableShooting(cooldownTime);
                        UIRadialTimerManager.Instance.AddTimer(raycastedGameObject.transform.position, cooldownTime);
                    }
                    else
                    {
                        // area is not available

                        // we move the selected tower back where it was
                        raycastedGameObject.transform.position = raycastedGameObjectStart;
                        // this position is now unavailable
                        unavailablePositions.Add(new Vector2Int(Mathf.RoundToInt(raycastedGameObjectStartSnap.x - movement.x), Mathf.RoundToInt(raycastedGameObjectStartSnap.y - movement.y)));

                        // tower can shoot again without cooldown timer
                        raycastedGameObject.GetComponent<TowerShooting>().EnableShooting(0f);
                    }

                    // we are no longer dragging
                    isDragging = false;
                }
                else
                {
                    // change this are to be occupied
                    unavailablePositions.Add(new Vector2Int(Mathf.RoundToInt(snapPosition.x - movement.x), Mathf.RoundToInt(snapPosition.y - movement.y)));
                }

                // show the selection window
                ShowSelectionWindow(true);
                // move the selection indicator to selectedGameObjects position
                SelectionIndicator.transform.position = raycastedGameObject.transform.position;
                // show selection indicator
                SelectionIndicator.SetActive(true);

                // raycasted object is now selected
                selectedGameObject = raycastedGameObject;

                // show the store again
                if (!showStoreAllTheTime)
                {
                    storeHandler.ScreenSpaceOverlayCanvasObject.SetActive(true);
                }

                // reset raycastObject
                raycastedGameObject = null;

                // hide ghost
                Ghost.SetActive(false);
            }
            else if (raycastedGameObject != null && !towersCanBeMoved)
            {
                // show the selection window
                ShowSelectionWindow(true);

                // raycasted object is now selected
                selectedGameObject = raycastedGameObject;

                // reset raycastObject
                raycastedGameObject = null;
            }
        }
    }

    private Vector2 GetSnapPosition(Vector3 mousePosition)
    {
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

        return new Vector2(snapX, snapY);
    }

    // this is called when player successfully buys a new tower from the store
    public void NewTower(GameObject go)
    {
        boughtGameObject = go;

        // hide selection window
        ShowSelectionWindow(false);
        // hide selection indicator
        SelectionIndicator.SetActive(false);
    }

    private bool isAreaAvailable(Vector2 snapPosition, Vector3 movement)
    {
        Vector2Int v = new Vector2Int(Mathf.RoundToInt(snapPosition.x - movement.x), Mathf.RoundToInt(snapPosition.y - movement.y));
        foreach (Vector2Int unavailablePosition in unavailablePositions)
        {
            if ((v.x == unavailablePosition.x && v.y == unavailablePosition.y))
            {
                return false;
            }
        }

        return true;
    }

    private bool isAreaOfflimits(Vector2 snapPosition, Vector3 movement)
    {
        Vector2Int v = new Vector2Int(Mathf.RoundToInt(snapPosition.x - movement.x), Mathf.RoundToInt(snapPosition.y - movement.y));
        foreach (Vector2Int offlimitPosition in offlimitsPositions)
        {
            if ((v.x == offlimitPosition.x && v.y == offlimitPosition.y))
            {
                return true;
            }
        }

        return false;
    }

    private void ShowSelectionWindow(bool show)
    {
        if (show)
        {
            if(boughtGameObject != null)
            {
                selectionWindow.Init(boughtGameObject.GetComponent<TowerInfo>());
                selectionWindowCanvasObject.SetActive(show);
            }
            else if (raycastedGameObject != null)
            {
                selectionWindow.Init(raycastedGameObject.GetComponent<TowerInfo>());
                selectionWindowCanvasObject.SetActive(show);
            }
        }
        else
        {
            selectionWindow.CloseSelectionWindow();
            selectionWindowCanvasObject.SetActive(show);
        }
    }

    public void SelectedTowerWasSold(TowerInfo towerInfo)
    {
        //unavailablePositions.Remove(new Vector2Int(Mathf.RoundToInt(selectedGameObjectStartSnap.x - movement.x), Mathf.RoundToInt(selectedGameObjectStartSnap.y - movement.y)));
        unavailablePositions.Remove(new Vector2Int(Mathf.RoundToInt(selectedGameObject.transform.position.x - movement.x), Mathf.RoundToInt(selectedGameObject.transform.position.y - movement.y)));
        Destroy(towerInfo.gameObject);
        selectedGameObject = null;
        // hide the selection window
        ShowSelectionWindow(false);
        // hide the selection indicator
        SelectionIndicator.SetActive(false);
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

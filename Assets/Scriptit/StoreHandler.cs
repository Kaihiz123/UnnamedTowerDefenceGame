using UnityEngine;

public class StoreHandler : MonoBehaviour
{
    // Store layout
    public GameObject ScreenSpaceOverlayCanvasObject;
    // Visual grid on top of the background
    public GameObject WorldSpaceCanvasObject;

    public Transform ObjectsOnGrid; // Object that has all the bought towers and the ghost

    // different towers that are instantiated when player buys a new tower
    public GameObject Tower1Prefab;
    public GameObject Tower2Prefab;
    public GameObject Tower3Prefab;
    public GameObject Tower4Prefab;
    public GameObject Tower5Prefab;

    // script that handles money
    public Bank bank;

    bool isDragging = false; // if tower is being dragged
    bool newTower = false; // if tower is new (just bought)

    TowerPlacementGrid tpg;
    Camera mainCamera;

    private void Start()
    {
        tpg = GetComponent<TowerPlacementGrid>();
        mainCamera = Camera.main;
    }

    Vector3 previousPosition = Vector3.zero;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            // show if hidden, hide if visible
            ScreenSpaceOverlayCanvasObject.SetActive(!ScreenSpaceOverlayCanvasObject.activeInHierarchy);
        }

        // middle mouse button is used to move the grid
        if (Input.GetMouseButtonDown(2)) // if mouse middle button is pressed
        {
            // save the current position of the mouse (this is used to calculate delta)
            previousPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            isDragging = true;
        }
        else if(Input.GetMouseButton(2))// if mouse middle button is held
        {
            if (isDragging)
            {
                // convert mouses position on screen to game world position
                Vector3 currentPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                // how the mouse has been moved between frames
                Vector3 delta = currentPosition - previousPosition;
                // save the current position of the mouse
                previousPosition = currentPosition;

                // move the grid
                transform.position += delta;
                // move the canvas with visible grid
                WorldSpaceCanvasObject.transform.position += delta;

                // notify how the grid has been moved
                tpg.movement += delta;
            }
        }
        else if (Input.GetMouseButtonUp(2)) // if mouse middle button is released
        {
            // we are no longer dragging the grid
            isDragging = false;
        }

        // if a tower was bought from the store and left button of the mouse was released
        if (Input.GetMouseButtonUp(0) && newTower)
        {
            // the tower is no longer new
            newTower = false;
            // notify that dragging of the new tower ends
            tpg.NewTowerDragEnd();
        }
    }

    public void CursorEnterStoreItem(GameObject go)
    {
        // TODO: indicate the storeItem that cursor is above the storeItem
    }

    public void CursorExitStoreItem(GameObject go)
    {
        // TODO: stop indicating that the storeItem is no longer above the storeItem
    }

    public void MouseButtonDownOnStoreItem(GameObject go, int index)
    {
        // Left mouse button was pressed down above a certain storeItem
        
        // Does the player have enough money
        if (bank.BuyTower(index))
        {
            // just to know that we are creating a new tower
            newTower = true;

            // Hide store
            ScreenSpaceOverlayCanvasObject.SetActive(false);

            // Create a prefab of the tower into cursors position ja let
            // the towerPlacementGrid know that we are dragging the new tower

            GameObject go1 = Instantiate(Tower1Prefab);
            // Set the new tower child of ObjectsOnGrid
            go1.transform.SetParent(ObjectsOnGrid);
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            // Set the position of the new tower into cursor position
            go1.transform.position = mousePosition;
            // Notify the towerPlacementGrid of the new tower
            tpg.NewTower(go1);
        }
        
    }

    public void MouseButtonUpOnStoreItem(GameObject go)
    {
        // TODO: do something with this; this should be called if player doesn't have enough money to buy the tower
    }
}

using System.Collections.Generic;
using UnityEngine;

public class StoreHandler : MonoBehaviour
{
    //TODO: hide/show animation of the store window (translate left/right)

    public bool EnableMovingTheGrid;

    // Store layout
    public GameObject ScreenSpaceOverlayCanvasObject;
    // Visual grid on top of the background
    public GameObject WorldSpaceCanvasObject;

    public Transform ObjectsOnGrid; // Object that has all the bought towers and the ghost

    // different towers that are instantiated when player buys a new tower
    public List<GameObject> towerPrefabs = new List<GameObject>();

    // script that handles money
    public Bank bank;

    bool isDragging = false; // if tower is being dragged

    public TowerPlacementGrid towerPlacementGrid;
    Camera mainCamera;

    [Header("Tower Type Upgrades")]
    public TowerTypeUpgradeDataSO towerUpgrades;

    private void Start()
    {
        mainCamera = Camera.main;
        ScreenSpaceOverlayCanvasObject.SetActive(true);
    }

    Vector3 previousPosition = Vector3.zero;

    private void Update()
    {

        if (EnableMovingTheGrid)
        {
            // middle mouse button is used to move the grid
            if (Input.GetMouseButtonDown(2)) // if mouse middle button is pressed
            {
                // save the current position of the mouse (this is used to calculate delta)
                previousPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                isDragging = true;
            }
            else if (Input.GetMouseButton(2))// if mouse middle button is held
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
                    towerPlacementGrid.transform.position += delta;
                    // move the canvas with visible grid
                    WorldSpaceCanvasObject.transform.position += delta;

                    // notify how the grid has been moved
                    towerPlacementGrid.movement += delta;
                }
            }
            else if (Input.GetMouseButtonUp(2)) // if mouse middle button is released
            {
                // we are no longer dragging the grid
                isDragging = false;
            }
        }

    }

    public void CursorEnterStoreItem(GameObject go)
    {
        // TODO: indicate the storeItem that cursor is above the storeItem
    }

    public void CursorExitStoreItem(GameObject go)
    {
        // TODO: stop indicating that the cursor is no longer above the storeItem
    }

    public void MouseButtonDownOnStoreItem(GameObject pressedStoreItemObject, int index)
    {
        // Left mouse button was pressed down above a certain storeItem

        // cost of the tower
        int cost = towerUpgrades.towerType[index].upgradeLevels[0].upgradeCost;

        // Does the player have enough money
        if (bank.BuyTower(cost))
        {
            // Hide store 
            //ScreenSpaceOverlayCanvasObject.SetActive(false);

            // Create a prefab of the tower into cursors position ja let
            // the towerPlacementGrid know that we are dragging the new tower

            GameObject newTowerObject = Instantiate(towerPrefabs[index]);
            newTowerObject.transform.localScale = towerPlacementGrid.ElementSize;
            // Set the new tower child of ObjectsOnGrid
            newTowerObject.transform.SetParent(ObjectsOnGrid);
            // update tower's attack, range, firerate, etc. values to match the upgrade index
            newTowerObject.GetComponent<TowerUpgrading>().RunWhenTowerUpgrades();
            // show area visual
            newTowerObject.GetComponent<TowerShooting>().ShowAreaVisual(true);
            // pass the tower type from storeItem to towerInfo
            TowerInfo towerInfo = newTowerObject.GetComponent<TowerInfo>();
            towerInfo.towerType = pressedStoreItemObject.GetComponent<StoreItem>().towerType;
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            // Set the position of the new tower into cursor position
            newTowerObject.transform.position = mousePosition;
            // Notify the towerPlacementGrid of the new tower
            towerPlacementGrid.NewTower(newTowerObject);
        }
    }

    public void MouseButtonUpOnStoreItem(GameObject go)
    {
        // TODO: do something with this; this should be called if player doesn't have enough money to buy the tower
    }
}

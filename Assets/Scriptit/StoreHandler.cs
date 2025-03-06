using UnityEngine;

public class StoreHandler : MonoBehaviour
{
    public GameObject ScreenSpaceOverlayCanvasObject;
    public GameObject WorldSpaceCanvasObject;

    public Transform ObjectsOnGrid;

    public GameObject Tower1Prefab;
    public GameObject Tower2Prefab;
    public GameObject Tower3Prefab;
    public GameObject Tower4Prefab;
    public GameObject Tower5Prefab;

    bool isDragging = false;

    TowerPlacementGrid tpg;

    private void Start()
    {
        tpg = GetComponent<TowerPlacementGrid>();
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            // show if hidden, hide if visible
            ScreenSpaceOverlayCanvasObject.SetActive(!ScreenSpaceOverlayCanvasObject.activeInHierarchy);
        }

        if (Input.GetMouseButtonDown(2)) // if mouse middle button is pressed
        {
            //TODO: if we raycast and hit ui -> dont start dragging else start dragging
            
            isDragging = true;
        }
        else if(Input.GetMouseButton(2))// if mouse middle button is held
        {
            if (isDragging)
            {
                Vector3 delta = Input.mousePositionDelta;

                // move the grid
                transform.position += delta;
                // move the canvas with visible grid
                WorldSpaceCanvasObject.transform.position += delta;

                // notify how grid has been moved
                tpg.movement += delta;
            }
        }
        else if (Input.GetMouseButtonUp(2)) // if mouse middle button is released
        {
            isDragging = false;
        }
    }

    public void CursorEnterStoreItem(GameObject go)
    {
        Debug.Log("mouseEnter=" + go.name);
    }

    public void CursorExitStoreItem(GameObject go)
    {
        Debug.Log("mouseExit=" + go.name);
    }

    public void MouseButtonDownOnStoreItem(GameObject go)
    {
        // TODO: piilotetaan store (ScreenSpaceOverlayCanvasObject)

        ScreenSpaceOverlayCanvasObject.SetActive(false);

        //TODO: luodaan prefabi cursorin sijaintiin ja kerrotaan towerPlacementGrid-scriptille että
        //ollaan liikuttamassa toweria, ja instantioitu gameObjecti on selectedObject

        GameObject go1 = Instantiate(Tower1Prefab);
        go1.transform.SetParent(ObjectsOnGrid);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        go1.transform.position = mousePosition;
        tpg.NewTower(go1);
    }

    public void MouseButtonUpOnStoreItem(GameObject go)
    {

    }
}

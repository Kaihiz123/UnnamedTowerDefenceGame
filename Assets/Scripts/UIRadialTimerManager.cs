using System.Collections.Generic;
using UnityEngine;

public class UIRadialTimerManager : MonoBehaviour
{
    public static UIRadialTimerManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject uiRadialTimerPrefab;

    public void AddTimer(Vector3 worldPosition, float cooldownTime)
    {
        GameObject newTimerObject = Instantiate(uiRadialTimerPrefab);
        newTimerObject.transform.position = worldPosition;
        newTimerObject.transform.SetParent(transform);
        CooldownTimerScript cts = newTimerObject.GetComponent<CooldownTimerScript>();
        cts.Init(cooldownTime);
    }
}

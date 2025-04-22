using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Hiscore : MonoBehaviour
{

    public GameObject hiscoreItemPrefab;
    public Transform hiscoreItemParent;

    public void GetLeaderboard()
    {
        UGSManager.Instance.GetTopScores();
    }

    public void ShowLeaderboard(Dictionary<string, int> leaderboard)
    {
        // destroy old ones
        Transform[] transforms = hiscoreItemParent.GetComponentsInChildren<Transform>(true);
        foreach(Transform t in transforms)
        {
            if(t != null)
            {
                if (t.parent == hiscoreItemParent)
                {
                    Destroy(t.gameObject);
                }
            }
        }

        int runningNumber = 1;

        // instantiate prefabs
        foreach (KeyValuePair<string, int> entry in leaderboard)
        {
            GameObject go = Instantiate(hiscoreItemPrefab);
            go.name = "HiscoreItem";
            go.transform.SetParent(hiscoreItemParent);
            MenuSettingsItemText msit = go.GetComponent<MenuSettingsItemText>();
            string playerName = entry.Key.Split("#")[0];
            msit.itemName1 = "#" + runningNumber + " " + playerName;
            msit.itemName2 = "" + entry.Value;
            msit.UpdateTexts();
            runningNumber++;
        }
    }


}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseScript : MonoBehaviour
{

    public List<GameObject> playerBaseObjects = new List<GameObject>();

    public void PlayerBaseHealthChange(float health, float maxHealth)
    {
        if(health > 0f)
        {
            float step = maxHealth / (float)(playerBaseObjects.Count - 1);
            for(int i = 0; i < playerBaseObjects.Count; i++)
            {
                Debug.Log("" + ((float) i * step) + " < " + health + " < " + ((float) i + 1) * step);
                if ((float) i * step < health && health < ((float) i + 1) * step)
                {
                    ShowPlayerBase(i);
                    return;
                }
            }
        }
        else
        {
            PlayerBaseDestroyed();
        }
    }

    private void PlayerBaseDestroyed()
    {
        ShowPlayerBase(playerBaseObjects.Count - 1);
    }

    private void ShowPlayerBase(int index)
    {
        for(int i = 0; i < playerBaseObjects.Count; i++)
        {
            playerBaseObjects[i].SetActive(i == index);
        }
    }

}

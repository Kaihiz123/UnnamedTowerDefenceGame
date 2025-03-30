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
            for(int i = 0; i < playerBaseObjects.Count - 1; i++)
            {
                //Debug.Log("health=" + health + " <= " + (float) i * step);
                if(health <= (float) i * step)
                {
                    ShowPlayerBase(i);
                    return;
                }
            }

            ShowPlayerBase(playerBaseObjects.Count - 1);
        }
        else
        {
            PlayerBaseDestroyed();
        }
    }

    private void PlayerBaseDestroyed()
    {
        ShowPlayerBase(0);
    }

    private void ShowPlayerBase(int index)
    {
        for(int i = 0; i < playerBaseObjects.Count; i++)
        {
            playerBaseObjects[i].SetActive(i == index);
        }
    }

}

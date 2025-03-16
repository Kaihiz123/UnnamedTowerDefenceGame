using System.Collections.Generic;
using UnityEngine;

public class StoreItemHandler : MonoBehaviour
{
    public StoreHandler storeHandler;
    public Bank bank;

    StoreItem[] storeItems; // items that are available in the store

    public void Init()
    {
        storeItems = GetComponentsInChildren<StoreItem>(true);
        foreach(StoreItem storeItem in storeItems)
        {
            // initialize storeItems so that they are ready
            storeItem.Init();
        }
        // change the colors of the towers in the store
        ChangeUITowerColors();
    }

    public void ChangeUITowerColors()
    {
        foreach(StoreItem storeItem in storeItems)
        {
            // change the color of the tower in the store based on if the player has enough money to by them
            if(bank.GetPlayerMoney() >= storeItem.price)
            {
                storeItem.PlayerCanAfford();
            }
            else
            {
                storeItem.PlayerCannotAfford();
            }
        }
    }

    // return list of the prices of the StoreItems where the order matters
    public List<int> GetStoreItemPrices()
    {
        List<int> prices = new List<int>();
        foreach(StoreItem storeItem in storeItems)
        {
            prices.Add(storeItem.price);
        }
        return prices;
    }
}

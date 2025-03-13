using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    // how much money the player has
    public int playerMoney;

    public StoreItemHandler storeItemHandler;

    // prices of the towers in store
    List<int> prices = new List<int>();

    public TMPro.TextMeshProUGUI bankBalanceText;

    private void Start()
    {
        // initialize storeItemHandler
        storeItemHandler.Init();
        
        // get the prices of towers
        prices = storeItemHandler.GetStoreItemPrices();
        // update the bank balance to show how much money the player has
        UpdateBankBalanceText();
    }

    public bool BuyTower(int towerIndexInStore)
    {   
        // if the player has enough money to buy the tower
        if(playerMoney >= prices[towerIndexInStore])
        {
            // players money isn't be reduced immediately because the player might try to place the tower on an unavailable area
            // money is reduced when the tower was placed successfully to the grid

            newTowerIndex = towerIndexInStore;
            
            return true;
        }
        else
        {
            // TODO: add sound of failure
            return false;
        }
    }

    public bool BuyUpgrade(int price)
    {
        // if the player has enough money to buy the tower
        if (playerMoney >= price)
        {
            playerMoney -= price;
            UpdateBankBalanceText();
            
            return true;
        }
        else
        {
            // TODO: add sound of failure
            return false;
        }
    }

    int newTowerIndex;
    public void NewTowerWasPlacedSuccessfully()
    {
        // reduce players money
        playerMoney -= prices[newTowerIndex];
        // update store towers color so it indicates if player has enough money
        storeItemHandler.ChangeUITowerColors();
        // update the bank balance to show how much money the player has
        UpdateBankBalanceText();
    }

    private void UpdateBankBalanceText()
    {
        // change the text above the store
        bankBalanceText.text = "" + playerMoney;
    }

}

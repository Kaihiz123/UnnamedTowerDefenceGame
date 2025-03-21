using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    // how much money the player has
    [SerializeField] private int playerMoney;

    public StoreItemHandler storeItemHandler;

    int price;

    public TMPro.TextMeshProUGUI bankBalanceText;

    private void Start()
    {
        // initialize player money
        playerMoney = PlayerPrefs.GetInt(ISettings.Type.STARTMONEY.ToString(), 4000);

        // initialize storeItemHandler
        storeItemHandler.Init();
        
        // update the bank balance to show how much money the player has
        UpdateBankBalanceText();
    }

    public bool BuyTower(int price)
    {
        this.price = price;
        
        // if the player has enough money to buy the tower
        if (playerMoney >= price)
        {
            // players money isn't be reduced immediately because the player might try to place the tower on an unavailable area
            // money is reduced when the tower was placed successfully to the grid

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

    //int newTowerIndex;
    public void NewTowerWasPlacedSuccessfully()
    {
        // reduce players money
        playerMoney -= price;
        // update the bank balance to show how much money the player has
        UpdateBankBalanceText();
    }

    private void UpdateBankBalanceText()
    {
        // change the text above the store
        bankBalanceText.text = "" + playerMoney;
        // update store towers color so it indicates if player has enough money
        storeItemHandler.ChangeUITowerColors();
    }

    public void IncreasePlayerMoney(int money)
    {
        playerMoney += money;
        UpdateBankBalanceText();
    }

    public int GetPlayerMoney()
    {
        return playerMoney;
    }
}

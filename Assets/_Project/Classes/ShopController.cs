using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the purchase of new skins and required UI updates.
/// </summary>

namespace SnakeWarzIO
{
    public class ShopController : MonoBehaviour
    {
        public static ShopController instance;

        [Header("UI Items")]
        public TMP_Text playerCoinsUI;
        public GameObject demoSnake;

        [Header("Shop Details")]
        private int playerCoins;

        private int cheatClickCounter;

        public delegate void PurchaseItemAction();
        public static event PurchaseItemAction OnPurchaseItem;


        private void Awake()
        {
            instance = this;
            Time.timeScale = 1f;
            Application.targetFrameRate = 60;
            cheatClickCounter = 0;

            //Init shop for the first time
            if (PlayerPrefs.GetInt("ShopIsInited") == 0)
            {
                //Mark default items as opened
                PlayerPrefs.SetInt("SelectedTableID", 0);
                PlayerPrefs.SetInt("SelectedBallID", 100);
                PlayerPrefs.SetInt("SelectedCupID", 200);

                //Mark shop as inited
                PlayerPrefs.SetInt("ShopIsInited", 1);

                //Debug
                print("Shop is inited successfuly!");
            }

            UpdatePlayerCoinsUI();
        }

        /// <summary>
        /// Display current player coins on UI
        /// </summary>
        public void UpdatePlayerCoinsUI()
        {
            playerCoins = PlayerPrefs.GetInt("PlayerCoins");
            playerCoinsUI.text = "" + playerCoins;
        }

        /// <summary>
        /// Purchase the given item by running all required checks in advance.
        /// </summary>
        /// <param name="itemDetail"></param>
        public void PurchaseItem(ShopItemController itemDetail)
        {
            //Item details
            print(
                "Purchase command received: " +
                "ItemName: " + itemDetail.itemName + "\r\n" +
                "ItemType: " + itemDetail.itemType + "\r\n" +
                "ItemID: " + itemDetail.itemID + "\r\n" +
                "ItemPrice: " + itemDetail.itemPrice + "\r\n" +
                "PlayerCoins: " + playerCoins + "\r\n" +
                "ItemIsPurchased: " + itemDetail.isPurchased + "\r\n" +
                "ItemIsSelected: " + itemDetail.isSelected
                );

            //Preview the snake as demo
            PreviewSelectedSnake(itemDetail.itemID);

            //if the item is already purchased, select it.
            if (itemDetail.isPurchased)
            {
                print("Item is Selected: " + itemDetail.itemName);
                PlayerPrefs.SetInt("Selected" + itemDetail.itemType + "ID", itemDetail.itemID);

                //Purchase event
                OnPurchaseItem?.Invoke();

                SfxPlayer.instance.PlaySfx(10);
            }
            else
            {
                //if not, purchase the item.

                //check if player has enough funds
                if (playerCoins >= itemDetail.itemPrice)
                {
                    print("Item is purchased: " + itemDetail.itemName);
                    PlayerPrefs.SetInt(itemDetail.itemType + "_" + itemDetail.itemID + "_IsPurchased", 1);
                    PlayerPrefs.SetInt("Selected" + itemDetail.itemType + "ID", itemDetail.itemID);

                    SfxPlayer.instance.PlaySfx(11);

                    //Purchase event
                    OnPurchaseItem?.Invoke();

                    //Deduct the price from player coin
                    int newPlayerCoin = playerCoins - itemDetail.itemPrice;
                    PlayerPrefs.SetInt("PlayerCoins", newPlayerCoin);

                    //Update player data on UI
                    UpdatePlayerCoinsUI();
                }
                else
                {
                    print("Not enough coin to purchase this item.");
                    SfxPlayer.instance.PlaySfx(12);
                    return;
                }
            }
        }

        /// <summary>
        /// To ease the tests, we have this function in place, so you can increase your money and purchase every items available in the shop.
        /// To use this method, you need to click on the small coin icon (top-right) 5 times.
        /// </summary>
        public void CheatClick()
        {
            cheatClickCounter++;

            if (cheatClickCounter > 5)
            {
                SfxPlayer.instance.PlaySfx(8);
                cheatClickCounter = 0;
                PlayerPrefs.SetInt("PlayerCoins", PlayerPrefs.GetInt("PlayerCoins") + 5000);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void PreviewSelectedSnake(int skinID = 0)
        {
            demoSnake.GetComponent<DemoSnake>().PreviewSkin(skinID);
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
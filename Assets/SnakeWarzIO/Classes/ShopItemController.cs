using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Every item inside the shop needs to have this class so we can assign the required item properties
/// such as type, name, price and ID
/// </summary>

namespace SnakeWarzIO
{
    public class ShopItemController : MonoBehaviour
    {
        public enum ItemTypes { Skin = 0 }

        [Header("Item Details")]
        public ItemTypes itemType = ItemTypes.Skin;
        public int itemID = 0;
        public int itemPrice = 500;
        public string itemName = "SkinItem-#";
        //public Sprite itemIcon;

        [Header("UI Objects")]
        public Text itemNameUI;
        public Text itemPriceUI;
        public Button itemButtonUI;
        public Text itemButtonTextUI;
        public GameObject buyBlock;
        public GameObject buyBlockCoinIcon;

        [Header("UI Resources")]
        public Sprite[] buttonSprites;

        [Header("Item Status")]
        public string itemPrefsKey;
        public bool isPurchased;
        public bool isSelected;

        public void OnEnable()
        {
            ShopController.OnPurchaseItem += CheckItemStatus;
        }

        public void OnDisable()
        {
            ShopController.OnPurchaseItem -= CheckItemStatus;
        }

        void Start()
        {
            itemPrefsKey = itemType + "_" + itemID;
            //print("ItemDetail: " + itemPrefsKey); 

            CheckItemStatus();
        }

        /// <summary>
        /// Each item is responsible for its state. Here we run the checks to get the status and take appropriate actions. 
        /// </summary>
        public void CheckItemStatus()
        {
            //Make everything locked
            isPurchased = false;
            isSelected = false;

            //Check item status
            if (PlayerPrefs.GetInt(itemPrefsKey + "_IsPurchased") == 1 || itemPrice == 0)
                isPurchased = true;

            if (PlayerPrefs.GetInt("Selected" + itemType + "ID") == itemID)
                isSelected = true;

            //Apply item data to item UI
            itemNameUI.text = "" + itemName;
            itemPriceUI.text = "" + itemPrice;

            //Apply status to items
            if (!isPurchased)
            {
                itemButtonUI.image.sprite = buttonSprites[0];
                itemButtonTextUI.text = "Buy";
                buyBlockCoinIcon.SetActive(true);
            }
            else if (isPurchased && !isSelected)
            {
                itemButtonUI.image.sprite = buttonSprites[1];
                itemButtonTextUI.text = "Use";
                itemPriceUI.text = "<color=cyan>Purchased</color>";
                buyBlockCoinIcon.SetActive(false);
            }
            else if (isPurchased && isSelected)
            {
                itemButtonUI.image.sprite = buttonSprites[2];
                itemButtonTextUI.text = "In Use";
                itemPriceUI.text = "<color=cyan>Purchased</color>";
                buyBlockCoinIcon.SetActive(false);
            }
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SnakeWarzIO
{
    public class WofItemController : MonoBehaviour
    {
        [Header("Settings")]
        public WofController.WheelItemTypes itemType = WofController.WheelItemTypes.NoPrize;
        public int itemID = 0;

        [Header("UI Items")]
        public Image itemIconUI;
        public Text itemValueUI;

        [Header("Data")]
        public WofController.WheelItemData itemData;

        private void Awake()
        {
            
        }

        void Start()
        {
            UpdateItemState();
        }

        /// <summary>
        /// Display item data on UI
        /// </summary>
        public void UpdateItemState()
        {
            itemData = WofController.instance.GetItemDataByType(itemType);

            itemIconUI.sprite = itemData.wheelItemIcon;
            itemValueUI.text = "x" + itemData.wheelItemValue;
        }
    }
}
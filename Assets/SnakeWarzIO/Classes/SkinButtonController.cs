using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SnakeWarzIO
{
    public class SkinButtonController : MonoBehaviour
    {
        public enum SkinTypes { Food = 0, Pit = 1 }        

        [Header("Button Settings")]
        public SkinTypes skinType = SkinTypes.Food;
        public int skinButtonID;
        public bool isSelected;

        [Header("Object References")]
        public Button buttonUI;
        public GameObject buttonSelectFrame;

        public delegate void SkinSelectionAction();
        public static event SkinSelectionAction OnSkinSelection;


        private void Awake()
        {
            buttonSelectFrame.SetActive(false);
        }

        void Start()
        {
            UpdateItemStatus();
        }

        public void SelectButton(SkinButtonController sbc)
        {
            SfxPlayer.instance.PlaySfx(10);

            print($"{sbc.skinType} skin button selected: #{sbc.skinButtonID}");
            PlayerPrefs.SetInt($"Selected{sbc.skinType}SkinID", sbc.skinButtonID);

            OnSkinSelection?.Invoke();
        }

        public void OnEnable()
        {
            OnSkinSelection += UpdateItemStatus;
        }

        public void OnDisable()
        {
            OnSkinSelection -= UpdateItemStatus;
        }

        public void UpdateItemStatus()
        {
            if(skinType == SkinTypes.Food && skinButtonID == PlayerPrefs.GetInt("SelectedFoodSkinID", 0) || (skinType == SkinTypes.Pit && skinButtonID == PlayerPrefs.GetInt("SelectedPitSkinID", 0)))
            {
                isSelected = true;
                buttonSelectFrame.SetActive(true);
            }
            else
            {
                isSelected = false;
                buttonSelectFrame.SetActive(false);
            }          
        }
    }
}
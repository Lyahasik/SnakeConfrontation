using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// When player buy boosters from the shop, or obtain them thorugh ingame activities like daily login, or wheel of fortune, they can utilize their available (saved) boosters via 
/// ingame UI buttons. This class handles everything related to utilization of saved boosters inside the game.
/// </summary>

namespace SnakeWarzIO
{
    public class BoosterButtonController : MonoBehaviour
    {       
        //Type (ID) of this booster
        public BoostersController.BoosterTypes boosterID = BoostersController.BoosterTypes.Unzoom;

        [Header("Settings & Values")]
        private int availableAmount;        //This needs to be read from PlayerPrefs

        [Header("References")]
        public Button buttonReference;
        public CanvasGroup parentCanvasGroup;
        public CanvasGroup boosterIconCanvasGroup;
        public Text amountUI;
        public Image boosterFillUI;

        private void Awake()
        {
            UpdateBoosterStatus();
        }

        public void OnEnable()
        {
            BoostersController.OnBoosterAmountUpdateAction += UpdateBoosterStatus;
        }

        public void OnDisable()
        {
            BoostersController.OnBoosterAmountUpdateAction -= UpdateBoosterStatus;
        }

        /// <summary>
        /// Update the amount and status of this booster button on UI
        /// </summary>
        public void UpdateBoosterStatus()
        {
            availableAmount = PlayerPrefs.GetInt($"AvailableBoosterByID{(int)boosterID}", 0);

            amountUI.text = "" + availableAmount;
            boosterFillUI.fillAmount = 0;
            
            if (availableAmount > 0)
            {
                MakeInteractable(true);
                //parentCanvasGroup.alpha = 1f;
                boosterIconCanvasGroup.alpha = 1f;
            }
            else
            {
                MakeInteractable(false);
                //parentCanvasGroup.alpha = 0.3f;
                boosterIconCanvasGroup.alpha = 0.3f;
            }
        }

        private void Start()
        {

        }

        public void MakeInteractable(bool state = true)
        {
            buttonReference.interactable = state;
        }

        public void ConsumeBooster()
        {
            availableAmount--;

            if (availableAmount == 0)
            {
                MakeInteractable(false);
            }

            PlayerPrefs.SetInt($"AvailableBoosterByID{(int)boosterID}", availableAmount);

            UpdateBoosterStatus();
        }

        /// <summary>
        /// Clicking on the UI booster button consumes one instance of the booster if allowed by BoostersController.
        /// </summary>
        public void UseThisBooster()
        {
            //Return if using boosters in not allowed by the BoostersController class
            if (!BoostersController.canUseBoosters)
                return;

            //Sfx
            SfxPlayer.instance.PlaySfx(3);

            //Update the amount and reflect the new value on UI
            ConsumeBooster();

            //Apply the effect of this booster to the game.
            BoostersController.instance.ActivateBoosterIngame((int)boosterID);
        }

        /// <summary>
        /// Update the timer bar on this booster button to give the player an idea for the remaining time.
        /// </summary>
        /// <param name="remTime"></param>
        /// <param name="maxTime"></param>
        public void RunTimerUI(float remTime, float maxTime)
        {
            boosterFillUI.fillAmount = remTime / maxTime;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

/// <summary>
/// This is a fully functional wheel-of-fortune system that helps with player retention and allows your player to return to game every once in a while to try their luck with the wheel and win
/// some useful ingame boosters or a bit of extra coins.
/// </summary>

namespace SnakeWarzIO
{
    public class WofController : MonoBehaviour
    {
        public static WofController instance;
        public static bool canSpinTheWheel;
        public const int totalWheelItems = 16;
        public static int spinResult;

        public enum WheelItemTypes
        {
            UnzoomBooster_1x = 0,
            MagnetBooster_1x,
            ScoreMultiplierBooster_1x,
            ExtraSpeedMultiplier_1x,
            CoinPrize_100x,
            CoinPrize_250x,
            CoinPrize_1000x,
            NoPrize
        }

        [Header("Wheel Items")]
        public GameObject[] wheelItems;

        [Header("Data Holders")]
        public List<WheelItemData> wheelItemDatas;
        public static WheelItemData selectedItemData;

        [Header("Child objects & Components")]
        public GameObject wheelBody;
        public GameObject wheelIndicator;
        public Animator wheelAnimator;
        public Animator wheelIndicatorAnimator;

        [Header("Object References")]
        public Button spinButtonUI;
        public GameObject spinButtonReadyLabel;
        public TMP_Text spinButtonNotreadyLabel;
        public GameObject ResultPanel;
        public Image resultIconUI;
        public TMP_Text resultValueUI;
        public TMP_Text resultDescriptionUI;

        [Header("Time Settings")]
        public int spinCooldown = 3600; //Seconds
        private DateTime baseTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        private int currentTime;

        [Serializable]
        public class WheelItemData
        {
            public Sprite wheelItemIcon;
            public int wheelItemValue;
            public string wheelItemDescription;
        }

        private void Awake()
        {
            instance = this;
            canSpinTheWheel = false;
            spinResult = -1;
            ResultPanel.SetActive(false);
            selectedItemData = null;

            //Needed only once
            if (!PlayerPrefs.HasKey("LastSpinTime"))
            {
                PlayerPrefs.SetInt("LastSpinTime", 0);
                print($"Initial timestamp is saved: {PlayerPrefs.GetInt("LastSpinTime")}");
            }

            //Cache all wheelItem objects
            wheelItems = GameObject.FindGameObjectsWithTag("WheelItem").OrderBy(go => Regex.Replace(go.name, @"\d+", match => match.Value.PadLeft(4, '0'))).ToArray();
        }

        private void OnEnable()
        {
            CheckFreeSpinCondition();
        }

        public void CheckFreeSpinCondition()
        {
            currentTime = (int)(DateTime.UtcNow - baseTime).TotalSeconds;
            print("Current time: " + currentTime.ToString());

            if (currentTime >= spinCooldown + PlayerPrefs.GetInt("LastSpinTime") || PlayerPrefs.GetInt("LastSpinTime") == 0)
            {
                print("Free spin is allowed");
                canSpinTheWheel = true;
            }
            else
            {
                print("Free spin is NOT allowed");
                canSpinTheWheel = false;
            }

            UpdateSpinButton();
        }

        public void UpdateSpinButton()
        {
            if (canSpinTheWheel)
            {
                spinButtonUI.enabled = true;
                spinButtonReadyLabel.SetActive(true);
                spinButtonNotreadyLabel.gameObject.SetActive(false);
            }
            else
            {
                spinButtonUI.enabled = false;
                spinButtonReadyLabel.SetActive(false);
                spinButtonNotreadyLabel.gameObject.SetActive(true);

                int remainingTimeToNextSpin = (spinCooldown + PlayerPrefs.GetInt("LastSpinTime") - currentTime);
                print("remainingTimeToNextSpin: " + remainingTimeToNextSpin);
                spinButtonNotreadyLabel.text = "СЛЕДУЮЩАЯ ПОПЫТКА\n" + TimeSpan.FromSeconds(remainingTimeToNextSpin).ToString(@"hh\:mm\:ss");
            }
        }

        void Start()
        {

        }

        void Update()
        {
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    SpinTheWheel();
                }
            }
        }

        public WheelItemData GetItemDataByType(WheelItemTypes itemType)
        {
            WheelItemData wid = new();

            wid.wheelItemIcon = wheelItemDatas[(int)itemType].wheelItemIcon;
            wid.wheelItemValue = wheelItemDatas[(int)itemType].wheelItemValue;
            wid.wheelItemDescription = wheelItemDatas[(int)itemType].wheelItemDescription;

            return wid;
        }


        public void SpinTheWheel()
        {
            if (!canSpinTheWheel)
                return;            
            canSpinTheWheel = false;

            SfxPlayer.instance.PlaySfx(10);

            PlayerPrefs.SetInt("LastSpinTime", (int)(DateTime.UtcNow - baseTime).TotalSeconds);
            
            UpdateSpinButton();

            wheelAnimator.enabled = true;
            wheelAnimator.Play("WheelFakeRotation", -1, 0f);

            //Wheel spin sfx
            StartCoroutine(PlayWheelSpinSfxCo());

            wheelIndicatorAnimator.speed = 1f;
            wheelIndicatorAnimator.Play("WheelIndicatorPivot");

            //Determine the spin result
            spinResult = UnityEngine.Random.Range(0, totalWheelItems);
            //spinResult = 0;   //Easy debug
            print("SpinResult: " + spinResult);

            //Save the selected item data in a separate variable for easy access
            int itemType = (int)wheelItems[(int)spinResult].GetComponent<WofItemController>().itemType;
            selectedItemData = new WheelItemData();
            selectedItemData.wheelItemIcon = wheelItemDatas[itemType].wheelItemIcon;
            selectedItemData.wheelItemValue = wheelItemDatas[itemType].wheelItemValue;
            selectedItemData.wheelItemDescription = wheelItemDatas[itemType].wheelItemDescription;

            Invoke(nameof(StopTheWheel), 2f);
        }

        public void StopTheWheel()
        {
            float targetAngle = spinResult * (360f / totalWheelItems) * -1;
            print("targetAngle: " + targetAngle);

            StartCoroutine(StopTheWheelCo(0, targetAngle));
        }

        public IEnumerator StopTheWheelCo(float from, float to)
        {
            float indicatorFinalSpeed = Mathf.Abs(to - 1) / 360f;
            float wheelFinalSpeed = 1f;

            //Important check #1
            if (indicatorFinalSpeed < 0.15f) 
                indicatorFinalSpeed = 0.15f;

            //Important check #2
            if (to == 0)
            {
                indicatorFinalSpeed = 0;
                wheelFinalSpeed = 10f;
            }

            //Apply the altered final speed values
            wheelIndicatorAnimator.speed = indicatorFinalSpeed;
            wheelAnimator.enabled = false;

            //Wheel spin sfx
            StartCoroutine(PlayFinalWheelSpinSfxCo(indicatorFinalSpeed));

            float t = 0;
            while(t < 1)
            {
                t += Time.deltaTime * 1f * wheelFinalSpeed;
                wheelBody.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(from, to, t));
                yield return null;
            }

            if(t >= 1)
            {
                wheelIndicatorAnimator.Play("Idle");       
                DisplaySpinResult();
            }
        }


        public void DisplaySpinResult()
        {
            canSpinTheWheel = true;

            ResultPanel.SetActive(true);
            ResultPanel.GetComponent<Animator>().Play("PopIn");

            resultIconUI.sprite = selectedItemData.wheelItemIcon;
            resultValueUI.text = "x" + selectedItemData.wheelItemValue;
            resultDescriptionUI.text = "" + selectedItemData.wheelItemDescription;

            //Sfx
            SfxPlayer.instance.PlaySfx(13);

            //Grant the prize
            int savedAmount = 0;
            int resultType = (int)wheelItems[(int)spinResult].GetComponent<WofItemController>().itemType;
            switch (resultType)
            {
                case 0:
                    savedAmount = PlayerPrefs.GetInt("AvailableBoosterByID0", 0);
                    PlayerPrefs.SetInt("AvailableBoosterByID0", savedAmount + wheelItemDatas[resultType].wheelItemValue);
                    break;

                case 1:
                    savedAmount = PlayerPrefs.GetInt("AvailableBoosterByID1", 0);
                    PlayerPrefs.SetInt("AvailableBoosterByID1", savedAmount + wheelItemDatas[resultType].wheelItemValue);
                    break;

                case 2:
                    savedAmount = PlayerPrefs.GetInt("AvailableBoosterByID2", 0);
                    PlayerPrefs.SetInt("AvailableBoosterByID2", savedAmount + wheelItemDatas[resultType].wheelItemValue);
                    break;

                case 3:
                    savedAmount = PlayerPrefs.GetInt("AvailableBoosterByID3", 0);
                    PlayerPrefs.SetInt("AvailableBoosterByID3", savedAmount + wheelItemDatas[resultType].wheelItemValue);
                    break;

                case 4:
                    savedAmount = PlayerPrefs.GetInt("PlayerCoins", 0);
                    PlayerPrefs.SetInt("PlayerCoins", savedAmount + wheelItemDatas[resultType].wheelItemValue);
                    break;

                case 5:
                    savedAmount = PlayerPrefs.GetInt("PlayerCoins", 0);
                    PlayerPrefs.SetInt("PlayerCoins", savedAmount + wheelItemDatas[resultType].wheelItemValue);
                    break;

                case 6:
                    savedAmount = PlayerPrefs.GetInt("PlayerCoins", 0);
                    PlayerPrefs.SetInt("PlayerCoins", savedAmount + wheelItemDatas[resultType].wheelItemValue);
                    break;

                case 7:
                    //No prize! - Seems like a bad day :))
                    break;
            }

            //Debug
            print("Prize detail: " + wheelItems[(int)spinResult].GetComponent<WofItemController>().itemType + " / " + (savedAmount + wheelItemDatas[resultType].wheelItemValue));
        }

        public void CloseWheelResultPanel()
        {
            SfxPlayer.instance.PlaySfx(10);

            ResultPanel.GetComponent<Animator>().Play("PopOut");
            Invoke(nameof(CloseWheelResultPanelDelayed), 0.5f);
        }

        public void CloseWheelResultPanelDelayed()
        {
            ResultPanel.SetActive(false);
        }


        public IEnumerator PlayWheelSpinSfxCo()
        {
            for(int i = 0; i < 20; i++)
            {
                SfxPlayer.instance.PlaySfx(2);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public IEnumerator PlayFinalWheelSpinSfxCo(float extraSpinData)
        {
            for (int i = 0; i < (int)(extraSpinData * 10f); i++)
            {                
                yield return new WaitForSeconds(1f / ((extraSpinData + 0.001f) * 10f));
                SfxPlayer.instance.PlaySfx(2);
            }
        }
    }
}
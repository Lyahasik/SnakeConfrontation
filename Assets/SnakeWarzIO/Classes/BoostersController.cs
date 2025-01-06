using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// As of update v1.2, we have 4 default boosters in the game:
/// 1. Unzoom (zooming out to have a bigger view of the game)
/// 2. Magnet (increases the radius of player collider for picking up foods)
/// 3. Score Multiplier (each food gives 2X score)
/// 4. Extra Speed (Snake moves 50% faster without any penalty)
/// 
/// Boosters are considered a great incentive for players to wander through the scene and find them for an easier game or aiming for a bigger score.
/// 
/// There are two ways to consume a booster: 
/// 1. Picking up a booster item (prefab) that is scattered on the game scene.
/// 2. Obtain new boosters through various ingame activity (daily login, wheel of fortune, etc) or purchase them through the shop with ingame currency.
/// </summary>

namespace SnakeWarzIO
{
    public class BoostersController : MonoBehaviour
    {
        public static BoostersController instance;
        public static bool canUseBoosters;              //For the times we should not let players to use boosters, we can utilize this flag.

        public enum BoosterTypes
        {
            Unzoom = 0,
            Magnet = 1,
            ScoreMultiplier = 2,
            ExtraSpeed = 3
        }

        [Header("Available Booster Prefabs")]
        public GameObject[] availableBoosterPrefabs;

        [Header("Available InGame Booster Buttons")]
        public GameObject[] availableIngameBoosterButtons;

        [Header("Boosters - UI Elements")]
        public GameObject ingameBoostersParent;

        //Static Boosters State & Settings

        [Header("Unzoom Booster")]
        public static bool isUnzoomBoosterEnabled;
        public static float unzoomBoosterMaxTime = 10;
        public static float unzoomBoosterRemainingTime;

        [Header("Magnet Booster")]
        public static bool isMagnetBoosterEnabled;
        public static float magnetBoosterMaxTime = 10;
        public static float magnetBoosterRemainingTime;

        [Header("Score Multiplier Booster")]
        public const int scoreMultiplier = 2; //2X
        public static bool isScoreMultiplierBoosterEnabled;
        public static float scoreMultiplierBoosterMaxTime = 10;
        public static float scoreMultiplierBoosterRemainingTime;

        [Header("Extra Speed Booster")]
        public const float extraSpeedAmount = 1.5f; //1.5X
        public static bool isExtraSpeedBoosterEnabled;
        public static float extraSpeedBoosterMaxTime = 10;
        public static float extraSpeedBoosterRemainingTime;

        //Delegates
        public delegate void BoosterAmountUpdateAction();
        public static event BoosterAmountUpdateAction OnBoosterAmountUpdateAction;

        private void Awake()
        {
            instance = this;
            canUseBoosters = true;
            isUnzoomBoosterEnabled = false;
            isMagnetBoosterEnabled = false;
            isScoreMultiplierBoosterEnabled = false;
            isExtraSpeedBoosterEnabled = false;
        }

        void Start()
        {
            
        }

        public void DisplayInitialBoostersAtStart()
        {
            ingameBoostersParent.SetActive(true);
        }

        void Update()
        {
            //Debug
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    IncreaseBoosterAmountByType(0, 3);
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    IncreaseBoosterAmountByType(1, 3);
                }

                if (Input.GetKeyDown(KeyCode.O))
                {
                    IncreaseBoosterAmountByType(2, 3);
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    IncreaseBoosterAmountByType(3, 3);
                }
            }
        }


        public void IncreaseBoosterAmountByType(int boosterID, int amount = 1)
        {
            PlayerPrefs.SetInt($"AvailableBoosterByID{(int)boosterID}", amount);
            print($"BoosterID #{boosterID} amount increased by {amount}");

            OnBoosterAmountUpdateAction?.Invoke();
        }

        /// <summary>
        /// This is where we activate the booster effect in the game.
        /// </summary>
        /// <param name="boosterID"></param>
        public void ActivateBoosterIngame(int boosterID)
        {
            //Debug
            //boosterID = 1;

            print($"Booster [#{boosterID}] is activated ==> {availableIngameBoosterButtons[boosterID].GetComponent<BoosterButtonController>().boosterID}");

            switch (boosterID)
            {
                case 0:
                    UnzoomGameCamera();
                    break;

                case 1:
                    EnableMagnetEffect();
                    break;

                case 2:
                    EnableScoreMultiplierBooster();
                    break;

                case 3:
                    EnableExtraSpeedBooster();
                    break;
            }
        }

        #region ExtraSpeedBooster
        public void EnableExtraSpeedBooster()
        {
            if (isExtraSpeedBoosterEnabled)
            {
                extraSpeedBoosterRemainingTime = extraSpeedBoosterMaxTime;
                //print("===> extra speed booster time is restored to max!");
                return;
            }

            extraSpeedBoosterRemainingTime = extraSpeedBoosterMaxTime;
            isExtraSpeedBoosterEnabled = true;
            StartCoroutine(EnableExtraSpeedBoosterCo());
        }

        public IEnumerator EnableExtraSpeedBoosterCo()
        {
            while (extraSpeedBoosterRemainingTime > 0)
            {
                extraSpeedBoosterRemainingTime -= Time.deltaTime;
                //print("extraSpeedBoosterRemainingTime: " + extraSpeedBoosterRemainingTime);
                availableIngameBoosterButtons[3].GetComponent<BoosterButtonController>().RunTimerUI(extraSpeedBoosterRemainingTime, extraSpeedBoosterMaxTime);
                yield return 0;
            }

            if (extraSpeedBoosterRemainingTime <= 0)
            {
                //print("Extra speed booster is finished!");
                isExtraSpeedBoosterEnabled = false;
            }
        }

        public float GetExtraSpeed()
        {
            if (isExtraSpeedBoosterEnabled)
                return extraSpeedAmount;
            else
                return 1;
        }
        #endregion


        #region ScoreMultiplierBooster
        public void EnableScoreMultiplierBooster()
        {
            if (isScoreMultiplierBoosterEnabled)
            {
                scoreMultiplierBoosterRemainingTime = scoreMultiplierBoosterMaxTime;
                //print("===> score booster time is restored to max!");
                return;
            }

            scoreMultiplierBoosterRemainingTime = scoreMultiplierBoosterMaxTime;
            isScoreMultiplierBoosterEnabled = true;
            StartCoroutine(EnableScoreMultiplierBoosterCo());
        }

        public IEnumerator EnableScoreMultiplierBoosterCo()
        {
            while (scoreMultiplierBoosterRemainingTime > 0)
            {
                scoreMultiplierBoosterRemainingTime -= Time.deltaTime;
                //print("scoreMultiplierBoosterRemainingTime: " + scoreMultiplierBoosterRemainingTime);
                availableIngameBoosterButtons[2].GetComponent<BoosterButtonController>().RunTimerUI(scoreMultiplierBoosterRemainingTime, scoreMultiplierBoosterMaxTime);
                yield return 0;
            }

            if (scoreMultiplierBoosterRemainingTime <= 0)
            {
                //print("Score booster is finished!");
                isScoreMultiplierBoosterEnabled = false;
            }
        }

        public int GetScoreMultiplier()
        {
            if (isScoreMultiplierBoosterEnabled)
                return scoreMultiplier;     //Multiplied
            else
                return 1;                   //Normal
        }
        #endregion


        #region MagnetBooster
        public void EnableMagnetEffect()
        {
            if (isMagnetBoosterEnabled)
            {
                magnetBoosterRemainingTime = magnetBoosterMaxTime;
                //print("===> magnet booster time is restored to max!");
                return;
            }

            magnetBoosterRemainingTime = magnetBoosterMaxTime;
            isMagnetBoosterEnabled = true;
            StartCoroutine(EnableMagnetEffectCo());
        }

        public IEnumerator EnableMagnetEffectCo()
        {
            GameController.mainPlayerObject.GetComponent<Snake>().EnableMagnetCircle(true);

            while (magnetBoosterRemainingTime > 0)
            {
                magnetBoosterRemainingTime -= Time.deltaTime;
                //print("magnetBoosterRemainingTime: " + magnetBoosterRemainingTime);
                availableIngameBoosterButtons[1].GetComponent<BoosterButtonController>().RunTimerUI(magnetBoosterRemainingTime, magnetBoosterMaxTime);
                yield return 0;
            }

            if (magnetBoosterRemainingTime <= 0)
            {
                //print("Magnet booster is finished!");
                if(GameController.mainPlayerObject)
                    GameController.mainPlayerObject.GetComponent<Snake>().EnableMagnetCircle(false);
                isMagnetBoosterEnabled = false;
            }
        }
        #endregion


        #region UnzoomBooster
        public void UnzoomGameCamera()
        {
            if (isUnzoomBoosterEnabled)
            {
                unzoomBoosterRemainingTime = unzoomBoosterMaxTime;
                //print("===> Unzoom booster time is restored to max!");
                return;
            }

            unzoomBoosterRemainingTime = unzoomBoosterMaxTime;
            isUnzoomBoosterEnabled = true;
            StartCoroutine(PerformUnzoomCo());
        }

        public IEnumerator PerformUnzoomCo()
        {
            CameraController.instance.StartUnzoom();

            while(unzoomBoosterRemainingTime > 0)
            {
                unzoomBoosterRemainingTime -= Time.deltaTime;
                //print("unzoomBoosterRemainingTime: " + unzoomBoosterRemainingTime);
                availableIngameBoosterButtons[0].GetComponent<BoosterButtonController>().RunTimerUI(unzoomBoosterRemainingTime, unzoomBoosterMaxTime);
                yield return 0;
            }

            if(unzoomBoosterRemainingTime <= 0)
            {
                //print("Unzoom booster is finished!");
                canUseBoosters = false;
                CameraController.instance.EndUnzoom();
                Invoke(nameof(EndUnzoomBoosterFlag), 2f);
            }
        }

        public void EndUnzoomBoosterFlag()
        {
            isUnzoomBoosterEnabled = false;
            canUseBoosters = true;
        }
        #endregion

    }
}
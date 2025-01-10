using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A lightweight XP manager system that handles player leveling and XP progression
/// </summary>

namespace SnakeWarzIO
{
    public class XpManager : MonoBehaviour
    {
        public static XpManager instance;
        public static int baseXpReward = 50;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            // TODO Used for easy debug & test on editor
            // if (Application.isEditor)
            // {
            //     if (Input.GetKeyDown(KeyCode.KeypadPlus))
            //     {
            //         int currentXp = PlayerPrefs.GetInt("PlayerCurrentXP");
            //         PlayerPrefs.SetInt("PlayerCurrentXP", currentXp + 150);
            //         print("PlayerCurrentXP: " + PlayerPrefs.GetInt("PlayerCurrentXP"));
            //         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //     }
            //
            //     if (Input.GetKeyDown(KeyCode.KeypadMinus))
            //     {
            //         int currentXp = PlayerPrefs.GetInt("PlayerCurrentXP");
            //         PlayerPrefs.SetInt("PlayerCurrentXP", currentXp - 150);
            //         print("PlayerCurrentXP: " + PlayerPrefs.GetInt("PlayerCurrentXP"));
            //         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //     }
            //
            //     if (Input.GetKeyDown(KeyCode.R))
            //     {
            //         PlayerPrefs.SetInt("PlayerCurrentXP", 0);
            //         print("PlayerCurrentXP: " + PlayerPrefs.GetInt("PlayerCurrentXP"));
            //         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //     }
            // }
        }

        /// <summary>
        /// Translate the total XP into level
        /// </summary>
        /// <param name="currentXP"></param>
        /// <returns></returns>
        public int GetLevelByXP(int currentXP)
        {
            int result = 1;

            if (currentXP < 200)
                result = 1;
            else if (currentXP >= 200 && currentXP < 500)
                result = 2;
            else if (currentXP >= 500 && currentXP < 1100)
                result = 3;
            else if (currentXP >= 1100 && currentXP < 2300)
                result = 4;
            else if (currentXP >= 2300 && currentXP < 4700)
                result = 5;
            else if (currentXP >= 4700 && currentXP < 9500)
                result = 6;
            else if (currentXP >= 9500 && currentXP < 19100)
                result = 7;
            else if (currentXP >= 19100 && currentXP < 38300)
                result = 8;
            else if (currentXP >= 38300 && currentXP < 76700)
                result = 9;
            else if (currentXP >= 76700)
                result = 10;

            return result;
        }

        /// <summary>
        /// Calculate the amount of XP needed to progress to the next level
        /// </summary>
        /// <param name="currentXP"></param>
        /// <returns></returns>
        public int GetNextLevelXP(int currentXP)
        {
            int result = 200;

            if (currentXP < 200)
                result = 200;
            else if (currentXP >= 200 && currentXP < 500)
                result = 500;
            else if (currentXP >= 500 && currentXP < 1100)
                result = 1100;
            else if (currentXP >= 1100 && currentXP < 2300)
                result = 2300;
            else if (currentXP >= 2300 && currentXP < 4700)
                result = 4700;
            else if (currentXP >= 4700 && currentXP < 9500)
                result = 9500;
            else if (currentXP >= 9500 && currentXP < 19100)
                result = 19100;
            else if (currentXP >= 19100 && currentXP < 38300)
                result = 38300;
            else if (currentXP >= 38300 && currentXP < 76700)
                result = 76700;
            else if (currentXP >= 76700)
                result = 1000000;

            return result;
        }
    }
}
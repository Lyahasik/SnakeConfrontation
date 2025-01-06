using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeWarzIO
{
    public class InitManager : MonoBehaviour
    {
        void Awake()
        {
            //Run required inits here
            Time.timeScale = 1f;

            //PlayerPrefs.DeleteAll();
            //PlayerPrefs.SetInt("PlayerCoins", 5000);

            //If we want to give players some free boosters on the first run
            if (!PlayerPrefs.HasKey("ReceivedFreeBoosters"))
            {
                //Save the flag
                PlayerPrefs.SetInt("ReceivedFreeBoosters", 1);

                //Charge up player boosters
                PlayerPrefs.SetInt("AvailableBoosterByID0", 3);
                PlayerPrefs.SetInt("AvailableBoosterByID1", 3);
                PlayerPrefs.SetInt("AvailableBoosterByID2", 3);
                PlayerPrefs.SetInt("AvailableBoosterByID3", 3);
            }
        }

        private void Start()
        {
            //Load the next scene
            SceneManager.LoadScene("Menu");
        }
    }
}
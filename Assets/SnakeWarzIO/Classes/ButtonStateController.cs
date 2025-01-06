using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle the state of sound & music buttons to make sure they are always displaying the proper status.
/// </summary>

namespace SnakeWarzIO
{
    public class ButtonStateController : MonoBehaviour
    {
        public Sprite[] availableStates;
        public GameObject buttonImage;
        private Image r;
        private int currentState;
        public string prefsCode = "";

        void Start()
        {
            currentState = PlayerPrefs.GetInt(prefsCode, 1);
            r = buttonImage.GetComponent<Image>();
            r.sprite = availableStates[currentState];
        }

        public void ChangeButtonState()
        {
            if (currentState == 0)
                currentState = 1;
            else
                currentState = 0;

            PlayerPrefs.SetInt(prefsCode, currentState);
            r.sprite = availableStates[currentState];
        }

        public void ChangeSoundState()
        {
            MusicPlayer.instance.ToggleSound();
        }

        public void ChangeMusicState()
        {
            MusicPlayer.instance.ToggleMusic();
        }
    }
}
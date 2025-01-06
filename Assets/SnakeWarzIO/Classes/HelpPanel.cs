using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// In this game, we are using a slide-based help panel where player can go back & forth through available guides.
/// If player is playing for the first time, the help panel will always show automatically.
/// </summary>

namespace SnakeWarzIO
{
    public class HelpPanel : MonoBehaviour
    {
        public GameObject masterSlideHolder;
        public Text pageNumberUI;
        public GameObject closeButton;
        private int totalSlides = 4;
        private int currentSlideNumber;
        private bool canSlide;

        private void Awake()
        {
            currentSlideNumber = 1;
            canSlide = true;
        }

        void OnEnable()
        {
            if (PlayerPrefs.GetInt("TutorialIsShown") == 1)
                closeButton.SetActive(true);
            else
                closeButton.SetActive(false);
        }

        public void NextSlide()
        {
            SfxPlayer.instance.PlaySfx(10);

            if (currentSlideNumber == 4)
                return;

            MoveSlides(-1);
        }

        public void PreviousSlide()
        {
            SfxPlayer.instance.PlaySfx(10);

            if (currentSlideNumber == 1)
                return;

            MoveSlides(1);
        }

        public void MoveSlides(int dir)
        {
            if (!canSlide)
                return;
            canSlide = false;

            currentSlideNumber += dir * -1;

            if (currentSlideNumber == totalSlides)
                closeButton.SetActive(true);

            pageNumberUI.text = "" + currentSlideNumber + "/" + totalSlides;
            StartCoroutine(MoveSlidesCo(dir));
        }

        public IEnumerator MoveSlidesCo(int dir)
        {
            Vector3 currentPos = masterSlideHolder.GetComponent<RectTransform>().anchoredPosition;
            Vector3 targetPos = currentPos + new Vector3(2000 * dir, 0, 0);

            print("dir: " + dir);
            print("currentPos: " + currentPos);
            print("targetPos: " + targetPos);

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * 4f;
                masterSlideHolder.GetComponent<RectTransform>().anchoredPosition = new Vector3(
                    Mathf.SmoothStep(currentPos.x, targetPos.x, t),
                    0,
                    0);
                yield return 0;
            }

            if (t >= 1)
                canSlide = true;
        }
    }
}
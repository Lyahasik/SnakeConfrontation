using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// We want to display a graceful loading panel while game is spawning the initial items during the first few seconds. 
/// This helps to hide any possible lags that may be caused by the instantiaion process.
/// </summary>

namespace SnakeWarzIO
{
    public class IntroLoadingController : MonoBehaviour
    {
        public static IntroLoadingController instance;

        public GameObject loadingPanel;
        private Animator lpAnimator;

        private void Awake()
        {
            instance = this;
            loadingPanel.SetActive(true);
            lpAnimator = loadingPanel.GetComponent<Animator>();
        }

        void Start()
        {
            StartCoroutine(HideLoadingPanel());
        }

        public IEnumerator HideLoadingPanel()
        {
            yield return new WaitForSeconds(GameController.loadingDelay);
            lpAnimator.Play("HideLoadingPanel");
            yield return new WaitForSeconds(0.5f);
            loadingPanel.SetActive(false);

            GameController.instance.StartTheGame();
        }
    }
}
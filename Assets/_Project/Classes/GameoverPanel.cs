using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles showing game result and other useful data on gameover panel once the game is ended.
/// </summary>

namespace SnakeWarzIO
{
    public class GameoverPanel : MonoBehaviour
    {
        public static GameoverPanel instance;

        [Header("UI Objects")]
        public GameObject normalScoreGroup;
        public TMP_Text rankUI;
        public TMP_Text scoreUI;
        public GameObject newBestUI;
        public TMP_Text currentLevelUI;
        public TMP_Text xpUI;
        public TMP_Text prizeCoinUI;
        public Image xpBarUI;

        //Game modes
        //Battle royale
        public GameObject battleRoyaleGroup;
        public TMP_Text battleRoyaleRankUI;


        private void Awake()
        {
            instance = this;
            newBestUI.SetActive(false);
            normalScoreGroup.SetActive(true);
            battleRoyaleGroup.SetActive(false);
        }

        void Start()
        {
            UpdatePanelData();
        }

        public void UpdatePanelData()
        {
            //override gameover panel setup based on the selected game mode
            //If battle royale...
            if(GameController.gameModeID == 4)
            {
                normalScoreGroup.SetActive(false);
                battleRoyaleGroup.SetActive(true);

                battleRoyaleRankUI.text = "" + (BotSpawner.instance.GetAvailableBotsCount() + 1);
            }

            rankUI.text = "" + IngameLeaderboardManager.instance.GetPlayerRank();
            scoreUI.text = "" + GameController.playerScore;

            //Save best score if needed
            int lastSavedBestScore = PlayerPrefs.GetInt("SavedBestScore", 0);
            print("CurrentScore/SavedBestScore: " + GameController.playerScore + "/" + lastSavedBestScore);
            if (GameController.playerScore > lastSavedBestScore)
            {
                PlayerPrefs.SetInt("SavedBestScore", GameController.playerScore);
                StartCoroutine(DisplayMewBestDelayedCo());
            }

            //Prize coin
            int rankScore = 1;
            int currentRank = IngameLeaderboardManager.instance.GetPlayerRank();
            rankScore = Mathf.Max(1, (4 - currentRank));
            int prizeCoins = (int)((GameController.playerScore * 1.7f) * rankScore);
            prizeCoinUI.text = "" + prizeCoins;
            int playerCoins = PlayerPrefs.GetInt("PlayerCoins");
            PlayerPrefs.SetInt("PlayerCoins", playerCoins + prizeCoins);
            print("Prize coin saved! ===> " + playerCoins + " + " + prizeCoins);

            //XP & levels
            int receivedXP = 0;
            if (currentRank < 10)
                receivedXP = XpManager.baseXpReward;
            else
                receivedXP = XpManager.baseXpReward + (Mathf.Max(1, (11 - currentRank)) * 10);

            //Save XP
            int savedXP = PlayerPrefs.GetInt("PlayerCurrentXP", 0);
            int currentXP = savedXP + receivedXP;            
            PlayerPrefs.SetInt("PlayerCurrentXP", currentXP);
            
            currentLevelUI.text = "" + XpManager.instance.GetLevelByXP(currentXP);
            xpUI.text = "" + currentXP + "/" + XpManager.instance.GetNextLevelXP(currentXP);
            xpBarUI.fillAmount = (float)currentXP / (float)XpManager.instance.GetNextLevelXP(currentXP);
        }

        public IEnumerator DisplayMewBestDelayedCo()
        {
            yield return new WaitForSeconds(1.5f);
            newBestUI.SetActive(true);
        }
    }
}
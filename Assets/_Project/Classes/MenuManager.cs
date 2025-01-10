using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Display player data on Menu scene, and handling player interactions in this scene.
/// </summary>

namespace SnakeWarzIO
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance;
        private bool canFastStart;

        [Header("Player Data")]
        public TMP_Text playerNameUI;
        public TMP_InputField playerNameInputfieldUI;
        public TMP_Text playerMoneyUI;
        public TMP_Text playerBestScoreUI;
        public TMP_Text playerXPUI;
        public TMP_Text playerLevelUI;
        public Image xpBarUI;

        [Header("Audio Settings")]        
        public GameObject audioButtons;
        private Animator abAnim;
        private bool isAudioButtonsVisible;

        [Header("Settings Panel")]
        public GameObject settingsPanel;
        public GameObject[] availableSubPanels;
        public Button[] settingTabButtons;
        public Sprite[] availableTabSprites;

        [Header("Wheel-Of-Fortune Panel")]
        public GameObject wheelPanel;
        private Animator wheelAnim;

        [Header("Player Profile Panel")]
        public GameObject profilePanel;
        private Animator profileAnim;
        public TMP_Text ppGamesPlayedUI;
        public TMP_Text ppTotalPlayTimeUI;
        public TMP_Text ppKillsUI;
        public TMP_Text ppBestScoreUI;
        public TMP_Text ppCollectedFoodUI;

        [Header("Mode Selection Panel")]
        public GameObject modeSelectionPanel;

        private int currentXP;
        private int nextLevelXP;
        private int currentLevel;
        private string playerName;


        private void Awake()
        {
            instance = this;
            canFastStart = true;
            Time.timeScale = 1f;
            Application.targetFrameRate = 60;

            abAnim = audioButtons.GetComponent<Animator>();
            wheelAnim = wheelPanel.GetComponent<Animator>();
            profileAnim = profilePanel.GetComponent<Animator>();
            settingsPanel.SetActive(false);
            audioButtons.SetActive(false);
            wheelPanel.SetActive(false);
            profilePanel.SetActive(false);
            modeSelectionPanel.SetActive(false);

            playerName = PlayerPrefs.GetString("PlayerName", "You");
            //print($"playerName: {playerName}");
        }

        void Start()
        {
            currentXP = PlayerPrefs.GetInt("PlayerCurrentXP", 0);
            currentLevel = XpManager.instance.GetLevelByXP(currentXP);
            nextLevelXP = XpManager.instance.GetNextLevelXP(currentXP);

            playerNameUI.text = "" + playerName;
            playerNameInputfieldUI.text = "" + playerName;

            playerMoneyUI.text = "" + PlayerPrefs.GetInt("PlayerCoins", 0);
            playerBestScoreUI.text = "" + PlayerPrefs.GetInt("SavedBestScore", 0);
            playerXPUI.text = "" + currentXP + "/" + nextLevelXP;
            playerLevelUI.text = "" + currentLevel;

            xpBarUI.fillAmount = (float)currentXP / (float)nextLevelXP;
        }

        void Update()
        {
            //If we want to quickly start the game by pressing space key
            if (Input.GetKeyDown(KeyCode.Space) && canFastStart)
            {
                canFastStart = false;
                LoadGame();
            }
        }

        /// <summary>
        /// Start the game
        /// </summary>
        public void LoadGame(int gameModeID = 0)
        {
            SfxPlayer.instance.PlaySfx(10);

            //Force show tutorial if needed!
            if (PlayerPrefs.GetInt("TutorialIsShown") == 0)
            {
                PlayerPrefs.SetInt("AutoplayTheGameAfterTutorial", 1);

                DisplayHelpPanel();
                return;
            }

            StartCoroutine(LoadGameCo(gameModeID));
        }

        public IEnumerator LoadGameCo(int gameModeID = 0)
        {
            //Set proper game mode
            PlayerPrefs.SetInt("GameModeID", gameModeID);

            //Wait a bit
            yield return new WaitForSeconds(0.3f);

            //Load the game
            SceneManager.LoadScene("Game");
        }


        /// <summary>
        /// Display help panel
        /// </summary>
        public void DisplayHelpPanel()
        {
            SfxPlayer.instance.PlaySfx(10);

            //Save the view flag
            PlayerPrefs.SetInt("TutorialIsShown", 1);
        }

        public void HideHelpPanel()
        {
            SfxPlayer.instance.PlaySfx(10);
            StartCoroutine(HideHelpPanelCo());
        }

        public IEnumerator HideHelpPanelCo()
        {
            yield return new WaitForSeconds(0.5f);

            //Auto start the game when tutorial is being forced for the first time
            if(PlayerPrefs.GetInt("AutoplayTheGameAfterTutorial") == 1)
            {
                PlayerPrefs.SetInt("AutoplayTheGameAfterTutorial", 0);
                StartCoroutine(LoadGameCo());
            }
        }


        /// <summary>
        /// Open settings sub-panel
        /// </summary>
        public void ClickOnAudioSettingButton()
        {
            SfxPlayer.instance.PlaySfx(10);

            if (isAudioButtonsVisible)
                HideAudioButtons();
            else
                DisplayAudioButtons();
        }

        public void DisplayAudioButtons()
        {
            audioButtons.SetActive(true);
            isAudioButtonsVisible = true;
            abAnim.Play("AudioButtonsIn");
        }

        public void HideAudioButtons()
        {
            abAnim.Play("AudioButtonsOut");
            StartCoroutine(HideAudioButtonsCo());
        }

        public IEnumerator HideAudioButtonsCo()
        {
            yield return new WaitForSeconds(0.5f);
            isAudioButtonsVisible = false;
            audioButtons.SetActive(false);
        }


        /// <summary>
        /// Load skin selection (aka shop) scene
        /// </summary>
        public void OpenSkinPanel()
        {
            SceneManager.LoadScene("Skins");
        }


        public void UpdatePlayernameOnUI()
        {
            playerName = PlayerPrefs.GetString("PlayerName", "You");

            playerNameUI.text = "" + playerName;
            playerNameInputfieldUI.text = "" + playerName;
        }


        public void OnEditUsernameUI(string value)
        {
            print("Editing username: " + value);

        }

        public void OnFinishedEditingUsernameUI(string value)
        {
            print("FINISHED Editing username: " + value);

            playerName = value;
            PlayerPrefs.SetString("PlayerName", playerName);

            print("New playername saved! ==> " + PlayerPrefs.GetString("PlayerName"));

            UpdatePlayernameOnUI();
        }


        public void DisplaySettingsPanel()
        {
            SfxPlayer.instance.PlaySfx(10);

            settingsPanel.SetActive(true);
            settingsPanel.GetComponent<Animator>().Play("SettingsPanelPopIn");

            //Init the status of all sub-panels
            //Always start with "Foods" sub-panel as the active one.
            SelectSettingsPanelByTabID(0);

            //Update tabs status (first tab item is always selected by default)
            foreach (Button b in settingTabButtons)
            {
                b.GetComponent<Image>().sprite = availableTabSprites[1];
            }
            settingTabButtons[0].GetComponent<Image>().sprite = availableTabSprites[0];           
        }

        public void CloseSettingsPanel()
        {
            SfxPlayer.instance.PlaySfx(10);

            settingsPanel.GetComponent<Animator>().Play("SettingsPanelPopOut");
            Invoke(nameof(CloseSettingsPanelDelayed), 0.5f);
        }

        public void CloseSettingsPanelDelayed()
        {
            settingsPanel.SetActive(false);
        }


        public void SelectSettingsPanelByTabID(int tabID)
        {
            foreach (Button b in settingTabButtons)
                b.GetComponent<Image>().sprite = availableTabSprites[1];
            settingTabButtons[tabID].GetComponent<Image>().sprite = availableTabSprites[0];

            for(int i = 0; i < availableSubPanels.Length; i++)
            {
                if (tabID == i)
                    availableSubPanels[i].GetComponent<Animator>().Play("PopIn");
                else
                    availableSubPanels[i].GetComponent<Animator>().Play("PopOut");
            }
        }


        public void DisplayWheelOfFortunePanel()
        {
            SfxPlayer.instance.PlaySfx(10);

            wheelPanel.SetActive(true);
            wheelAnim.Play("WofPanelPopIn");
        }

        public void CloseWheelOfFortunePanel()
        {
            SfxPlayer.instance.PlaySfx(10);

            wheelAnim.Play("WofPanelPopOut");
            Invoke(nameof(CloseWheelOfFortunePanelDelayed), 0.5f);
        }

        public void CloseWheelOfFortunePanelDelayed()
        {
            wheelPanel.SetActive(false);
        }


        public void DisplayPlayerProfilePanel()
        {
            SfxPlayer.instance.PlaySfx(10);

            profilePanel.SetActive(true);
            profileAnim.Play("ProfilePopIn");

            //Populate data on player profile
            ppGamesPlayedUI.text = "" + PlayerPrefs.GetInt("GamesPlayed", 0).ToString("N0");
            int totalPlayTime = PlayerPrefs.GetInt("TotalPlayTime", 0);
            int hours = (int)(totalPlayTime / 3600f);
            int minutes = (int)((totalPlayTime % 3600) / 60f);
            int seconds = Mathf.FloorToInt(totalPlayTime % 60f);
            ppTotalPlayTimeUI.text = "" + string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            ppKillsUI.text = "" + PlayerPrefs.GetInt("TotalKills", 0).ToString("N0");
            ppBestScoreUI.text = "" + PlayerPrefs.GetInt("SavedBestScore", 0).ToString("N0");
            ppCollectedFoodUI.text = "" + PlayerPrefs.GetInt("CollectedFood", 0).ToString("N0");
        }

        public void ClosePlayerProfilePanel()
        {
            SfxPlayer.instance.PlaySfx(10);

            profileAnim.Play("ProfilePopOut");
            Invoke(nameof(ClosePlayerProfilePanelDelayed), 0.5f);
        }

        public void ClosePlayerProfilePanelDelayed()
        {
            profilePanel.SetActive(false);
        }

        public void OpenModeSelectionPanel()
        {
            SfxPlayer.instance.PlaySfx(10);
            modeSelectionPanel.SetActive(true);
            modeSelectionPanel.GetComponent<Animator>().Play("ModesPopIn");
        }

        public void CloseModeSelectionPanel()
        {
            SfxPlayer.instance.PlaySfx(10);
            modeSelectionPanel.GetComponent<Animator>().Play("ModesPopOut");
            Invoke(nameof(CloseModeSelectionPanelDelayed), 0.5f);
        }

        public void CloseModeSelectionPanelDelayed()
        {
            modeSelectionPanel.SetActive(false);
        }

    }
}
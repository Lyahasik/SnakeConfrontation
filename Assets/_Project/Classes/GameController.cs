using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// GameController class in charge of monitoring game timer, game score, global flags, control type, start/finish states, game boundary values and some global snake related parameters.
/// </summary>

namespace SnakeWarzIO
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        //Game modes
        public static int gameModeID;

        //Control Type
        // 0 = UI Joystick (Mobile/Touch)
        // 1 = Mouse (PC/Desktop)
        public static int controlType = 0;
        public static bool isRunningOnMobilePlatform;

        //UI
        public GameObject mainCanvas;
        public GameObject gameoverPanel;
        public GameObject scoreGroup;
        public GameObject eliminationGroup;
        public Text ingameScoreUI;
        public Text ingameTimerUI;
        public Text remainingEnemiesUI;
        public GameObject joystickController;

        //Game state
        public static bool isGameStarted;
        public static bool isGameFinished;

        //Loading (intro)
        public static float loadingDelay = 0.5f;

        //Spawn locations, borders & edge limits
        public static float minimumFieldX = -93;
        public static float minimumFieldY = -93;
        public static float maximumFieldX = 93;
        public static float maximumFieldY = 93;
        public static float moveSpeedMax = 8f;
        public static float moveSpeedBoostMax = 2.0f;
        public static float rotationSpeedBoostPenalty = 0.6f;

        //Snake related values
        public static int defaultSnakeCount = 25;                   //When the game begins, this many bot snakes will be created.
        public static int initialSnakeCount;
        public static int foodIntoBodypart = 3;                     //This much food is needed to be collected in order to grow a new bodypart unit
        public static int ghostfoodIntoBodypart = 1;                //This much ghost-food is needed to be collected in order to grow a new bodypart unit
        public static int defaultBodyParts = 5;                     //Each new snake object should contain this many bodyparts when created
        public static int initialBodyParts;
        public static int minimumSnakeBodyparts;                    //When we advance in game, we need to create snakes with more initial bodyparts. This variable holds the additional number
        public static float bodypartsFollowDelayNormal = 0.12f;     //Default = 0.09f
        public static float bodypartsFollowDelayBoost = 0.08f;      //Default = 0.08f
        public static int maxSnakeSizeForScale = 500;

        //Scoring
        public static int normalFoodScore = 1;
        public static int ghostFoodScore = 1;
        public static int playerScore;

        //Timer
        public static float totalGameTime = 120; //seconds
        public static float remainingGameTime;
        public static float timeSpentInGame;
        private int minutes;
        private int seconds;
        public bool[] timerWarningFlags;

        //Misc
        public static float maxDistanceToTriggerShake = 20f;
        public static GameObject mainPlayerObject;                      //Pointer to the snake that is under player's control
        public static bool speedBoostButtonIsPressed;
        public static bool canTurnDeadSnakesIntoGhostfood = true;       //If true, game will convert the bodyparts of each dead snake into ghost foods
        public static bool canSpawnNewFoodFromPickedupFoods = true;     //If true, game will spawn new foods when any available food item is picked up
        public static bool canRespawnDeadBots = true;
        public static int remainingEnemiesCount;

        /// <summary>
        /// Make sure to init everything here!
        /// </summary>
        private void Awake()
        {
            instance = this;
            gameoverPanel.SetActive(false);
            joystickController.SetActive(false);
            minimumSnakeBodyparts = 0;
            Application.targetFrameRate = 60;
            mainPlayerObject = null;
            speedBoostButtonIsPressed = false;
            //remainingGameTime = totalGameTime;
            Time.timeScale = 1f;

            //Default values
            initialBodyParts = defaultBodyParts;
            initialSnakeCount = defaultSnakeCount;
            FoodSpawner.maximumFoods = FoodSpawner.defaultAmountOfFoods;

            eliminationGroup.SetActive(false);
            scoreGroup.SetActive(true);

            isGameStarted = false;
            isGameFinished = false;

            //Stats
            int gamesPlayed = PlayerPrefs.GetInt("GamesPlayed");
            PlayerPrefs.SetInt("GamesPlayed", ++gamesPlayed);

            //Important - platform detection
            isRunningOnMobilePlatform = Application.isMobilePlatform;
            //print("<b>isRunningOnMobilePlatform: " + isRunningOnMobilePlatform + "</b>");

            //Automatic control type setting
            if (isRunningOnMobilePlatform)
                controlType = 0;
            else
                controlType = 1;

            //Get game mode
            gameModeID = PlayerPrefs.GetInt("GameModeID", 0);

            //Update or override game settings based on the selected game mode
            OverrideGameSettings();

            //Debug - force control type
            //controlType = 0;
        }

        /// <summary>
        /// As of v1.3, we are introducing new game modes. Each game mode offers a unique type of gameplay, and this helps to keep the game interesting for longer since players can engage in different missions.
        /// 
        /// Description of available game modes:
        /// 1. Quick play: The original game mode when all snakes start small & timer counts from the default value (120 seconds) to 0.
        /// 2. Infinity: There is no timer in this mode. You can play for unlimited amount of time and get as big as possible. All other setting is set to normal.
        /// 3. Boss Hunt: In this mode, all snakes start big. There is also no food on the pit, so every snake needs to hunt other snakes to become the final boss!
        /// 4. Ghetto: Foods are very scarce in “Ghetto” mode & they are not replenished as well. So, every snake needs to fight for every single food that is available.
        /// 5. Battle Royale: This is where you need to be the last man standing in order to win. Foods are limited and the pit is filled with snakes. You need to be every other snake and survive as the last one to achieve the #1 rank.
        /// </summary>
        public void OverrideGameSettings()
        {
            switch (gameModeID)
            {
                case 0:
                    //Quick Play
                    remainingGameTime = totalGameTime;
                    canTurnDeadSnakesIntoGhostfood = true;
                    canSpawnNewFoodFromPickedupFoods = true;
                    canRespawnDeadBots = true;
                    break;

                case 1:
                    //Infinity
                    timeSpentInGame = 0;
                    canTurnDeadSnakesIntoGhostfood = true;
                    canSpawnNewFoodFromPickedupFoods = true;
                    canRespawnDeadBots = true;
                    break;

                case 2:
                    //Boss Hunt
                    remainingGameTime = totalGameTime;
                    initialBodyParts = 100;
                    FoodSpawner.maximumFoods = 0;
                    canTurnDeadSnakesIntoGhostfood = false;
                    canSpawnNewFoodFromPickedupFoods = false;
                    canRespawnDeadBots = true;
                    break;

                case 3:
                    //Ghetto
                    remainingGameTime = totalGameTime;
                    FoodSpawner.maximumFoods = 300;
                    canTurnDeadSnakesIntoGhostfood = false;
                    canSpawnNewFoodFromPickedupFoods = false;
                    canRespawnDeadBots = true;
                    break;

                case 4:
                    //Elimination
                    timeSpentInGame = 0;
                    initialSnakeCount = 80;
                    FoodSpawner.maximumFoods = 500;
                    canTurnDeadSnakesIntoGhostfood = true;
                    canSpawnNewFoodFromPickedupFoods = false;
                    canRespawnDeadBots = false;
                    eliminationGroup.SetActive(true);
                    scoreGroup.SetActive(false);
                    remainingEnemiesCount = initialSnakeCount;
                    break;
            }
        }

        void Start()
        {
            InvokeRepeating(nameof(UpdateMinimumBodyparts), 1f, 1f);
            isGameStarted = true;
        }

        void Update()
        {
            //Update game score based on ingame events
            UpdateIngameScore();
            UpdateRemainingEnemiesCount();

            //Handle game timer based on different game modes
            HandleGameTimer();
            //RunGameTimer();

            //Debug
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    GameObject[] allBots = GameObject.FindGameObjectsWithTag("BotHead");
                    foreach (GameObject b in allBots)
                        b.GetComponent<BotController>().Die();
                }
            }

            //Debug only!
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                mainCanvas.SetActive(!mainCanvas.activeSelf);
            }
        }

        public void StartTheGame()
        {
            isGameStarted = true;
            joystickController.SetActive(true);
        }

        public void HandleGameTimer()
        {
            switch (gameModeID)
            {
                case 0:
                    RunNormalGameTimer();
                    break;

                case 1:
                    RunInfinityGameTimer();
                    break;

                case 2:
                    RunNormalGameTimer();
                    break;

                case 3:
                    RunNormalGameTimer();
                    break;

                case 4:
                    RunInfinityGameTimer();
                    break;
            }
        }

        /// <summary>
        /// This timer system is used when player wants to play the "Quick Play" mode. ie, playing for [default] 120 seconds.
        /// </summary>
        public void RunNormalGameTimer()
        {
            if (!isGameStarted || isGameFinished)
                return;

            remainingGameTime -= Time.deltaTime;

            //Timer warnings
            if (remainingGameTime <= 10 && !timerWarningFlags[0])
            {
                timerWarningFlags[0] = true;
                SfxPlayer.instance.PlaySfx(15);
            }
            else if (remainingGameTime <= 3 && !timerWarningFlags[1])
            {
                timerWarningFlags[1] = true;
                SfxPlayer.instance.PlaySfx(15);
            }
            else if (remainingGameTime <= 2 && !timerWarningFlags[2])
            {
                timerWarningFlags[2] = true;
                SfxPlayer.instance.PlaySfx(15);
            }
            else if (remainingGameTime <= 1 && !timerWarningFlags[3])
            {
                timerWarningFlags[3] = true;
                SfxPlayer.instance.PlaySfx(15);
            }

            if (remainingGameTime <= 0)
            {
                //Sfx
                SfxPlayer.instance.PlaySfx(16);

                //Create vfx
                FBParticleManager.instance.CreateParticle(7, mainPlayerObject.transform.position);

                //Game over
                isGameFinished = true;
                Time.timeScale = 0.1f;
                Gameover(0.3f);
                return;
            }

            seconds = (int)(remainingGameTime % 60);
            minutes = Mathf.FloorToInt(remainingGameTime / 60);

            ingameTimerUI.text = "" + minutes + ":" + seconds /*+ " -> " + remainingGameTime*/;
        }


        public void RunInfinityGameTimer()
        {
            if (!isGameStarted || isGameFinished)
                return;

            timeSpentInGame += Time.deltaTime;

            //Timer warnings
            if (timeSpentInGame >= 60 && !timerWarningFlags[0])
            {
                timerWarningFlags[0] = true;
                SfxPlayer.instance.PlaySfx(15);
            }
            else if (timeSpentInGame >= 120 && !timerWarningFlags[1])
            {
                timerWarningFlags[1] = true;
                SfxPlayer.instance.PlaySfx(15);
            }
            else if (timeSpentInGame >= 180 && !timerWarningFlags[2])
            {
                timerWarningFlags[2] = true;
                SfxPlayer.instance.PlaySfx(15);
            }
            else if (timeSpentInGame >= 240 && !timerWarningFlags[3])
            {
                timerWarningFlags[3] = true;
                SfxPlayer.instance.PlaySfx(15);
            }

            seconds = (int)(timeSpentInGame % 60);
            minutes = Mathf.FloorToInt(timeSpentInGame / 60);

            ingameTimerUI.text = "" + minutes + ":" + seconds;
        }

        /// <summary>
        /// Display player score on UI
        /// Score is the same as the total number of snake's bodyparts
        /// </summary>
        public void UpdateIngameScore()
        {
            if (!isGameStarted || isGameFinished)
                return;

            playerScore = mainPlayerObject.GetComponent<Snake>().GetBodypartsCount();
            ingameScoreUI.text = "" + playerScore;
        }

        /// <summary>
        /// As the game goes on, scores and snakes becomes bigger, so we need to increase the initial size for the snakes we want to spawn.
        /// </summary>
        public void UpdateMinimumBodyparts()
        {
            if (IngameLeaderboardManager.instance.leaderboardItemData.Count == 0)
                return;

            minimumSnakeBodyparts = Mathf.FloorToInt(IngameLeaderboardManager.instance.leaderboardItemData[0].score / 15f);

            //Prevent negative
            if (minimumSnakeBodyparts < 0)
                minimumSnakeBodyparts = 0;
        }

        /// <summary>
        /// Gameover logic
        /// </summary>
        /// <param name="delay"></param>
        public void Gameover(float delay = 3f)
        {
            isGameFinished = true;
            joystickController.SetActive(false);

            //Stats
            int totalPlayTime = PlayerPrefs.GetInt("TotalPlayTime", 0);
            int sessionTime = (int)Time.timeSinceLevelLoad;
            PlayerPrefs.SetInt("TotalPlayTime", totalPlayTime + sessionTime);

            StartCoroutine(DisplayGameoverSequenceCo(delay));
        }

        public IEnumerator DisplayGameoverSequenceCo(float delay)
        {
            CameraController.instance.Fadeout(delay);
            yield return new WaitForSeconds(delay);

            Time.timeScale = 1f;
            gameoverPanel.SetActive(true);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        public int GetInitialBodyparts()
        {
            return initialBodyParts + minimumSnakeBodyparts;
        }

        /// <summary>
        /// Add a bit of randomness to the bodypart size
        /// </summary>
        /// <returns></returns>
        public int GetRandomInitialBodyparts()
        {
            //For a very rare chance, spawn huge snakes
            if (Random.value > 0.98f)
                return Random.Range(30, 80);

            return Random.Range(1, 12);
        }

        /// <summary>
        /// We always need to hold a reference to the main player's snake
        /// </summary>
        /// <param name="go"></param>
        public void SetMainPlayer(GameObject go)
        {
            mainPlayerObject = go;
        }

        public GameObject GetMainPlayer()
        {
            return mainPlayerObject;
        }

        /// <summary>
        /// Find a random position on the game scene, while taking the boundaries into consideration
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRandomPositionInMap()
        {
            return new Vector3(Random.Range(minimumFieldX, maximumFieldX), Random.Range(minimumFieldY, maximumFieldY), 0);
        }

        public void EnableSpeedBoostViaUI()
        {
            speedBoostButtonIsPressed = true;
        }

        public void DisableSpeedBoostViaUI()
        {
            speedBoostButtonIsPressed = false;
        }

        public void UpdateRemainingEnemiesCount()
        {
            //We only need this info panel when "battle royale" game mode is being played.
            if (gameModeID != 4)
                return;

            if (!isGameStarted || isGameFinished)
                return;

            remainingEnemiesCount = BotSpawner.instance.GetAvailableBotsCount();
            remainingEnemiesUI.text = "" + remainingEnemiesCount;

            //If player is the only snake that is alive, game is finished. We however need a bit of delay to make sure the game doesn't end instantly
            if (remainingEnemiesCount <= 0 && Time.timeSinceLevelLoad > 5f)
            {
                //Sfx
                SfxPlayer.instance.PlaySfx(16);

                //Create vfx
                FBParticleManager.instance.CreateParticle(7, mainPlayerObject.transform.position);

                //Game over
                isGameFinished = true;
                Time.timeScale = 0.1f;
                Gameover(0.3f);
                return;
            }
        }
    }
}
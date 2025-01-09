using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// We have a realtime leaderboard inside the game which tracks the size of all snakes and updates their data on the UI accordingly.
/// </summary>

namespace SnakeWarzIO
{
    public class IngameLeaderboardManager : MonoBehaviour
    {
        public static IngameLeaderboardManager instance;

        public static int maxItemsInUiList = 10;

        //Prefabs
        public Transform listItemsParent;
        public GameObject listItemPrefab;

        //Find all players & bots in scene
        internal Snake[] snakes;
        internal List<int> snakeBodyCounts;
        public List<LeaderboardItemData> leaderboardItemData;

        //UI items
        public List<GameObject> UIListItems;

        //Color settings
        [Header("Color Settings")]
        public Color[] tierColors;
        public Color defaultColor;

        //Events
        public delegate void LeaderboardUpdated();
        public static event LeaderboardUpdated OnLeaderboardUpdated;

        //Important!
        public static int playerRank;


        [System.Serializable]
        public class LeaderboardItemData : IComparable<LeaderboardItemData>
        {
            public int score;
            public Snake snake;

            public LeaderboardItemData(int newScore, Snake newSnake)
            {
                score = newScore;
                snake = newSnake;
            }

            public int CompareTo(LeaderboardItemData lid)
            {
                if (lid == null)
                    return 1;

                return lid.score - score;
            }
        }

        private void Awake()
        {
            instance = this;
            UIListItems = new List<GameObject>();
            ResetArrays();
        }

        public void ResetArrays()
        {
            snakes = null;
            snakeBodyCounts = new List<int>();
            leaderboardItemData = new List<LeaderboardItemData>();
        }

        public virtual void UpdateLeaderboard()
        {
            UpdateLeaderboardData();
        }

        void Start()
        {
            //Create UIlistItems objects
            for (int i = 0; i < maxItemsInUiList; i++)
            {
                GameObject item = Instantiate(listItemPrefab, listItemsParent);
                item.name = "ListItem-" + i;
                UIListItems.Add(item);
            }

            //Soft start the ingame leaderboard after a while
            InvokeRepeating(nameof(UpdateLeaderboard), 0.5f, 1f);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                UpdateLeaderboardData();
            }
        }

        public void UpdateLeaderboardData()
        {
            //Important - apply a cooldown to this method at start
            if (Time.timeSinceLevelLoad < 0.25f)
                return;

            //Important - if UIlistItem objects are not ready, do not proceed.
            if (UIListItems.Count < maxItemsInUiList)
                return;

            //Important - don't proceed if we can't find enough players
            // (snakes.Length < maxItemsInUiList)
            //    return;

            //Reset all arrays
            ResetArrays();

            //Find all snake objects in scene (players & bots)
            snakes = FindObjectsOfType<Snake>();

            //Get the scores, ie bodycounts and store them in a separate array
            foreach (Snake s in snakes)
            {
                snakeBodyCounts.Add(s.GetBodypartsCount());
            }

            for (int i = 0; i < snakes.Length; i++)
            {
                LeaderboardItemData lid = new LeaderboardItemData(snakes[i].GetBodypartsCount(), snakes[i]);
                leaderboardItemData.Add(lid);
            }

            leaderboardItemData.Sort();

            //Generate new list on UI
            //print("UIListItems.Count: " + UIListItems.Count);
            //print("leaderboardItemData.Count: " + leaderboardItemData.Count);
            for (int j = 0; j < UIListItems.Count; j++)
            {
                //Prevent error if UIListItems & Snake numbers doesnt match
                if (UIListItems.Count > leaderboardItemData.Count)
                    return;

                //Default UI color for items
                Color c = Color.white;

                //Special cases - different colors for 1st to 3rd positions
                if (j >= 0 && j <= 2)
                {
                    c = tierColors[j];
                }
                else
                {
                    c = defaultColor;
                }

                //For the player (Your snake) we may want to use a different color
                if (leaderboardItemData[j].snake.tag.Contains("Player"))
                    c = Color.green;

                UIListItems[j].GetComponent<ListItem>().BuildListItemInstance((j + 1), leaderboardItemData[j].snake.GetNickname(), leaderboardItemData[j].snake.GetBodypartsCount(), c);
            }

            //We always need to show player name in leaderboard. So if player's score is smaller than the last snakes in leaderboard list, we need to replace the last item with player data
            if (!GameController.isGameFinished)
            {
                if (GameController.playerScore < leaderboardItemData[maxItemsInUiList - 1].snake.GetBodypartsCount())
                {
                    //update data on the last LB UI item
                    UIListItems[maxItemsInUiList - 1].GetComponent<ListItem>().BuildListItemInstance(
                        maxItemsInUiList,
                        GameController.mainPlayerObject.GetComponent<Snake>().GetNickname(),
                        GameController.mainPlayerObject.GetComponent<Snake>().bodyParts.Count,
                        Color.green);
                }
            }

            //Fire the event
            OnLeaderboardUpdated?.Invoke();
        }

        /// <summary>
        /// Find the player with the longest body
        /// </summary>
        /// <returns></returns>
        public GameObject GetTopPlayer()
        {
            return leaderboardItemData[0].snake.gameObject;
        }

        /// <summary>
        /// Get the rank, ie, the leaderboard position of the given snake
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public int GetRankByObject(Snake s)
        {
            int rank = -1;

            for (int i = 0; i < leaderboardItemData.Count; i++)
            {
                if (leaderboardItemData[i].snake == s)
                    rank = i + 1;
            }

            return rank;
        }

        /// <summary>
        /// Get the current rank of player snake
        /// </summary>
        /// <returns></returns>
        public int GetPlayerRank()
        {
            if (GameController.instance.GetMainPlayer())
            {
                playerRank = GetRankByObject(GameController.instance.GetMainPlayer().GetComponent<Snake>());
            }

            return playerRank;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// At the beginning of the game, we need to spawn certain number of bot snakes to make the game competitive for the player. BotSpawner handles this task beautifully.
/// 1) we have to create some predefined spawn points all across the game scene, so we can use them as possible positions to spawn bots.
/// 2) we need to select a random spawn position and spawn a bot on that position, if and only if that spawn position is free. ie, there is no other snake available at that position.
/// 3) we continue monitoring the state of our spawn positions all throughout the game.
/// </summary>

namespace SnakeWarzIO
{
    public class BotSpawner : MonoBehaviour
    {
        public static BotSpawner instance;

        [Header("Objects")]
        public GameObject collisionChecker;

        [Header("Spawn Settings & Items")]
        public GameObject[] availableBotsPrefab;
        public List<Vector3> availableSpawnPoints;
        public List<GameObject> availableSpawnLocations;
        private int edgeOffset = 15;

        [Header("Counters")]
        public List<GameObject> availableBotsInGame;
        public static int masterBotCounter;

        [Header("Misc")]
        public GameObject spawnLocationsParent;


        private void Awake()
        {
            instance = this;
            availableBotsInGame = new List<GameObject>();
            masterBotCounter = 0;

            CreateSpawnpoints();
        }

        /// <summary>
        /// Create certain number of spawn points we can use later on in order to spawn new bot snakes.
        /// </summary>
        public void CreateSpawnpoints()
        {
            //Init arrays
            availableSpawnPoints = new List<Vector3>();
            availableSpawnLocations = new List<GameObject>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //Spawn point (vector3)
                    Vector3 sp = new Vector3(GameController.minimumFieldX + (22 * j) + edgeOffset, GameController.minimumFieldY + (22 * i) + edgeOffset, 0);
                    availableSpawnPoints.Add(sp);

                    //Spawn location (game object)
                    GameObject sl = Instantiate(collisionChecker, sp, Quaternion.Euler(0, 0, 0)) as GameObject;
                    sl.name = "SpawnLocation-" + ((i + 1) * (j + 1));
                    sl.transform.parent = spawnLocationsParent.transform;
                    availableSpawnLocations.Add(sl);
                }
            }
        }

        void Start()
        {
            StartCoroutine(SpawnBotCo(GameController.initialSnakeCount));
        }

        /// <summary>
        /// When a bot is dead, we need to spawn another bot in a new location. For certain reasons, we may need to do some additional steps in this scenario.
        /// </summary>
        /// <param name="amount"></param>
        public void SpawnBotFromDeadSnake(int amount = 1)
        {
            //If additional steps needs to be taken
            //...

            //print("==> SpawnBotFromDeadSnake");
            StartCoroutine(SpawnBotCo(amount));
        }

        /// <summary>
        /// Spanw N number of bots
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public IEnumerator SpawnBotCo(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return new WaitForSeconds(0.05f);

                Vector3 spawnPoint = GetFreeSpawnPoint();
                if (spawnPoint != Vector3.zero)
                {
                    masterBotCounter++;

                    GameObject bot = Instantiate(availableBotsPrefab[Random.Range(0, availableBotsPrefab.Length)], spawnPoint, Quaternion.Euler(0, 0, 0)) as GameObject;
                    bot.name = "Bot-" + masterBotCounter;

                    AddBot(bot);
                }
                else
                {
                    //print("<b><color=red>" + spawnPoint + " was not FREE!" + "</color></b>");
                    StartCoroutine(SpawnBotCo(1));
                }
            }
        }

        /// <summary>
        /// Find a spawn point that is free of other players & bots.
        /// If no free spawn point is found, return Vector3.zero
        /// </summary>
        /// <returns></returns>
        public Vector3 GetFreeSpawnPoint()
        {
            Vector3 sp = Vector3.zero;

            //select a random spawn point from the array
            int rndPosID = Random.Range(0, availableSpawnPoints.Count);

            //check if the selected location is free
            if (availableSpawnLocations[rndPosID].GetComponent<CollisionChecker>().isFree)
            {
                return availableSpawnPoints[rndPosID];
            }

            return sp;
        }

        public void AddBot(GameObject obj)
        {
            availableBotsInGame.Add(obj);
        }

        public void DeleteBot(GameObject obj)
        {
            availableBotsInGame.Remove(obj);
        }

        public int GetAvailableBotsCount()
        {
            return availableBotsInGame.Count;
        }
    }
}
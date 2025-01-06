using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the main Snake controller base class that both player and bot snakes are inherited from. You can modify the general behaviour of the snake objects here.
/// But you can also tweak "PlayerController" & "BotController" classes incase you are looking for specific changes.
/// </summary>

namespace SnakeWarzIO
{
    public class Snake : MonoBehaviour
    {
        //Speed & Rotation 
        internal float moveSpeed = GameController.moveSpeedMax;
        internal float moveSpeedBoost = 1f;
        internal float rotationSpeed = 3f;
        internal float rotationSpeedBoostPenalty = 1f;
        internal float bodypartsFollowDelay;
        internal float sizeBasedSpeedPenalty = 0.0015f;  //Default = 0.002f
        internal int currentFoodToBodypartCounter;
        internal int bodyReduceCounter;
        internal int framesNeededForBodyReduce = 100;
        internal int minimumBodyparts = 3;

        //Direction
        internal Vector3 targetPosition;
        internal Vector3 myDirection;

        [Header("Body Parts")]
        public SpriteRenderer headShape;
        public GameObject headPart;
        public GameObject bodyPart;
        internal GameObject bodyPartsHolder;
        public GameObject headGlow;
        public GameObject magnetCircle;
        public int totalBodyParts;
        public List<GameObject> bodyParts;
        [Space(15)]
        public GameObject lastBodypart;

        //[Header("Position History Data")]
        //public List<Vector3> positionHistory;
        //public List<Vector3> rotationHistory;
        //internal float positionUpdateInterval = 0.05f;

        //Body Scale data
        internal Vector3 initialHeadScale;
        internal Vector3 initialBodyScale;
        internal float scaleUpStepsRatio = 0.01f; //default = 0.0075f

        [Header("Nickname")]
        public string nickname;

        [Header("Skin Settings")]
        public int skinID;

        //Leaderboard update event
        public delegate void UpdateLeaderboard();
        public static event UpdateLeaderboard OnUpdateLeaderboard;


        /*public void UpdatePositionHistory()
        {
            positionHistory.Add(transform.position);
            rotationHistory.Add(transform.eulerAngles);
        }*/

        public int GetRandomSkinID()
        {
            return Random.Range(0, SkinManager.totalAvailableSkins);
        }

        public string GetNickname()
        {
            return "" + nickname;
        }

        public int GetBodypartsCount()
        {
            return bodyParts.Count;
        }

        /// <summary>
        /// Create a new snake based on the given parameters.
        /// Please notice that we only create bodyparts and assign them to a holder in here. We also update the skins when needed. The actual snake object (the head part) will be created by BotSpawner.
        /// For player snake, the head part is always present at the game scene at the begining. (Refer to "BotController" & "PlayerController" classes for more info)
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="isBot"></param>
        public void CreateSnake(GameObject actor, bool isBot = false)
        {
            initialHeadScale = headPart.transform.localScale;
            initialBodyScale = initialHeadScale;

            GameObject bph = new GameObject();
            bph.name = actor.name + "-BodyPartsHolder";
            bodyPartsHolder = bph;

            //Skin settings - head
            headShape.sprite = SkinManager.instance.GetHeadSkin(skinID);    

            //If this is a bot, we may want to give them additional bodyparts to make the game more fun when played with bots
            int totalBPs = GameController.instance.GetInitialBodyparts();
            if (isBot)
                totalBPs += GameController.instance.GetRandomInitialBodyparts();

            AddBodypart(totalBPs);

            //Fire an event to update leaderboard list data
            if (OnUpdateLeaderboard != null)
                OnUpdateLeaderboard();
        }

        public void AddBodypart(int amount = 1)
        {
            StartCoroutine(AddBodypartCo(amount));
        }

        public IEnumerator AddBodypartCo(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                AddBodypartMain();
                yield return new WaitForEndOfFrame();
            }
        }

        public void AddBodypartMain()
        {
            //Handle exceptions
            if (!bodyPartsHolder)
                return;

            Vector3 addPos;
            if (lastBodypart)
            {
                addPos = lastBodypart.transform.position;
            }
            else
            {
                addPos = transform.position;
            }

            GameObject nbp = Instantiate(bodyPart, addPos, Quaternion.Euler(90, 0, 0)) as GameObject;
            bodyParts.Add(nbp);
            nbp.GetComponent<BodypartController>().partIndex = totalBodyParts;
            nbp.GetComponent<BodypartController>().owner = this.gameObject;

            if (lastBodypart)
                nbp.GetComponent<BodypartController>().target = lastBodypart.transform;
            else
                nbp.GetComponent<BodypartController>().target = headPart.transform;

            //Skin settings - bodyparts
            nbp.GetComponent<BodypartController>().bodyShape.sprite = SkinManager.instance.GetBodySkin(skinID);

            nbp.name = "Body-" + totalBodyParts;
            nbp.transform.parent = bodyPartsHolder.transform;
            nbp.transform.localPosition = new Vector3(
                nbp.transform.localPosition.x /*+ (i * 0.25f * -1)*/,
                nbp.transform.localPosition.y,
                /*nbp.transform.localPosition.z +*/ ((totalBodyParts + 1) * 0.0001f));

            totalBodyParts++;

            //Set last bodypart
            lastBodypart = nbp;

            UpdateSize(totalBodyParts);
        }

        /// <summary>
        /// We should decrease snake size by removing bodyparts when this snake is using speed boost to keep the balance in game.
        /// </summary>
        public void RemoveBodypart()
        {
            //cache last bodypart of this snake
            GameObject bodypartToRemove = lastBodypart;

            //remove last BP from array
            bodyParts.RemoveAt(bodyParts.Count - 1);

            //Update the counter
            totalBodyParts--;

            //Assign the new last BP
            lastBodypart = bodyParts[bodyParts.Count - 1];

            //Destroy the unused last BP
            Destroy(bodypartToRemove);
        }

        /// <summary>
        /// Change the size of the snake as it grows bigger and also reduce its movement speed accordingly.
        /// </summary>
        /// <param name="totalParts"></param>
        public void UpdateSize(int totalParts)
        {
            //print("totalParts: " + totalParts);
            if (totalParts > GameController.maxSnakeSizeForScale)
            {
                print("Maximum scale reached!");
                //return;
                totalParts = GameController.maxSnakeSizeForScale;
            }

            headPart.transform.localScale = initialHeadScale * ((totalParts * scaleUpStepsRatio) + 1);
            for (int i = 0; i < bodyParts.Count; i++)
            {
                bodyParts[i].transform.localScale = initialBodyScale * ((totalParts * scaleUpStepsRatio) + 1);
            }

            //decrease speed based on size
            moveSpeed = GameController.moveSpeedMax - (totalBodyParts * sizeBasedSpeedPenalty);
            if (moveSpeed < 1)
                moveSpeed = 1;
        }

        /// <summary>
        /// Happens all the time to all mortal things & makes a lot of weight on garbage collector's shoulder
        /// </summary>
        public void Die()
        {
            if (GameController.canRespawnDeadBots)
            {
                //Death of each bot should led to one or more bots being born at the same time
                BotSpawner.instance.SpawnBotFromDeadSnake(1);
            }            

            //Collider on the head of this object should be disabled instantly
            GetComponent<SphereCollider>().enabled = false;

            //Important check - see if this is the main player that is dead
            if (gameObject.tag == "PlayerHead")
            {
                //Refresh player rank on Leaderboard
                IngameLeaderboardManager.instance.GetPlayerRank();

                //Incase speed boost sfx was looped, stop it instantly
                SfxPlayer.instance.StopLoopedSfx(7);

                //Stats
                int collectedFood = PlayerPrefs.GetInt("CollectedFood");
                collectedFood += currentFoodToBodypartCounter;
                PlayerPrefs.SetInt("CollectedFood", collectedFood);

                print("Player is dead & game is over");
                GameController.instance.Gameover();
            }

            //turn some/all bodyparts of this dead object into eatable foods
            if (GameController.canTurnDeadSnakesIntoGhostfood)
            {
                for (int i = 0; i < totalBodyParts; i++)
                {
                    //for 1 out of 2 part, turn it into food
                    if (i % 2 == 0)
                    {
                        FoodSpawner.instance.SpawnGhostFood(bodyParts[i].transform.position);
                    }
                }
            }            

            //Shake camera if the death was near enough & mainplayer is alive
            if (GameController.instance.GetMainPlayer() != null)
            {
                float distanceToPlayer = Vector3.Distance(this.transform.position, GameController.instance.GetMainPlayer().transform.position);
                if (distanceToPlayer < GameController.maxDistanceToTriggerShake)
                {
                    CameraShaker.instance.PublicShake(1.2f, 0.25f);

                    //Play death sfx
                    if (Time.timeSinceLevelLoad > 2f)
                        SfxPlayer.instance.PlaySfx(4, 1 - (distanceToPlayer / GameController.maxDistanceToTriggerShake));
                }
            }

            //Fire an event to update leaderboard list data
            if (OnUpdateLeaderboard != null)
                OnUpdateLeaderboard();

            BotSpawner.instance.DeleteBot(this.gameObject);

            Destroy(bodyPartsHolder);
            Destroy(gameObject);
        }

        /// <summary>
        /// This method is used when we kill another snake, and it converts a part of the other snake's size to score and adds it instantly to our size
        /// </summary>
        public void ConvertKillPrizeToBodyparts(int killedSnakeSize)
        {
            int sizePrize = Mathf.FloorToInt(killedSnakeSize / 10f);
            if (sizePrize == 0) sizePrize = 1;

            AddBodypart(sizePrize);
            UpdateSize(totalBodyParts);

            //print("<b>killedSnakeSize: </b>" + killedSnakeSize);
            //print("<b>Kill prize: </b>" + sizePrize);
        }


        public void EnableMagnetCircle(bool _state)
        {
            magnetCircle.SetActive(_state);
        }
    }
}
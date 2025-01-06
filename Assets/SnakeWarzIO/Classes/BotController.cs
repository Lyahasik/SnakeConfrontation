using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In this game, we are using bot snakes to make the game competitive for our players. To do this, we need smart bots that are able to handle ingame events.
/// This bot controller class is inherited from the main Snake class and is able to give bots certain abilities like moving around, sreaching for foods, 
/// finding ghost foods (remaining of dead snakes), using extra speed or tricking other snakes into death collisions.
/// </summary>

namespace SnakeWarzIO
{
    public class BotController : Snake
    {
        [Header("Bot Settings")]
        private bool botCanSearchForNewTarget;              //for manual usage
        private Vector3 botTargetPoint;
        private bool botCanUseSpeedBoost;
        private float botDistanceToTarget;

        [Header("Bot Target")]
        public GameObject botTargetPrefab;
        private GameObject myTargetInScene;

        //Experimental - Crazy moves by bots to make the game more fun and engaging
        private bool isCrazy;


        private void Awake()
        {
            totalBodyParts = 0;
            bodyParts = new List<GameObject>();
            lastBodypart = null;

            //Init
            targetPosition = new Vector3(0, 0, 0);
            myDirection = new Vector3(0, 0, 0);
            headGlow.SetActive(false);

            //Bot
            botCanSearchForNewTarget = true;
            botTargetPoint = Vector3.zero;
            botCanUseSpeedBoost = false;
            botDistanceToTarget = Mathf.Infinity;
            StartCoroutine(EnableBotBoostCooldownCo());

            //Misc
            isCrazy = false;
        }

        void Start()
        {
            //Create the snake
            skinID = GetRandomSkinID();
            CreateSnake(gameObject, true);

            //Create helper target which can be seen ingame
            myTargetInScene = Instantiate(botTargetPrefab, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            myTargetInScene.name = gameObject.name + "-Target";

            //Set a nickname for this bot
            nickname = NicknameGenerator.instance.GenerateNickname();

            //Create a sticky name that follows the snake on the pit
            NicknameGenerator.instance.CreateStickyNickname(gameObject);

            //Reste all states
            BotResetSpeedBoost();
        }

        /// <summary>
        /// When bot is finding for a new target, we need some cooldown till it can look for another target again.
        /// </summary>
        /// <returns></returns>
        public float BotGetNewCooldown()
        {
            float minRange, maxRange;
            minRange = Random.value > 0.8f ? 2f : 10f;
            maxRange = Random.value > 0.8f ? 20f : 30f;

            float cd = Random.Range(minRange, maxRange);
            //print("New Bot Cooldown: " + cd);

            return cd;
        }


        public float BotGetNewSpeedBoostInterval()
        {
            return Random.Range(2f, 8f);
        }


        public float BotGetNewSpeedBoostDuration()
        {
            return Random.Range(1f, 4f);
        }


        private void Update()
        {
            //Debug
            if (Application.isEditor && Input.GetKeyDown(KeyCode.Alpha1))
            {
                PerformCrazyMove();
            }

            //Debug
            //if (Application.isEditor)
            //    Debug.DrawLine(transform.position, myTargetInScene.transform.position, Color.yellow);
        }

        /// <summary>
        /// Experimental
        /// </summary>
        public void PerformCrazyMove()
        {
            if (isCrazy)
                return;
            isCrazy = true;

            print("CrazyMode is on");
            StartCoroutine(PerformPlayerHeadHuntCo());
        }


        /// <summary>
        /// Find the nearest player and try to kamikaze into its head to perform a suicide kill
        /// </summary>
        /// <returns></returns>
        public IEnumerator PerformPlayerHeadHuntCo()
        {
            float duration = 15f;
            float t = 0;
            GameObject target = GameController.instance.GetMainPlayer();

            while (t < duration)
            {
                if (!target)
                {
                    isCrazy = false;
                    yield break;
                }

                t += Time.deltaTime;
                botTargetPoint = target.transform.position;
                StartCoroutine(BotSpeedBoostCo());
                yield return 0;
            }

            if (t >= duration)
            {
                isCrazy = false;
            }
        }


        void LateUpdate()
        {
            if (!GameController.isGameStarted /*|| GameController.isGameFinished*/)
            {
                //Do nothing
                return;
            }

            StartCoroutine(BotFindNewTargetCo());
            StartCoroutine(EnablePeriodicSpeedBoostCo());
            MoveBot();
        }

        /// <summary>
        /// Allows the bot to perform extra speed movements
        /// </summary>
        /// <returns></returns>
        public IEnumerator EnablePeriodicSpeedBoostCo()
        {
            if (isCrazy)
                yield break;

            if (!botCanUseSpeedBoost)
                yield break;
            botCanUseSpeedBoost = false;
            StartCoroutine(EnableBotBoostCooldownCo());

            StartCoroutine(BotSpeedBoostCo());
        }


        public IEnumerator BotSpeedBoostCo()
        {
            //enable
            moveSpeedBoost = GameController.moveSpeedBoostMax;
            rotationSpeedBoostPenalty = GameController.rotationSpeedBoostPenalty;
            bodypartsFollowDelay = GameController.bodypartsFollowDelayBoost;

            //Display head glow
            headGlow.SetActive(true);

            //Reduce bodyparts when speedboost is enabled
            ReduceBodyparts();

            yield return new WaitForSeconds(BotGetNewSpeedBoostDuration());

            //reset
            BotResetSpeedBoost();
        }

        /// <summary>
        /// When extra speed feature is being used, it comes with a cost of reducing a bit of bodyparts over time.
        /// </summary>
        public void ReduceBodyparts()
        {
            bodyReduceCounter += 1;
            if (bodyReduceCounter % framesNeededForBodyReduce == 0)
            {
                bodyReduceCounter = 0;

                RemoveBodypart();
                UpdateSize(totalBodyParts);
            }
        }

        public IEnumerator EnableBotBoostCooldownCo()
        {
            yield return new WaitForSeconds(BotGetNewSpeedBoostInterval());
            botCanUseSpeedBoost = true;
        }

        public IEnumerator BotFindNewTargetCo(bool forceFind = false)
        {
            if (isCrazy)
                yield break;

            if (!botCanSearchForNewTarget && !forceFind)
                yield break;
            botCanSearchForNewTarget = false;
            StartCoroutine(EnableBotCooldownCo());

            botTargetPoint = GetNewTargetForBot();
            //print("Overridden New Target: " + botTargetPoint);

            //Move target object to new target position
            if (myTargetInScene)
                myTargetInScene.transform.position = botTargetPoint;
        }


        public IEnumerator EnableBotCooldownCo()
        {
            float delay = BotGetNewCooldown();
            yield return new WaitForSeconds(delay);
            botCanSearchForNewTarget = true;
        }

        public void MoveBot()
        {
            if (botTargetPoint != Vector3.zero)
            {
                targetPosition = botTargetPoint;
                myDirection = GetDirectionToMouse();
                RotateTowardsInput();
                MoveTowardsInput();
            }
        }

        public Vector3 GetNewManeuverPosition(Vector3 obstaclePos)
        {
            Vector3 nmp = Vector3.zero;
            Vector3 currentPos = transform.position;

            nmp = currentPos + ((currentPos - obstaclePos) * 2.5f);
            //print("NMP: " + nmp);

            //Move target object to new target position
            if (myTargetInScene)
                myTargetInScene.transform.position = nmp;

            return nmp;
        }


        /// <summary>
        /// Important. This method is the one that makes our bots smart or dumb. The least we can do is to give them a random position in the scene, so they can start moving.
        /// but the better approach is to make them search for ghost or normal foods and collect everything they see in their vicinity.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetNewTargetForBot()
        {
            Vector3 newTarget = Vector3.zero;
            bool useSmartWay = false;

            //Find a random pos on map anyway - we may use it later on
            Vector3 randomPosOnMap = new Vector3(Random.Range(GameController.minimumFieldX, GameController.maximumFieldX), Random.Range(GameController.minimumFieldY, GameController.maximumFieldY), 0);

            //Check if we have better options

            //#1 - check if we can go towards a food & no danger is near (no other bots or players)
            float searchRadius = 5;
            LayerMask snakeMask = LayerMask.GetMask("Snake");
            Collider[] snakeParts = Physics.OverlapSphere(transform.position, searchRadius, snakeMask);

            //Debug
            //print("snakeParts.Lenght: " + snakeParts.Length);

            //Now filter out only the colliders that are not belong us (this snake)
            List<Collider> otherSnakeParts = new List<Collider>();
            foreach (Collider c in snakeParts)
            {
                //print("OverlapSphere.Hit: " + c.name);

                //#1
                if (c.gameObject.tag == "BotHead")
                {
                    if (c.gameObject != this.gameObject)
                        otherSnakeParts.Add(c);
                }

                //#2
                if (c.gameObject.tag == "PlayerHead")
                {
                    otherSnakeParts.Add(c);
                }

                //#3
                if (c.gameObject.tag == "BotBody")
                {
                    if (c.GetComponent<BodypartController>().owner != this.gameObject)
                        otherSnakeParts.Add(c);
                }
            }

            //Debug again
            //print("<b>" + gameObject.name + " ==> otherSnakeParts.Lenght: " + otherSnakeParts.Count + "</b>");

            if (otherSnakeParts.Count == 0)
            {
                //print("Eat foods like crazy");
                //print("**************************");

                //Old
                //int newLayerMask = 1 << 10;
                //Collider[] foods = Physics.OverlapSphere(transform.position, searchRadius, newLayerMask);

                //New
                LayerMask foodMask = LayerMask.GetMask("Food");
                Collider[] foods = Physics.OverlapSphere(transform.position, searchRadius, foodMask);

                if (foods.Length > 0)
                {
                    newTarget = GetClosestObject(foods);
                    useSmartWay = true;
                }
            }


            //#2 - pick a random pos
            if (!useSmartWay)
                newTarget = randomPosOnMap;

            return newTarget;
        }


        //Transform GetClosestObject(Transform[] objects)
        public Vector3 GetClosestObject(Collider[] objects)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach (Collider potentialTarget in objects)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }

            return bestTarget.position;
        }


        public Vector3 GetDirectionToMouse()
        {
            Vector3 dir = targetPosition - transform.position;
            //print("myDirection: " + myDirection);
            return dir;
        }


        public void RotateTowardsInput()
        {
            myDirection.Normalize();
            float rotation_z = Mathf.Atan2(myDirection.y, myDirection.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, rotation_z, Time.deltaTime * rotationSpeed * rotationSpeedBoostPenalty));
        }


        public void MoveTowardsInput()
        {
            transform.Translate(Vector3.right * moveSpeed * moveSpeedBoost * Time.deltaTime * 1f, Space.Self);

            //check if bot is arrived at target point
            botDistanceToTarget = Vector3.Distance(transform.position, botTargetPoint);
            if (botDistanceToTarget < 2)
            {
                //print("<color=green>" + gameObject.name + " arrived to its target point!" + "</color>");
                //QuickManeuver();
                StartCoroutine(BotFindNewTargetCo(true));
            }
        }

        /// <summary>
        /// When a bot snake detects a difficult situation, it will change its direction.
        /// The actual detection is done via "BotSensorController" class.
        /// </summary>
        /// <param name="hitPoint"></param>
        public void QuickManeuver(Vector3 hitPoint = default(Vector3))
        {
            //print("<b>Bot ==> Force direction change!</b>");

            //Reset speed boost (if its activated already)
            BotResetSpeedBoost();

            //Find an urgent maneuver target point
            botTargetPoint = GetNewManeuverPosition(hitPoint);
        }


        public void BotResetSpeedBoost()
        {
            moveSpeedBoost = 1f;
            rotationSpeedBoostPenalty = 1f;
            bodypartsFollowDelay = GameController.bodypartsFollowDelayNormal;

            //hide head glow
            headGlow.SetActive(false);
        }


        private void OnTriggerEnter(Collider other)
        {
            //print("Hit: " + other.gameObject.name);
            //print("<color=yellow>" + gameObject.name + " hit the: " + other.gameObject.name + "</color>");

            //If we run into a food, pick it up & consume it
            if (other.gameObject.tag == "Food")
            {
                currentFoodToBodypartCounter++;
                if (currentFoodToBodypartCounter % GameController.foodIntoBodypart == 0)
                {
                    AddBodypart(GameController.normalFoodScore);
                    UpdateSize(totalBodyParts);
                }

                other.GetComponent<FoodController>().Absorb(this.gameObject);
            }


            //If we run into a ghost food (aka remaining of a dead player or bot), pick it up & consume it
            if (other.gameObject.tag == "GhostFood")
            {
                currentFoodToBodypartCounter++;
                if (currentFoodToBodypartCounter % GameController.ghostfoodIntoBodypart == 0)
                {
                    AddBodypart(GameController.ghostFoodScore);
                    UpdateSize(totalBodyParts);
                }

                other.gameObject.GetComponent<FoodController>().Absorb(this.gameObject);
            }

            //if a bot runs into player's body
            if (other.gameObject.tag == "PlayerBody")
            {
                //Create vfx
                FBParticleManager.instance.CreateParticle(1, other.transform.position);
                FBParticleManager.instance.CreateParticle(6, other.transform.position);

                //The other snake should receive a prize
                other.GetComponent<BodypartController>().owner.GetComponent<Snake>().ConvertKillPrizeToBodyparts(bodyParts.Count);

                //Stats
                int totalKills = PlayerPrefs.GetInt("TotalKills");
                PlayerPrefs.SetInt("TotalKills", ++totalKills);

                //this bot is now dead
                Die();
            }

            //if a bot runs into player's Head
            if (other.gameObject.tag == "PlayerHead")
            {
                //both players are dead
                Die();
            }

            //if a bot runs into another bot's body
            if (other.gameObject.tag == "BotBody" && other.gameObject.GetComponent<BodypartController>().owner != this.transform.root.gameObject)
            {
                //The other snake should receive a prize
                other.GetComponent<BodypartController>().owner.GetComponent<Snake>().ConvertKillPrizeToBodyparts(bodyParts.Count);

                //this bot is now dead
                Die();
            }

            //if a bot runs into another bot's head
            if (other.gameObject.tag == "BotHead" && other.gameObject != this.gameObject)
            {
                //both bots are now dead
                Die();
            }

            //if a bot runs into any border, its GG
            if (other.gameObject.tag == "Border")
            {
                Die();
            }
        }

        public new void Die()
        {
            Destroy(myTargetInScene);

            base.Die();
        }
    }
}
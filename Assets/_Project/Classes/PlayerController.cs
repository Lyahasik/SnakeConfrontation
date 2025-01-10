using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is where we control our player's snake.
/// </summary>

namespace SnakeWarzIO
{
    public class PlayerController : Snake
    {
        private GameObject joystickHelper;
        private bool canPlaySpeedBoostSfx;

        private void Awake()
        {
            //Init
            totalBodyParts = 0;
            currentFoodToBodypartCounter = 0;
            bodyParts = new List<GameObject>();
            lastBodypart = null;
            canPlaySpeedBoostSfx = true;
            joystickHelper = GameObject.FindGameObjectWithTag("JoystickPositionController");            
            targetPosition = new Vector3(0, 0, 0);
            myDirection = new Vector3(0, 0, 0);
            headGlow.SetActive(false);
            base.skinID = PlayerPrefs.GetInt("SelectedSkinID", 0);
            magnetCircle.SetActive(false);
        }

        void Start()
        {
            //Create the snake
            CreateSnake(gameObject);

            //Player nickname - it should be received via NETWORK but for now, we use a hardcoded string
            //nickname = "You";
            nickname = PlayerPrefs.GetString("PlayerName", "You");

            //Set this snake as the main player snake on GameController - this needs to be synced over netwrok since each newtwork player controls its own snake
            GameController.instance.SetMainPlayer(this.gameObject);

            //Create a sticky name that follows the snake on the pit
            NicknameGenerator.instance.CreateStickyNickname(gameObject);
        }

        private void Update()
        {
            // TODO Debug - add manual bodypart
            // if (Application.isEditor)
            // {
            //     if (Input.GetKey(KeyCode.Return))
            //     {
            //         currentFoodToBodypartCounter++;
            //         if (currentFoodToBodypartCounter % GameController.foodIntoBodypart == 0)
            //         {
            //             AddBodypart(GameController.normalFoodScore * BoostersController.instance.GetScoreMultiplier());
            //             UpdateSize(totalBodyParts);
            //         }
            //     }
            // }
            // else
            // {
            //     #if UNITY_WEBGL
            //         if (Input.GetKey(KeyCode.Alpha8))
            //         {
            //             currentFoodToBodypartCounter++;
            //             if (currentFoodToBodypartCounter % GameController.foodIntoBodypart == 0)
            //             {
            //                 AddBodypart(GameController.normalFoodScore);
            //                 UpdateSize(totalBodyParts);
            //             }
            //         }
            //     #endif
            // }
        }

        void LateUpdate()
        {
            if (!GameController.isGameStarted || GameController.isGameFinished)
            {
                //stop boost sfx
                SfxPlayer.instance.StopLoopedSfx(7);

                //Do nothing
                return;
            }

            //Control type #1 - using virtual joystick - useful for touch devices
            //Control type #2 - using mouse & single click to burst - useful for PC & desktop        
            HandleBoostState(GameController.controlType);
            targetPosition = Get2dMousePosition(GameController.controlType);
            myDirection = GetDirectionToMouse();
            RotateTowardsInput();
            MoveTowardsInput();
        }

        /// <summary>
        /// Players can activate the extra speed mode on their snakes by using Space key, UI button (left mouse button), or virtual joystick. 
        /// Moving at extra speed comes with a price as it consumes from your bodyparts. So this needs to be used with care and is specially useful to hunt another snake or flee.
        /// </summary>
        /// <param name="_ctrlType"></param>
        public void HandleBoostState(int _ctrlType = 0)
        {
            if ((Input.GetKey(KeyCode.Space) || GameController.speedBoostButtonIsPressed || (_ctrlType == 1 && Input.GetMouseButton(0))) && bodyParts.Count > minimumBodyparts)
            {
                moveSpeedBoost = GameController.moveSpeedBoostMax;
                rotationSpeedBoostPenalty = GameController.rotationSpeedBoostPenalty;
                bodypartsFollowDelay = GameController.bodypartsFollowDelayBoost;

                //Reduce bodyparts when speedboost is enabled
                ReduceBodyparts();

                //Play boost sfx
                SfxPlayer.instance.PlaySfxLooped(7);

                //Display headGlow
                headGlow.SetActive(true);

                if (canPlaySpeedBoostSfx)
                {
                    canPlaySpeedBoostSfx = false;
                    SfxPlayer.instance.PlaySfx(8, 0.15f);
                }
            }
            /*else if (Input.GetKeyUp(KeyCode.Space) || (_ctrlType == 1 && Input.GetMouseButtonUp(0)))
            {
                moveSpeedBoost = 1f;
                rotationSpeedBoostPenalty = 1f;
                bodypartsFollowDelay = GameController.bodypartsFollowDelayNormal;

                //Hide headGlow
                headGlow.SetActive(false);

                //stop boost sfx
                SfxPlayer.instance.StopLoopedSfx(7);
                canPlaySpeedBoostSfx = true;
            }*/
            else
            {
                moveSpeedBoost = 1f;
                rotationSpeedBoostPenalty = 1f;
                bodypartsFollowDelay = GameController.bodypartsFollowDelayNormal;

                //Hide headGlow
                headGlow.SetActive(false);

                //stop boost sfx
                SfxPlayer.instance.StopLoopedSfx(7);
                canPlaySpeedBoostSfx = true;
            }
        }

        /// <summary>
        /// Decrease snake's bodyparts when extra speed mode is on for long enough!
        /// </summary>
        public void ReduceBodyparts()
        {
            bodyReduceCounter += 1; //Counter is increased by 1 each frame
            if (bodyReduceCounter % framesNeededForBodyReduce == 0)
            {
                bodyReduceCounter = 0;

                RemoveBodypart();
                UpdateSize(totalBodyParts);
            }
        }

        /// <summary>
        /// Find the position of player mouse on the screen. We need this data since our snake is supposed to move towards our input position as all times.
        /// </summary>
        /// <param name="_ctrlType"></param>
        /// <returns></returns>
        public Vector3 Get2dMousePosition(int _ctrlType = 0)
        {
            Vector3 result = Vector3.zero;

            if (_ctrlType == 0)
            {
                result = new Vector3(joystickHelper.transform.position.x, joystickHelper.transform.position.y, Camera.main.transform.position.z * -1);
            }
            else if (_ctrlType == 1)
            {
                result = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1));
            }

            return result;
        }

        /// <summary>
        /// Find the direction from snake to player's input position. This is needed so we can rotate the snake appropriately.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDirectionToMouse()
        {
            Vector3 dir = targetPosition - transform.position;
            //print("myDirection: " + myDirection);
            return dir;
        }

        //float smoothAmount = 0.1f;
        //float vel = 0;
        //float rollVelocity = 0;
        //float rollSmoothTime = 0.1f;
        public void RotateTowardsInput()
        {
            myDirection.Normalize();
            float rotation_z = Mathf.Atan2(myDirection.y, myDirection.x) * Mathf.Rad2Deg;   //using this formula to translate the dir into rotation gives the most accurate result and is also able to handle positive/negative directions.
            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, rotation_z, Time.deltaTime * rotationSpeed * rotationSpeedBoostPenalty));
        }

        /// <summary>
        /// Move the snake towards its target with the given params
        /// </summary>
        public void MoveTowardsInput()
        {
            transform.Translate(1f * BoostersController.instance.GetExtraSpeed() * moveSpeed * moveSpeedBoost * Time.deltaTime * Vector3.right, Space.Self);
        }


        public void OnMagnetTriggerEnter(Collider other)
        {
            OnTriggerEnter(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            //print("Hit: " + other.gameObject.name);

            //If we run into a food, pick it up & consume it
            if (other.gameObject.tag == "Food")
            {
                //Create vfx
                FBParticleManager.instance.CreateParticle(3, headPart.transform.position);

                currentFoodToBodypartCounter++;
                if (currentFoodToBodypartCounter % GameController.foodIntoBodypart == 0)
                {
                    //Create vfx
                    FBParticleManager.instance.CreateParticle(0, transform.position);
                    FBParticleManager.instance.CreateParticle(5, transform.position);

                    AddBodypart(GameController.normalFoodScore * BoostersController.instance.GetScoreMultiplier());
                    UpdateSize(totalBodyParts);
                }

                other.GetComponent<FoodController>().Absorb(this.gameObject);

                //Play eat sfx
                SfxPlayer.instance.PlaySfx(0);
            }

            //If we run into a ghost food (aka remaining of a dead player or bot), pick it up & consume it
            if (other.gameObject.tag == "GhostFood")
            {
                //Create vfx
                FBParticleManager.instance.CreateParticle(3, headPart.transform.position);

                currentFoodToBodypartCounter++;
                if (currentFoodToBodypartCounter % GameController.ghostfoodIntoBodypart == 0)
                {
                    //Create vfx
                    FBParticleManager.instance.CreateParticle(0, transform.position);
                    FBParticleManager.instance.CreateParticle(5, transform.position);

                    AddBodypart(GameController.ghostFoodScore * BoostersController.instance.GetScoreMultiplier());
                    UpdateSize(totalBodyParts);
                }

                other.gameObject.GetComponent<FoodController>().Absorb(this.gameObject);

                //Play eat sfx
                SfxPlayer.instance.PlaySfx(1);
            }

            //If we run into a booster, pick it up & consume it
            if (other.gameObject.tag == "IngameBooster")
            {
                //Create spark vfx
                FBParticleManager.instance.CreateParticle(4, transform.position);

                //Create booster vfx
                FBParticleManager.instance.CreateParticle(other.gameObject.GetComponent<IngameBoosterController>().vfxID, transform.position);

                //Play eat sfx
                SfxPlayer.instance.PlaySfx(0);

                //Activate the booster
                BoostersController.instance.ActivateBoosterIngame((int)other.gameObject.GetComponent<IngameBoosterController>().boosterID);

                //Update the booster object
                other.gameObject.GetComponent<IngameBoosterController>().ApplyReposition();
            }

            //if a bot runs into bot's body
            if (other.gameObject.tag == "BotBody")
            {
                //The other snake should receive a prize
                other.GetComponent<BodypartController>().owner.GetComponent<Snake>().ConvertKillPrizeToBodyparts(bodyParts.Count);

                //Create vfx
                FBParticleManager.instance.CreateParticle(2, transform.position);

                //we are dead
                Die();
            }

            //if a bots run into player's Head
            if (other.gameObject.tag == "BotHead")
            {
                //Create vfx
                FBParticleManager.instance.CreateParticle(2, transform.position);

                //both player & bot are dead
                //other.GetComponent<BotController>().Die();
                Die();
            }

            //if player run into another player's body (online mode)
            if (other.gameObject.tag == "PlayerBody" && other.gameObject.GetComponent<BodypartController>().owner != this.transform.root.gameObject)
            {
                //Create vfx
                FBParticleManager.instance.CreateParticle(2, transform.position);

                //The other snake should receive a prize
                other.GetComponent<BodypartController>().owner.GetComponent<Snake>().ConvertKillPrizeToBodyparts(bodyParts.Count);

                //we are dead
                Die();
            }

            //if player run into another player's head (online mode)
            if (other.gameObject.tag == "PlayerHead" && other.gameObject != this.gameObject)
            {
                //Create vfx
                FBParticleManager.instance.CreateParticle(2, transform.position);

                //both bots are now dead
                Die();
            }

            //if player run into any border, its GG
            if (other.gameObject.tag == "Border")
            {
                //Create vfx
                FBParticleManager.instance.CreateParticle(2, transform.position);

                Die();
            }
        }

        /// <summary>
        /// Change the skin of player's snake.
        /// </summary>
        /// <param name="skinID"></param>
        public void UpdateSkin(int skinID = 0)
        {
            //Apply the selected skin to head and bodyparets
            headShape.sprite = SkinManager.instance.GetHeadSkin(skinID);
            foreach (GameObject go in bodyParts)
            {
                go.GetComponent<BodypartController>().bodyShape.sprite = SkinManager.instance.GetBodySkin(skinID);
            }
        }

        /// <summary>
        /// Everything that has a beginning, has an end. Love is the only thing that remains. 
        /// </summary>
        public new void Die()
        {
            base.Die();
        }
    }
}
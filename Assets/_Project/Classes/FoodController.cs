using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// Foods are spread accross the scene for player and snake bots to eat and grow. We have two type of food.
/// 1) Normal food = the first instance of foods that "FoodSpawner" class spawns at the beginning of the game
/// 2) Ghost food = which is created once a snake is dead. (refer to Snake.cs -> Die() for more info)
/// </summary>

namespace SnakeWarzIO
{
    public class FoodController : MonoBehaviour
    {
        public enum FoodTypes { NormalFood, GhostFood }
        [Header("Food Type")]
        public FoodTypes foodType = FoodTypes.NormalFood;

        //Static behaviours (Edit with care!)
        public static float normalFoodMoveSpeed = 2.5f;
        public static float normalFoodRadius = 1.5f;
        public static float normalFoodBodyScaleRatio = 1f;
        public static float ghostFoodMoveSpeed = 3.5f;
        public static float ghostFoodRadius = 1.25f;
        public static float ghostFoodBodyScaleRatio = 3f;
        public static float scaleAnimSpeed = 1f;

        //[Header("Public Stats")]
        //public Color[] availableColors;
        //public Sprite[] availableIcons;

        [Header("Food Skin Settings")]
        public static int selectedFoodSkinID;
        public List<FoodSkin> availableFoodSkins;


        [Header("Objects")]
        public GameObject bodyParent;
        public GameObject shape;
        public GameObject trail;

        [Header("Animation")]
        float animSpeed;
        float normalAnimSpeed = 2f;
        float ghostAnimSpeed = 6f;
        float rndAnimSpeed;

        //Private stats
        private Animator myAnimator;
        private Vector3 startSize;
        private Vector3 targetSize;
        private float currentMoveSpeed;
        private bool isMoving;
        private Vector3 startPosition;
        private SphereCollider myCollider;
        private float bodyScaleRatio;

        private void Awake()
        {
            myAnimator = GetComponent<Animator>();
            trail.SetActive(false);

            startSize = bodyParent.transform.localScale;
            targetSize = startSize * 1.3f;
            rndAnimSpeed = Random.Range(0, 3);
        }

        public void OnEnable()
        {
            selectedFoodSkinID = PlayerPrefs.GetInt("SelectedFoodSkinID", 0);
            //selectedFoodSkinID = 0;

            isMoving = false;
            startPosition = transform.position;
            myCollider = GetComponent<SphereCollider>();

            //Debug
            InitObjectBasedOnType();
        }

        /// <summary>
        /// This method should always be called by the class that is creating this food object
        /// </summary>
        public void InitObjectBasedOnType()
        {
            if (tag == "Food")
            {
                foodType = FoodTypes.NormalFood;
            }
            else if (tag == "GhostFood")
            {
                foodType = FoodTypes.GhostFood;
            }

            //print("FB ==> FoodType: " + foodType);

            //For normal foods
            if (foodType == FoodTypes.NormalFood)
            {
                currentMoveSpeed = normalFoodMoveSpeed;
                myCollider.radius *= normalFoodRadius;

                //bodyScaleRatio = normalFoodBodyScaleRatio;
                bodyScaleRatio = availableFoodSkins[selectedFoodSkinID].normalSize;

                animSpeed = normalAnimSpeed;
            }

            //For ghost foods
            if (foodType == FoodTypes.GhostFood)
            {
                currentMoveSpeed = ghostFoodMoveSpeed;
                myCollider.radius *= ghostFoodRadius;

                //bodyScaleRatio = ghostFoodBodyScaleRatio;
                bodyScaleRatio = availableFoodSkins[selectedFoodSkinID].bigSize;

                animSpeed = ghostAnimSpeed;

                //Performance optimization - Ghost Foods should disappear after a short while since they can't be destroyed other way and can become too many very soon
                Destroy(gameObject, 12f);
            }

            //Skin settings

            //Apply a random skin from all available skin in this category
            shape.GetComponent<SpriteRenderer>().sprite = availableFoodSkins[selectedFoodSkinID].availableIcons[Random.Range(0, availableFoodSkins[selectedFoodSkinID].availableIcons.Length)];

            //Apply color randomization if needed
            if (availableFoodSkins[selectedFoodSkinID].canUseRandomizedColors)
            {
                shape.GetComponent<SpriteRenderer>().color = availableFoodSkins[selectedFoodSkinID].availableColors[Random.Range(0, availableFoodSkins[selectedFoodSkinID].availableColors.Length)];
            }

            //Apply size variations
            float bodyScale = Random.Range(availableFoodSkins[selectedFoodSkinID].minRandomSize, availableFoodSkins[selectedFoodSkinID].maxRandomSize) * bodyScaleRatio;
            shape.transform.localScale = new Vector3(bodyScale, bodyScale, 1);

            /*if(isStylizedFood && availableIcons.Length > 0)
            {
                //Assign actual food sprites
                shape.GetComponent<SpriteRenderer>().sprite = availableIcons[Random.Range(0, availableIcons.Length)];
            }
            else
            {
                //Assign colors
                shape.GetComponent<SpriteRenderer>().color = availableColors[Random.Range(0, availableColors.Length)];
            } */

            //Body Size
            //float bodyScale = Random.Range(0.3f, 0.8f) * bodyScaleRatio;
            //shape.transform.localScale = new Vector3(bodyScale, bodyScale, 1);
        }

        public void OnDestroy()
        {
            if (foodType == FoodTypes.GhostFood)
            {
                FoodSpawner.instance.UpdateGhostFoodsInScene(-1);
            }
        }

        private void Update()
        {
            SizeAnimation();
        }

        /// <summary>
        /// We want our foods to animate (scale up/down) a bit to make the look more yummy!
        /// </summary>
        public void SizeAnimation()
        {
            bodyParent.transform.localScale = new Vector3(
                    Mathf.SmoothStep(startSize.x, targetSize.x, Mathf.PingPong(Time.time * GetAnimSpeed(), 1)),
                    Mathf.SmoothStep(startSize.x, targetSize.x, Mathf.PingPong(Time.time * GetAnimSpeed(), 1)),
                    startSize.z);
        }

        /// <summary>
        /// If a food objects is close enough, it can move towards the hunter automatically like a magnet.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public IEnumerator MoveTowardsPlayer(GameObject target)
        {
            if (isMoving)
                yield break;
            isMoving = true;

            float t = 0;
            while (t < 1)
            {
                //if target is dead, exit!
                if (!target)
                    yield break;

                t += Time.deltaTime * currentMoveSpeed;
                transform.position = new Vector3(
                    Mathf.SmoothStep(startPosition.x, target.transform.position.x, t),
                    Mathf.SmoothStep(startPosition.y, target.transform.position.y, t),
                    0);
                yield return 0;
            }
        }


        public void Absorb(GameObject target)
        {
            myCollider.enabled = false;
            trail.SetActive(true);

            StartCoroutine(MoveTowardsPlayer(target));

            //Pooling & reposition routine
            StartCoroutine(RepositionCo());          
        }

        public IEnumerator RepositionCo()
        {
            yield return new WaitForSeconds(0.25f);

            trail.SetActive(false);
            shape.SetActive(false);

            //Convert ghostfoods to normal food
            if (tag == "GhostFood")
            {
                //Option #1 - do not turn ghostfoods into food
                Destroy(gameObject);
                yield break;

                //Option #2 - reposition ghostfoods 
                //tag = "Food";
                //InitObjectBasedOnType();
            }

            //New setting based on game modes
            if (!GameController.canSpawnNewFoodFromPickedupFoods)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return new WaitForSeconds(2f);

            transform.position = GameController.instance.GetRandomPositionInMap();
            startPosition = transform.position;
            isMoving = false;
            trail.SetActive(true);
            shape.SetActive(true);
            myCollider.enabled = true;
        }

        public float GetAnimSpeed()
        {
            return animSpeed + rndAnimSpeed;
        }
    }


    [Serializable]
    public class FoodSkin
    {
        public float normalSize = 1f;       //1x
        public float bigSize = 3f;          //3x
        public float minRandomSize = 0.3f;
        public float maxRandomSize = 0.8f;

        [Space(25)]
        public bool canUseRandomizedColors = false;
        public Color[] availableColors;

        [Space(25)]
        public Sprite[] availableIcons;
    }
}
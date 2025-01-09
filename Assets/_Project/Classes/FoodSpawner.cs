using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// We need to spread some foods on the scene for player and bot snakes to eat. We also need to pool these food objects so we can just 
/// reposition them after they are being eaten. This way, we don't put extra weight on garbage collector's shoulder and improve the performance of the game.
/// </summary>

namespace SnakeWarzIO
{
    public class FoodSpawner : MonoBehaviour
    {
        public static FoodSpawner instance;

        //There is no limit for the food amount, but since these initial food objects are never destroyed, its adviced to
        //keep this between 500 ~ 1500
        public static int defaultAmountOfFoods = 600;   //Default value. Can be edited.
        public static int maximumFoods;                 //Do not edit. This should be set by GameController, based on the selected game mode.   

        [Header("Counters")]
        public int availableFoodsInScene;
        public int availableGhostFoodsInScene;

        [Header("Objects")]
        public GameObject[] availableFoods;

        [Header("Misc")]
        public GameObject foodsParent;


        private void Awake()
        {
            instance = this;

            availableFoodsInScene = 0;
            availableGhostFoodsInScene = 0;
        }

        void Start()
        {
            //We create and pool N number of food items before the game begins. Later on we just need to 
            //reposition these food items when they are picked by players or bots. 
            SpawnInitialFoods();
        }

        public void SpawnInitialFoods()
        {
            for (int i = 0; i < maximumFoods; i++)
            {
                GameObject newFood = Instantiate(availableFoods[0], GameController.instance.GetRandomPositionInMap(), Quaternion.Euler(0, 0, 0)) as GameObject;
                newFood.name = "Food-" + availableFoodsInScene;
                newFood.tag = "Food";
                newFood.GetComponent<FoodController>().InitObjectBasedOnType();
                newFood.transform.parent = foodsParent.transform;

                UpdateFoodsInScene(1);
            }
        }


        /// <summary>
        /// Spawn GhostFood
        /// </summary>
        /// <param name="spawnPosition"></param>
        public void SpawnGhostFood(Vector3 spawnPosition)
        {
            GameObject newGhostFood = Instantiate(availableFoods[0], spawnPosition, Quaternion.Euler(0, 0, 0)) as GameObject;
            newGhostFood.name = "GhostFood-" + availableFoodsInScene;
            newGhostFood.tag = "GhostFood";
            newGhostFood.GetComponent<FoodController>().InitObjectBasedOnType();
            newGhostFood.transform.parent = foodsParent.transform;

            UpdateGhostFoodsInScene(1);
        }

        public void UpdateFoodsInScene(int amount = 1)
        {
            availableFoodsInScene += amount;

            if (availableFoodsInScene < 0)
                availableFoodsInScene = 0;
        }

        public void UpdateGhostFoodsInScene(int amount = 1)
        {
            availableGhostFoodsInScene += amount;

            if (availableGhostFoodsInScene < 0)
                availableGhostFoodsInScene = 0;
        }

        public int GetFoodsInScene()
        {
            return availableFoodsInScene;
        }

        public int GetGhostFoodsInScene()
        {
            return availableGhostFoodsInScene;
        }
    }
}
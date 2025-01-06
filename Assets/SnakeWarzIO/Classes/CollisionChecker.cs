using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the game begins, we spawn a lot of potential SpawnPoints. 
/// These SpawnPoints utilize this CollisionChecker class to see if their radius is free from any snake. 
/// We need this data as we only want to spawn new snakes inside free SpanwPoints.
/// </summary>

namespace SnakeWarzIO
{
    public class CollisionChecker : MonoBehaviour
    {
        public bool isFree;

        private float searchRadius = 8.5f;
        private LayerMask snakeMask;
        private Collider[] snakes;

        private void Awake()
        {
            isFree = true;
        }

        void Start()
        {
            snakeMask = LayerMask.GetMask("Snake");
            InvokeRepeating(nameof(RunCollisionCheck), 0, 0.5f);
        }

        public void RunCollisionCheck()
        {
            if (Physics.CheckSphere(transform.position, searchRadius, snakeMask))
                isFree = false;
            else
                isFree = true;
        }
    }
}
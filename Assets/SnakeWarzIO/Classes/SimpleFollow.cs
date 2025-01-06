using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used on Minimap-Camera and makes it follow the player's movement.
/// </summary>

namespace SnakeWarzIO
{
    public class SimpleFollow : MonoBehaviour
    {
        private GameObject targetToFollow;

        void Start()
        {
            targetToFollow = GameObject.FindGameObjectWithTag("PlayerHead");
        }

        void LateUpdate()
        {
            if (!targetToFollow)
                return;

            transform.position = new Vector3(targetToFollow.transform.position.x, targetToFollow.transform.position.y, transform.position.z);
        }
    }
}
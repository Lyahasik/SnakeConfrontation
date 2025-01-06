using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DemoBodyparts are only used on Demo Snakes (in menu & shop scenes) in order to give the player a preview of the currently selected skin.
/// They also follow the demo head with a sinusoid movement.
/// </summary>

namespace SnakeWarzIO
{
    public class DemoBodypart : MonoBehaviour
    {
        //Starts from 0. Indicates the index number of this part on the holder
        public int partIndex;

        //The snake object that owns this bodypart
        public GameObject owner;

        //This is the target this bodypart needs to follow
        public Transform target;

        //Reference to body shape
        public SpriteRenderer bodyShape;

        //Movement variables
        private float velY;

        void LateUpdate()
        {
            DelayedFollow();
        }

        public void DelayedFollow()
        {
            transform.position = new Vector3(
                transform.position.x,
                Mathf.SmoothDamp(transform.position.y, target.position.y, ref velY, 0.1f),
                transform.position.z);
        }

    }
}
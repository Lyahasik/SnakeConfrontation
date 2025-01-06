using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The head gameobject of each bot contains 3 sensors. Each of these sensors cast a ray in a certain layer to watch for possible collisions. 
/// When a collision is likely to happen, these sensors can trigger the maneuver function on the main bot controller class.
/// </summary>

namespace SnakeWarzIO
{
    public class BotSensorController : MonoBehaviour
    {
        //For the best performance, keep this between 4f ~ 7f
        public float collisionDepth = 6.5f; //default: 5f

        //Do not edit items below!
        public int sensorID = 0;            
        public bool isTouchingAnything;        
        internal Vector3 collisionPoint;

        //Private vars
        private Ray ray;
        private RaycastHit hit;

        private void Awake()
        {
            isTouchingAnything = false;
            collisionPoint = Vector3.zero;
        }

        private void FixedUpdate()
        {
            ManageCollisions();
        }

        public void ManageCollisions()
        {
            int layerMask = 1 << 8;
            //layerMask = ~layerMask;

            ray = new Ray(this.transform.position, transform.TransformDirection(Vector3.right) * collisionDepth);
            if (Physics.Raycast(ray, out hit, collisionDepth, layerMask))
            {
                if(Application.isEditor)
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.red);

                //ignore collision with own bodyparts
                if (hit.transform.gameObject.tag == "BotBody")
                {
                    if (hit.transform.GetComponent<BodypartController>().owner == this.transform.root.gameObject)
                    {
                        //print("Ignore collision with our own bodyparts. RETURN!");
                        return;
                    }
                }

                //print("<b>Raycast hit: </b>" + this.transform.root.gameObject + " ==> " + hit.transform.gameObject + "/" + hit.transform.root.gameObject);

                //Set flags & col data
                isTouchingAnything = true;
                collisionPoint = hit.point;

                //At this point, we need to change direction to avoid collision with others.
                //Only main sensor (ID = 0, front) can issue the turn command
                if (sensorID != 0)
                    return;

                //Case #Default - turn 180 degree
                transform.root.gameObject.GetComponent<BotController>().QuickManeuver(collisionPoint);
            }
            else
            {
                if (Application.isEditor)
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * collisionDepth, Color.white);

                //unset flag & col data
                isTouchingAnything = false;
                collisionPoint = Vector3.zero;
            }
        }
    }
}
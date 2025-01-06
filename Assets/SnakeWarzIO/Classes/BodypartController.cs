using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bodyparts need to follow their targets at all times.
/// The first bodypart will always use the head as its target. Second to last bodyparts will follow the bodypart before them as their target.
/// </summary>

namespace SnakeWarzIO
{
    public class BodypartController : MonoBehaviour
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
        private float velX;
        private float velY;
        private float rotDump = 10f;


        private void Awake()
        {

        }

        void LateUpdate()
        {
            //Graceful
            SmoothFollow();
        }

        public void SmoothFollow()
        {
            transform.position = new Vector3(
                Mathf.SmoothDamp(transform.position.x, target.position.x, ref velX, owner.GetComponent<Snake>().bodypartsFollowDelay),
                Mathf.SmoothDamp(transform.position.y, target.position.y, ref velY, owner.GetComponent<Snake>().bodypartsFollowDelay),
                transform.position.z);

            /*transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * 15),
                Mathf.Lerp(transform.position.y, target.position.y, Time.deltaTime * 15),
                transform.position.z);*/

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(target.rotation.eulerAngles.x, target.rotation.eulerAngles.y, target.rotation.eulerAngles.z), Time.deltaTime * rotDump);
        }

    }
}
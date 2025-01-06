using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeWarzIO
{
    public class MagnetCollisionController : MonoBehaviour
    {
        public GameObject rootParent;

        void Start()
        {

        }

        void Update()
        {

        }

        public void OnTriggerEnter(Collider other)
        {
            //print("Magnet collision report: " + other.name);

            if (other.gameObject.tag == "Food" || other.gameObject.tag == "GhostFood")
            {
                //print("Magnet collision report: " + other.name);

                if (rootParent.tag == "PlayerHead")
                {
                    rootParent.GetComponent<PlayerController>().OnMagnetTriggerEnter(other);
                }
            }
        }

    }
}
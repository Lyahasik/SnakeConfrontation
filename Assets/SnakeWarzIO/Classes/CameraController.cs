using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main camera needs to always follow the player's snake. We do the following in this class. We also update camera's FOV (or projection size) based on different criterias.
/// </summary>

namespace SnakeWarzIO
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;

        private GameObject targetToFollow;
        private float currentDistanceToTarget;  //Unused

        private Camera myCamera;
        private float baseFov;
        private float additionalFov;

        //Boosters related settings
        public const int unzoomExtraFov = 6;
        public static float currentUnzoomValue;

        private void Awake()
        {
            instance = this;
            myCamera = GetComponent<Camera>();
            baseFov = myCamera.orthographicSize;
            additionalFov = 0;
            currentUnzoomValue = 0;
        }

        void Start()
        {
            targetToFollow = GameObject.FindGameObjectWithTag("PlayerHead");    //Find the head of player's snake
        }

        void LateUpdate()
        {
            CalculateDistanceToTarget();
            FollowPlayer();            
            UpdateFOV();
        }

        public void CalculateDistanceToTarget()
        {
            if (!targetToFollow)
                return;

            currentDistanceToTarget = Vector3.Distance(transform.position, targetToFollow.transform.position);
            //print("currentDistanceToTarget: " + currentDistanceToTarget);
        }


        private float curVelX = 0;
        private float curVelY = 0;
        float smoothAmount = 0.3f;
        public void FollowPlayer()
        {
            if (!targetToFollow)
                return;

            transform.position = new Vector3(
                Mathf.SmoothDamp(transform.position.x, targetToFollow.transform.position.x, ref curVelX, smoothAmount),
                Mathf.SmoothDamp(transform.position.y, targetToFollow.transform.position.y, ref curVelY, smoothAmount),
                transform.position.z);
        }

        /// <summary>
        /// We need to move the camera further away from the player's snake as his snake grows bigger. This is needed as we have to give the player a better and wider view of the things that are happening around his snake.
        /// </summary>
        public void UpdateFOV()
        {
            if (GameController.isGameFinished || !GameController.isGameStarted)
                return;

            additionalFov = Mathf.Clamp((GameController.instance.GetMainPlayer().GetComponent<Snake>().totalBodyParts - GameController.initialBodyParts) / 75f, 0, 10);
            myCamera.orthographicSize = baseFov + additionalFov + currentUnzoomValue;
        }

        /// <summary>
        /// This is the last camera movement and happens when the game is over. 
        /// </summary>
        /// <param name="d"></param>
        public void Fadeout(float d)
        {
            StartCoroutine(FadeoutCo(d));
        }

        public IEnumerator FadeoutCo(float _d = 1f)
        {
            float currentFov = myCamera.orthographicSize;
            float targetFov = currentFov + 3.5f;    //+1.5f
            float t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / _d;
                myCamera.orthographicSize = Mathf.Lerp(currentFov, targetFov, t);
                yield return 0;
            }
        }


        public void StartUnzoom()
        {
            StartCoroutine(StartUnzoomCo(0, unzoomExtraFov));
        }

        public void EndUnzoom()
        {
            StartCoroutine(StartUnzoomCo(unzoomExtraFov, 0));
        }

        public IEnumerator StartUnzoomCo(float from, float to)
        {
            float t = 0;
            while(t < 1)
            {
                t += Time.deltaTime * 0.5f; //Takes 2 seconds to complete
                currentUnzoomValue = Mathf.SmoothStep(from, to, t);
                yield return null;
            }
        }

       
    }
}
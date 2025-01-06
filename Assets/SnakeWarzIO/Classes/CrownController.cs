using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Crown is a separate gameobject that always stick to and follows the snake with the longest body.
/// It will listen to leaderboard changes and as soon as a new leader is set, it will change its target automatically.
/// </summary>

namespace SnakeWarzIO
{
    public class CrownController : MonoBehaviour
    {
        public GameObject target;   //Crown should always follow the player/bot with the most score
        private int crownStatusUpdateCounter;
        private float scaleUpRatio = 1.02f;


        private void OnEnable()
        {
            IngameLeaderboardManager.OnLeaderboardUpdated += UpdateCrownStatus;
        }

        private void OnDisable()
        {
            IngameLeaderboardManager.OnLeaderboardUpdated -= UpdateCrownStatus;
        }

        void Awake()
        {
            crownStatusUpdateCounter = 0;
        }

        public void UpdateCrownStatus()
        {
            crownStatusUpdateCounter++;
            target = IngameLeaderboardManager.instance.GetTopPlayer();
            //print("Update crown status: " + crownStatusUpdateCounter + " & Target: " + target);
        }

        void Update()
        {
            if (!target)
            {
                transform.parent = null;
                transform.position = new Vector3(200, 200, 0);
                return;
            }

            //Otherwise, follow the target
            transform.position = target.transform.position;
            transform.localScale = new Vector3(target.GetComponent<Snake>().headPart.transform.localScale.x * scaleUpRatio, target.GetComponent<Snake>().headPart.transform.localScale.x * scaleUpRatio, 1);
        }
    }
}
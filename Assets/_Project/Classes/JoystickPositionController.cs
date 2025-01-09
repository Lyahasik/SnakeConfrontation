using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move the position of the virtual joystick handle on the screen based on the position of player input (click or touch)
/// </summary>

namespace SnakeWarzIO
{
    public class JoystickPositionController : MonoBehaviour
    {
        private float joystickDeadZone = 0.05f;
        private float offset = 3f;
        private Vector2 joystickHelperPosInScene;
        public IngameJoystick ingameJoystick;
        private Vector3 lastJoystickPos;

        private void Awake()
        {
            joystickHelperPosInScene = Vector2.zero;
            lastJoystickPos = Vector3.zero;
        }

        private void OnEnable()
        {
            Vector3 rndStartingPos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            joystickHelperPosInScene = lastJoystickPos = rndStartingPos;
            //print("Initial rndStartingPos: " + rndStartingPos);
        }

        public void Update()
        {
            if (!GameController.isGameStarted || GameController.isGameFinished || GameController.instance.GetMainPlayer() == null)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                //Register the position of last joystick position when player releases the input. It is needed for the normal update , since we need to continue moving player in its previous direction anyway.
                lastJoystickPos = joystickHelperPosInScene;
                //print("<b>Last Joystick pos is saved: " + lastJoystickPos + "</b>");
            }

            if (Input.GetMouseButton(0))
            {
                joystickHelperPosInScene.x = ingameJoystick.Horizontal;
                joystickHelperPosInScene.y = ingameJoystick.Vertical;

                if(joystickHelperPosInScene != Vector2.zero)
                {
                    lastJoystickPos = joystickHelperPosInScene;

                    //Debug position data
                    //print("-------> lastJoystickPos: " + lastJoystickPos);
                }

                //Debug position data
                //print("joystickHelperPosInScene: " + joystickHelperPosInScene);

                //If player input is not moving, or moving only a little, we can safely ignore it.
                if (Mathf.Abs(joystickHelperPosInScene.x) <= joystickDeadZone && Mathf.Abs(joystickHelperPosInScene.y) <= joystickDeadZone)
                {
                    if (lastJoystickPos != Vector3.zero)
                    {
                        joystickHelperPosInScene = lastJoystickPos;
                        //print("[true] joystickHelperPosInScene: " + joystickHelperPosInScene);
                    }
                    else
                    {
                        //print("[false] joystickHelperPosInScene: " + joystickHelperPosInScene);
                        return;
                    }
                }
            }
        }

        //New logic - update v1.3.1
        //Also altered in v1.3.2
        public void LateUpdate()
        {
            if (!GameController.isGameStarted || GameController.isGameFinished || GameController.instance.GetMainPlayer() == null)
                return;

            transform.position = GameController.instance.GetMainPlayer().transform.position + new Vector3(joystickHelperPosInScene.x * offset, joystickHelperPosInScene.y * offset, 0);

        }

        //Old Logic
        //Obsolete - Kept for backward compatibility or resolving merge issues
        /*public void Update()
        {
            if (!GameController.isGameStarted || GameController.isGameFinished || GameController.instance.GetMainPlayer() == null)
                return;

            if (Input.GetMouseButton(0))
            {
                joystickHelperPosInScene = Vector2.zero;
                joystickHelperPosInScene.x += ingameJoystick.Horizontal;
                joystickHelperPosInScene.y += ingameJoystick.Vertical;
            }

            transform.position = GameController.instance.GetMainPlayer().transform.position + new Vector3(joystickHelperPosInScene.x * offset, joystickHelperPosInScene.y * offset, 0);
        }*/

        
    }
}
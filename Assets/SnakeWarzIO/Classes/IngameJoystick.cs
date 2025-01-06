using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SnakeWarzIO
{
    public class IngameJoystick : Joystick
    {
        protected override void Start()
        {
            base.Start();
            background.gameObject.SetActive(false);

            //We don't need this if controlType is set to 1
            if (GameController.controlType == 1)
                gameObject.SetActive(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            background.gameObject.SetActive(false);
            base.OnPointerUp(eventData);
        }
    }
}
﻿using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Services.InputServices
{
    public class InputKeyboardMouseService : MonoBehaviour, IService
    {
        private readonly string _vertical = "Vertical";
        private readonly string _horizontal = "Horizontal";

        public Click _click = new Click();
        public Click Click { get => _click; }

        public float3 Axis
        {
            get
            {
                float3 axis = new float3();
                axis.x = Input.GetAxis(_horizontal);
                axis.z = Input.GetAxis(_vertical);

                return axis;
            }
        }

        public bool Shift =>
            Input.GetKey(KeyCode.LeftShift);

        public Vector3 ClickPosition
        {
            get
            {
                return Input.mousePosition;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Click.Active = true;
                Click.StaryPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                Click.Active = true;
                Click.EndPosition = Input.mousePosition;
            }
            else
            {
                Click.Up = false;
                Click.Active = false;
            }


            if (Input.GetMouseButtonUp(0))
            {
                Click.Up = true;
                Click.EndPosition = Input.mousePosition;
            }
        }
    }
}

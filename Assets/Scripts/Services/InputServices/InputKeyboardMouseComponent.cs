using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Services.InputServices
{
    public class InputKeyboardMouseService : IService
    {
        private readonly string _vertical = "Vertical";
        private readonly string _horizontal = "Horizontal";

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

        public bool Click
        {
            get
            {
                bool click = false;

                if (Input.GetMouseButton(0))
                    click = false;

                return click;
            }
        }
    }
}

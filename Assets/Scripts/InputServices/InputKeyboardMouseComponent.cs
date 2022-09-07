using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.InputServices
{
    public class InputKeyboardMouseService
    {
        private float3 _axis;
        private readonly string _vertical = "Vertical";
        private readonly string _horizontal = "Horizontal";

        public float3 Axis
        {
            get
            {
                _axis.x = Input.GetAxis(_horizontal);
                _axis.z = Input.GetAxis(_vertical);

                return _axis;
            }
        }
    }
}

using Assets.Scripts.Services.InputServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.Move
{
    public partial class MoveSystem : SystemBase
    {
        private InputKeyboardMouseService _inputKeyboardMouseService;

        protected override void OnStartRunning()
        {
            _inputKeyboardMouseService = new InputKeyboardMouseService();
        }

        protected override void OnUpdate()
        {
            float3 axis = _inputKeyboardMouseService.Axis;
            float deltaTime = Time.DeltaTime;

            Entities.ForEach((ref MoveComponent moveComponent, ref Translation translation) =>
            {
                translation.Value += moveComponent.Speed * axis * deltaTime;
            }).Run();
        }
    }
}
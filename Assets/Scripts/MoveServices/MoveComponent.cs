using Unity.Entities;

namespace Assets.Scripts.Move
{
    [GenerateAuthoringComponent]
    public struct MoveComponent : IComponentData
    {
        public float Speed;
    }
}
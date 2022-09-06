using Unity.Entities;

namespace Assets.Scripts.SelectableUnit
{
    [GenerateAuthoringComponent]
    public struct SelectionUIPrefabComponent : IComponentData
    {
        public Entity Value;
    }
}
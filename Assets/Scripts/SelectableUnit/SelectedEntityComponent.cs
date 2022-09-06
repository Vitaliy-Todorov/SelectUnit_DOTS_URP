using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.SelectableUnit
{
    [GenerateAuthoringComponent]
    internal struct SelectedEntityComponent : IComponentData
    {
        public Entity SelectionEntity;

        /*public void Log(Entity entity)
        {
            SelectionEntity = entity;
            Debug.Log($"Entity with tag: ({entity.Index}, {entity.Version})");
        }*/
    }
}
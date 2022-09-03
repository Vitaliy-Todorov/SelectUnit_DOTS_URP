using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.SelectableUnit
{
    [GenerateAuthoringComponent]
    internal struct SelectableEntityTag : IComponentData
    {
        public Entity Entity { get; set; }

        public void Log(Entity entity)
        {
            Entity = entity;
            Debug.Log($"Entity with tag: ({entity.Index}, {entity.Version})");
        }
    }
}
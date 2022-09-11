using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.SelectableUnit
{
    [GenerateAuthoringComponent]
    internal struct SelectedEntityComponent : IComponentData
    {
        public bool Active;
        public bool DestroySelection;
        public Entity SelectionEntity;
    }
}
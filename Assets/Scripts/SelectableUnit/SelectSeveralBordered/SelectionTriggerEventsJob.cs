using Assets.Scripts.Move;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Assets.Scripts.SelectableUnit.SelectSeveralBordered
{
    [BurstCompile]
    public struct SelectionTriggerEventsJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<SelectionTriggerTag> SelectionColliderTag;
        public EntityCommandBuffer ECB;

        public void Execute(TriggerEvent triggerEvent)
        {
            // HashSet<Entity> selectEntities = SelectionColliderTag[entityA].SelectEntities;

            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (SelectionColliderTag.HasComponent(entityA))
                ECB.AddComponent<SelectedEntityComponent>(entityB);

            if (SelectionColliderTag.HasComponent(entityB))
                ECB.AddComponent<SelectedEntityComponent>(entityA);
        }
    }
}

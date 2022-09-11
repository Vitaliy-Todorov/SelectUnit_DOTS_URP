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
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (SelectionColliderTag.HasComponent(entityA))
            {
                // HashSet<Entity> selectEntities = SelectionColliderTag[entityA].SelectEntities;

                ECB.AddComponent<SelectedEntityComponent>(entityB);
                UnityEngine.Debug.Log($"SelectionTriggerEventsJob EntityBEntityB ({triggerEvent.EntityA.Index}, {triggerEvent.EntityA.Version}). EntityA ({triggerEvent.EntityB.Index}, {triggerEvent.EntityB.Version})");
            }

            if (SelectionColliderTag.HasComponent(entityB))
            {
                ECB.AddComponent<SelectedEntityComponent>(entityA);
                UnityEngine.Debug.Log($"SelectionTriggerEventsJob EntityA ({triggerEvent.EntityA.Index}, {triggerEvent.EntityA.Version}). EntityA ({triggerEvent.EntityB.Index}, {triggerEvent.EntityB.Version})");
            }
        }
    }
}

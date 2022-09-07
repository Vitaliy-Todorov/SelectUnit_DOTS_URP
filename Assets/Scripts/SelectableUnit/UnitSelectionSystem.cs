using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;
using Ray = UnityEngine.Ray;
using Unity.Transforms;
using Assets.Scripts.Move;
using ParallelWriter = Unity.Entities.EntityCommandBuffer.ParallelWriter;
using System;

namespace Assets.Scripts.SelectableUnit
{
    [AlwaysUpdateSystem]
    public partial class UnitSelectionSystem : SystemBase
    {
        private Camera _mainCamera;
        private BuildPhysicsWorld _buildPhysicsWorld;
        private Entity _selectionEntity;
        private BeginInitializationEntityCommandBufferSystem _BeginInitializationEcbSystem;

        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _selectionEntity = GetSingleton<SelectionUIPrefabComponent>().Value;
            _BeginInitializationEcbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecbBeginInitialization = _BeginInitializationEcbSystem.CreateCommandBuffer().AsParallelWriter();

            if (Input.GetMouseButtonDown(0))
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                    DestroySelectionAll(ecbBeginInitialization);

                SelectedSinglUnit(ecbBeginInitialization);
            }
        }

        private void DestroySelectionAll(ParallelWriter ecbBeginInitialization)
        {
            EntityManager entityManager = EntityManager;
            Entities.ForEach((Entity selectedEntity, ref SelectedEntityComponent selectedEntityComponent) => {
                DestroySelection(selectedEntity, selectedEntityComponent, ecbBeginInitialization);
            }).Run();
        }

        private void SelectedSinglUnit(ParallelWriter ecbBeginInitialization)
        {
            CollisionWorld  collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            float3 rayStart = ray.origin;
            float3 rayEnd = ray.GetPoint(20);

            if (Raycast(rayStart, rayEnd, collisionWorld, out RaycastHit raycastHit))
            {
                Entity hitEntity = _buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;

                if (HasComponent<SelectedEntityComponent>(hitEntity))
                {
                    SelectedEntityComponent selectionStateData = EntityManager.GetComponentData<SelectedEntityComponent>(hitEntity);
                    DestroySelection(hitEntity, selectionStateData, ecbBeginInitialization);
                }
                else
                    SetSelection(hitEntity, ecbBeginInitialization);
            }
        }

        private bool Raycast(float3 rayStart, float3 rayEnd, CollisionWorld collisionWorld, out RaycastHit raycastHit)
        {
            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = (uint) ECollisionLayers.Defolt,
                CollidesWith = (uint) ECollisionLayers.Unit,
            };

            RaycastInput raycastInput = new RaycastInput
            {
                Start = rayStart,
                End = rayEnd,
                Filter = filter
            };

            return collisionWorld.CastRay(raycastInput, out raycastHit);
        }

        private void SetSelection(Entity entity, ParallelWriter ecbBeginInitialization)
        {

            Entity selection = ecbBeginInitialization.Instantiate(entity.Index, _selectionEntity);
            SelectedEntityComponent selectedEntityComponent = new SelectedEntityComponent { SelectionEntity = selection };

            ecbBeginInitialization.AddComponent(entity.Index, entity, selectedEntityComponent);
            ecbBeginInitialization.AddComponent(entity.Index, entity, new MoveComponent { Speed = 8 });

            ecbBeginInitialization.AddComponent(entity.Index, selection, new Parent { Value = entity });
            ecbBeginInitialization.AddComponent(entity.Index, selection, new LocalToParent());
            ecbBeginInitialization.SetComponent(entity.Index, selection, new Translation { Value = new float3(0, -.49f, 0)});
        }

        private static void DestroySelection(Entity entity, SelectedEntityComponent selectionStateData, ParallelWriter ecbBeginInitialization)
        {
            ecbBeginInitialization.DestroyEntity(entity.Index, selectionStateData.SelectionEntity);
            ecbBeginInitialization.RemoveComponent<SelectedEntityComponent>(entity.Index, entity);
            ecbBeginInitialization.RemoveComponent<MoveComponent>(entity.Index, entity);
        }
    }
}

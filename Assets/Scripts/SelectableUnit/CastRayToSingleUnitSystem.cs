using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;
using Ray = UnityEngine.Ray;
using Assets.Scripts.Services;
using Assets.Scripts.Services.InputServices;

namespace Assets.Scripts.SelectableUnit
{
    [AlwaysUpdateSystem]
    public partial class CastRayToSingleUnitSystem : SystemBase
    {
        private InputKeyboardMouseService _inputKeyboardMouseService;
        private BuildPhysicsWorld _buildPhysicsWorld;
        private BeginInitializationEntityCommandBufferSystem _beginInitializationECBSystem;
        private EndInitializationEntityCommandBufferSystem _endInitializationECBSystem;
        private Click _click;
        private Camera _mainCamera;

        protected override void OnStartRunning()
        {
            base.OnCreate();

            _inputKeyboardMouseService = AllServices.Container.Single<InputKeyboardMouseService>();
            _buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            _beginInitializationECBSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _endInitializationECBSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();

            _mainCamera = Camera.main;
            _click = _inputKeyboardMouseService.Click;
        }

        protected override void OnUpdate()
        {
            var ecbBeginInitialization = _beginInitializationECBSystem.CreateCommandBuffer();
            var ecbEndInitialization = _endInitializationECBSystem.CreateCommandBuffer();

            if (_click.Up)
            {
                if (!_inputKeyboardMouseService.Shift)
                    DestroySelectionAll(ecbEndInitialization);

                CastRayAndSelection(ecbBeginInitialization);
            }
        }

        private void CastRayAndSelection(EntityCommandBuffer ecb)
        {
            CollisionWorld  collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;

            Ray ray = _mainCamera.ScreenPointToRay(_click.EndPosition);
            float3 rayStart = ray.origin;
            float3 rayEnd = ray.GetPoint(20);

            if (CastRay(rayStart, rayEnd, collisionWorld, out RaycastHit raycastHit))
            {
                Entity hitEntity = _buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;

                if (HasComponent<SelectedEntityComponent>(hitEntity))
                {
                    SelectedEntityComponent selectionStateData = EntityManager.GetComponentData<SelectedEntityComponent>(hitEntity);
                    DestroySelection(hitEntity, selectionStateData, ecb);
                }
                else
                    SetSelection(hitEntity, ecb);
            }
        }

        private bool CastRay(float3 rayStart, float3 rayEnd, CollisionWorld collisionWorld, out RaycastHit raycastHit)
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

        private void SetSelection(Entity entity, EntityCommandBuffer ecb)
        {
            ecb.AddComponent<SelectedEntityComponent>(entity);
        }

        private void DestroySelectionAll(EntityCommandBuffer ecb)
        {
            Entities.ForEach((Entity selectedEntity, ref SelectedEntityComponent selectedEntityComponent) => {
                DestroySelection(selectedEntity, selectedEntityComponent, ecb);
            }).WithoutBurst().Run();
        }

        private void DestroySelection(Entity entity, SelectedEntityComponent selectionStateData, EntityCommandBuffer ecb)
        {
            selectionStateData.DestroySelection = true;
            ecb.SetComponent(entity, selectionStateData);
        }
    }
}

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;
using Ray = UnityEngine.Ray;

namespace Assets.Scripts.SelectableUnit
{
    [AlwaysUpdateSystem]
    public partial class UnitSelectionSystem : SystemBase
    {
        private Camera _mainCamera;
        private BuildPhysicsWorld _buildPhysicsWorld;
        private CollisionWorld _collisionWorld;
        private Entity _selectionEntity;

        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            _selectionEntity = GetSingleton<SelectionUIPrefabComponent>().Value;

            if (Input.GetMouseButtonDown(0))
                SelectedSinglUnit();
        }

        private void SelectedSinglUnit()
        {
            _collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            float3 rayStart = ray.origin;
            float3 rayEnd = ray.GetPoint(20);

            if (Raycast(rayStart, rayEnd, out RaycastHit raycastHit))
            {
                Entity hitEntity = _buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;

                if (HasComponent<SelectedEntityComponent>(hitEntity))
                    DestroySelection(hitEntity);
                else
                    SetSelection(hitEntity);
            }
        }

        private bool Raycast(float3 rayStart, float3 rayEnd, out RaycastHit raycastHit)
        {
            CollisionFilter filter = new CollisionFilter
            {
                BelongsTo = (uint) CollisionLayers.Defolt,
                CollidesWith = (uint) CollisionLayers.Unit,
            };

            RaycastInput raycastInput = new RaycastInput
            {
                Start = rayStart,
                End = rayEnd,
                Filter = filter
            };

            return _collisionWorld.CastRay(raycastInput, out raycastHit);
        }

        private void SetSelection(Entity entity)
        {
            EntityManager.AddComponent<SelectedEntityComponent>(entity);

            Entity selection = EntityManager.Instantiate(_selectionEntity);
            SelectedEntityComponent selectedEntityComponent = new SelectedEntityComponent { SelectionEntity = selection };

            EntityManager.SetComponentData(entity, selectedEntityComponent);
        }

        private void DestroySelection(Entity entity)
        {
            SelectedEntityComponent selectionStateData = EntityManager.GetComponentData<SelectedEntityComponent>(entity);
            EntityManager.DestroyEntity(selectionStateData.SelectionEntity);
            EntityManager.RemoveComponent<SelectedEntityComponent>(entity);

        }
    }
}

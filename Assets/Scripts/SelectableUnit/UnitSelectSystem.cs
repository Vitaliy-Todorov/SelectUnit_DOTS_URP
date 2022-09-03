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
    public partial class UnitSelectSystem : SystemBase
    {
        private Camera _mainCamera;
        private BuildPhysicsWorld _buildPhysicsWorld;
        private EntityManager _entityManager;
        private CollisionWorld _collisionWorld;

        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
                SelectSingleUnit();
        }

        private void SelectSingleUnit()
        {
            _collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            float3 rayStart = ray.origin;
            float3 rauEnd = ray.GetPoint(100f);

            if (Raycast(rayStart, rauEnd, out RaycastHit raycastHit))
            {
                Entity hitEntity = _buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;

                if (EntityManager.HasComponent<SelectableEntityTag>(hitEntity))
                    EntityManager.AddComponent<SelectableEntityTag>(hitEntity);
                Debug.Log($"{hitEntity} is select");
            }
        }

        private bool Raycast(float3 rayStart, float3 rayEnd, out RaycastHit raycastHit)
        {
            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = 1u,
                CollidesWith = 1u << 2
            };

            RaycastInput raycastInput = new RaycastInput
            {
                Start = rayStart,
                End = rayEnd,
                Filter = collisionFilter
            };

            return _collisionWorld.CastRay(raycastInput, out raycastHit);
        }
    }
}

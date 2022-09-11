using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections;
using Assets.Scripts.Services;
using Assets.Scripts.Services.InputServices;
using Unity.Jobs;

namespace Assets.Scripts.SelectableUnit.SelectSeveralBordered
{
    [AlwaysUpdateSystem]
    public partial class SelectMultipleUnitsSystem : SystemBase
    {
        private Camera _mainCamera;
        private BeginInitializationEntityCommandBufferSystem _beginInitializationECBSystem;
        private InputKeyboardMouseService _inputKeyboardMouseService;
        private Clicp _click;
        private EntityArchetype _physicsColliderForSelectionArchetype;

        /*private BuildPhysicsWorld _stepPhysicsWorld;*/
        private StepPhysicsWorld _stepPhysicsWorld;

        protected override void OnStartRunning()
        {
            base.OnCreate();

            _beginInitializationECBSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _inputKeyboardMouseService = AllServices.Container.Single<InputKeyboardMouseService>();

            _mainCamera = Camera.main;
            _click = _inputKeyboardMouseService.Click;
            _physicsColliderForSelectionArchetype = EntityManager.CreateArchetype(
                typeof(PhysicsCollider),                  
                typeof(LocalToWorld),
                typeof(PhysicsWorldIndex),
                typeof(SelectionTriggerTag));

            _stepPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            var ecbBeginInitialization = _beginInitializationECBSystem.CreateCommandBuffer();

            DesroyPhysicsColliderForSelection(ecbBeginInitialization);

            if (_click.Up && math.distance(_click.StaryPosition, _click.EndPosition) > 25)
                CreatePhysicsColliderForSelection(_click.StaryPosition, _click.EndPosition);

            StartTriggerEvents(ecbBeginInitialization);
        }

        private void CreatePhysicsColliderForSelection(float3 posiotion_1, float3 posiotion_2)
        {
            var topLeft = math.min(posiotion_1, posiotion_2);
            var botRight = math.max(posiotion_1, posiotion_2);

            var rect = Rect.MinMaxRect(topLeft.x, topLeft.y, botRight.x, botRight.y);

            var cornerRays = new[]
            {
                _mainCamera.ScreenPointToRay(rect.min),
                _mainCamera.ScreenPointToRay(rect.max),
                _mainCamera.ScreenPointToRay(new Vector2(rect.xMin, rect.yMax)),
                _mainCamera.ScreenPointToRay(new Vector2(rect.xMax, rect.yMin))
            };

            var vertices = new NativeArray<float3>(5, Allocator.Temp);

            for (var i = 0; i < cornerRays.Length; i++)
                vertices[i] = cornerRays[i].GetPoint(50f);

            vertices[4] = _mainCamera.transform.position;

            var collisionFilter = new CollisionFilter
            {
                BelongsTo = (uint) ECollisionLayers.Defolt,
                CollidesWith = (uint) ECollisionLayers.Unit,
            };

            var physicsMaterial = Unity.Physics.Material.Default;
            physicsMaterial.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;

            var selectionCollider = ConvexCollider.Create(vertices,
                                                          ConvexHullGenerationParameters.Default,
                                                          collisionFilter,
                                                          physicsMaterial);

            var newSelectionEntity = EntityManager.CreateEntity(_physicsColliderForSelectionArchetype);

            EntityManager.SetComponentData(newSelectionEntity, new PhysicsCollider { Value = selectionCollider });

        }

        private void StartTriggerEvents(EntityCommandBuffer ecbBeginInitialization)
        {
            SelectionTriggerEventsJob job = new SelectionTriggerEventsJob
            {
                SelectionColliderTag = GetComponentDataFromEntity<SelectionTriggerTag>(),
                ECB = ecbBeginInitialization
            };

            //Dependency = JobHandle.CombineDependencies(_stepPhysicsWorld.FinalSimulationJobHandle, Dependency);
            Dependency = job.Schedule(_stepPhysicsWorld.Simulation, Dependency);
            Dependency.Complete();
        }

        private void DesroyPhysicsColliderForSelection(EntityCommandBuffer ecbBeginInitialization)
        {
            if (HasSingleton<SelectionTriggerTag>())
            {
                Entity selectionTrigger = GetSingletonEntity<SelectionTriggerTag>();
                ecbBeginInitialization.DestroyEntity(selectionTrigger);
            }
        }
    }
}

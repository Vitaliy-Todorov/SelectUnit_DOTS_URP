using Assets.Scripts.Move;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ParallelWriter = Unity.Entities.EntityCommandBuffer.ParallelWriter;

namespace Assets.Scripts.SelectableUnit
{
    [AlwaysUpdateSystem]
    public partial class SelectableUnitSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _beginInitializationECDSystem;
        private EndSimulationEntityCommandBufferSystem _endSimulationECDSystem;
        private Entity _selectionEntity;
        private ComponentDataFromEntity<SelectedEntityComponent> _selectedEntityComponentCDFE;

        protected override void OnStartRunning()
        {
            _beginInitializationECDSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _endSimulationECDSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            _selectionEntity = GetSingleton<SelectionUIPrefabComponent>().Value;
        }

        protected override void OnUpdate()
        {
            var ecbBeginInitialization = _beginInitializationECDSystem.CreateCommandBuffer();
            var ecbEndSimulation = _endSimulationECDSystem.CreateCommandBuffer();

            Entity selectionEntity = _selectionEntity;
            //_selectedEntityComponentCDFE = GetComponentDataFromEntity<SelectedEntityComponent>();

            Entities.ForEach((Entity entity, ref SelectedEntityComponent selectedEntityComponent) =>
            {
                //if (_selectedEntityComponentCDFE.HasComponent(entity) && selectedEntityComponent.SelectionEntity.Index == 0)
                if (selectedEntityComponent.DestroySelection)
                    DestroySelection(entity, selectedEntityComponent, ecbEndSimulation);
                else if (!selectedEntityComponent.Active)
                    SetSelection(entity, selectedEntityComponent, selectionEntity, ecbBeginInitialization);
            }).WithoutBurst().Run();
        }

        private void SetSelection(Entity entity, SelectedEntityComponent selectedEntityComponent, Entity selectionEntity, EntityCommandBuffer ecb)
        {
            Entity selection = ecb.Instantiate(selectionEntity);

            selectedEntityComponent.Active = true;
            selectedEntityComponent.SelectionEntity = selection;
            ecb.SetComponent(entity, selectedEntityComponent);

            ecb.AddComponent(entity, new MoveComponent { Speed = 8 });

            ecb.AddComponent(selection, new Parent { Value = entity });
            ecb.AddComponent(selection, new LocalToParent());
            ecb.SetComponent(selection, new Translation { Value = new float3(0, -.49f, 0) });
            UnityEngine.Debug.Log($"SetSelection: {selectedEntityComponent.SelectionEntity.Index}.");
        }

        private void DestroySelection(Entity entity, SelectedEntityComponent selectionStateComponent, EntityCommandBuffer ecb)
        {
            ecb.DestroyEntity(selectionStateComponent.SelectionEntity);
            //ecb.SetComponent(entity, selectionStateComponent);
            ecb.RemoveComponent<SelectedEntityComponent>(entity);
            ecb.RemoveComponent<MoveComponent>(entity);
            UnityEngine.Debug.Log($"DestroySelection: {selectionStateComponent.SelectionEntity.Index} ------------------------------------------");
        }
    }
}
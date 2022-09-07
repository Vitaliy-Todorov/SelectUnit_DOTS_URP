using Assets.Scripts.SelectableUnit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[AlwaysUpdateSystem]
public partial class CommandBufferDetermineTestSystem : SystemBase
{
    protected override void OnCreate()
    {
        //base.OnCreate();

    }
    protected override void OnUpdate()
    {/*
        EntityManager entityManager = EntityManager;
        Entities.ForEach((Entity selectedEntity, ref SelectedEntityComponent selectedEntityComponent) => {
            SpawnEntity(selectedEntity, entityManager);
        }).Schedule(Dependency);*/

    }/*
    private static void SpawnEntity(Entity selectedEntity, EntityManager entityManager)
    {
    }*/
}
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Assets.Scripts.SelectableUnit
{
    [AlwaysUpdateSystem]
    public partial class GetUnitTegSystem : SystemBase
    {
        protected override void OnStartRunning()
        {
            Entities.ForEach(
                (Entity entity, ref SelectableEntityTag entityTag) =>
                {
                    entityTag.Log(entity);
                })
                .Schedule();
        }

        protected override void OnUpdate()
        {
        }
    }
}

using Unity.Entities;
using Unity.Jobs;

public partial class LifetimeSystem : SystemBase
{
    //Type: EntityCommandBuffer will popup various CommandBuffer Type
    private EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;

    protected override void OnCreate()
    {
        endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        //AsParallelWriter() because we use ecb in parallel
        var ecb = endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        //"entity" give entity that we are working on
        //entityInQueryIndex is entity Id that increment by one in each entity
        Entities.ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime) =>
        {
            lifetime.Value -= deltaTime;

            if (lifetime.Value <= 0)
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
                //ecb.Instantiate
                //ecb.AddComponent
                //ecb.RemoveComponent
            }
        }).ScheduleParallel();

        //creating/destroying an entity or adding/removing a component must create a SyncPoint
        endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;

public partial class PersonCollisionSystem : SystemBase
{
    private StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    struct PersonCollisionJob : ITriggerEventsJob
    {
        //These are input dependencies
        [ReadOnly] public ComponentDataFromEntity<PersonTag> PersonGroup;
        //Get data from entity, This variable will store all entity that has this component
        public ComponentDataFromEntity<URPMaterialPropertyBaseColor> ColorGroup;
        public float Seed;

        public void Execute(TriggerEvent triggerEvent)
        {
            bool isEntityAPerson = PersonGroup.HasComponent(triggerEvent.EntityA);
            bool isEntityBPerson = PersonGroup.HasComponent(triggerEvent.EntityB);

            if (!isEntityAPerson || !isEntityBPerson) return;
  
            var random = new Random((uint)((1 + Seed) + triggerEvent.BodyIndexA * triggerEvent.BodyIndexB));

            //Because it is struct so we need to return current value
            random = ChangeMaterialColor(random, triggerEvent.EntityA);
            ChangeMaterialColor(random, triggerEvent.EntityB);
        }

        private Random ChangeMaterialColor(Random random, Entity entity)
        {
            if (ColorGroup.HasComponent(entity))
            {
                //We can use "TryGetComponent(Entity entity, out T componentData);"
                var colorComponent = ColorGroup[entity];
                colorComponent.Value.x = random.NextFloat(0, 1);
                colorComponent.Value.y = random.NextFloat(0, 1);
                colorComponent.Value.z = random.NextFloat(0, 1);
                ColorGroup[entity] = colorComponent;
            }
            //Because it is struct so we need to return current value
            return random;
        }
    }

    protected override void OnUpdate()
    {
        Dependency = new PersonCollisionJob
        {
            PersonGroup = GetComponentDataFromEntity<PersonTag>(true),
            ColorGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
            Seed = System.DateTimeOffset.Now.Millisecond //Get system time to random seed
        }.Schedule(stepPhysicsWorld.Simulation, Dependency);
    }
}

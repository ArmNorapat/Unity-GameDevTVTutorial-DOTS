using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class NewDestinationSystem : SystemBase
{
    private RandomSystem randomSystem;

    protected override void OnCreate()
    {
        randomSystem = World.GetExistingSystem<RandomSystem>();
    }

    protected override void OnUpdate()
    {
        var randomArray = randomSystem.RandomArray;

        Entities
            //Complier may concern that this randomArray might be changed in runtime
            //Tell that we know it safe for randomArray
            .WithNativeDisableParallelForRestriction(randomArray) 
            //nativeThreadIndex is the fix name that dots knows what this params is
            .ForEach((int nativeThreadIndex, ref Destination destination, in Translation translation) => 
        {
            float distance = math.abs(math.length(destination.Value - translation.Value));

            if (distance < 0.1f)
            {
                var random = randomArray[nativeThreadIndex];

                destination.Value.x = random.NextFloat(0, 500);
                destination.Value.z = random.NextFloat(0, 500);

                randomArray[nativeThreadIndex] = random;
            }
        }).ScheduleParallel();
    }
}

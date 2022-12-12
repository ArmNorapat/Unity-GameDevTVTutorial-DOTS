using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class MoveToDestinationSystem : SystemBase
{
    //Stuff in OnUpdate run on Main Thread
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime; //Only receive on Main Thread

        //Get all of entities that have these Components
        //Order is important, keyword ref before your in
        //ref -> can read and write
        //in -> can read but cannot write (Readonly)
        //This line run event on different thread, multiple different threads
        Entities.ForEach((ref Translation translation, ref Rotation rotation,
        in Destination destination, in MovementSpeed movementSpeed) =>
        {
            //Modify translation and rotation once already there will make entity disappear
            //math.all() is -> If float3 equal
            if (math.all(destination.Value == translation.Value))
            {
                return;
            }
            // Implement the work to perform for each entity here.
            float3 toDestination = destination.Value - translation.Value;

            rotation.Value = quaternion.LookRotation(toDestination, new float3(0, 1, 0));

            float3 movement = math.normalize(toDestination) * movementSpeed.Value * deltaTime;

            //Compare destination
            if (math.length(movement) >= math.length(toDestination))
            {
                translation.Value = destination.Value;
            }
            else
            {
                translation.Value += movement;
            }
        }).ScheduleParallel();
        //.Schedule() run off the main thread
        //.ScheduleParallel() try and spread it across as many threads impossible, try to use all of other threads
        //.ScheduleParallel() But we can't use this every single time, some logic might not be able to be spread across multiple threads
        //.Run() run on the main thread
    }
}

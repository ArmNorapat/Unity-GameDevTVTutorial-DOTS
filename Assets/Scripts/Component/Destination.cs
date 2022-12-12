using Unity.Entities;
using Unity.Mathematics;

//Make a model behaviour version of this that we can stick onto game objects
[GenerateAuthoringComponent]
//Just a pure data component
public struct Destination : IComponentData
{
    //This is how you naming component that has One field
    public float3 Value;
}

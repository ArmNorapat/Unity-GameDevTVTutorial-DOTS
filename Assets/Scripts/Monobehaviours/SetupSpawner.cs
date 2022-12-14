using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SetupSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject personPrefab;

    [SerializeField]
    private int gridSize;

    [SerializeField]
    private float spread;

    [SerializeField]
    private Vector2 speedRange = new Vector2(4, 7);

    [SerializeField]
    private Vector2 lifetimeRange = new Vector2(10, 60);

    //Store some memory
    private BlobAssetStore blob;

    private void Start()
    {
        blob = new BlobAssetStore();
        //World.DefaultGameObjectInjectionWorld where we want object to be
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob);
        var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(personPrefab, settings);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                var instance = entityManager.Instantiate(entity);

                float3 position = new float3(x * spread, 0, z * spread);
                entityManager.SetComponentData(instance, new Translation { Value = position });
                entityManager.SetComponentData(instance, new Destination { Value = position });
                
                float lifetime = UnityEngine.Random.Range(lifetimeRange.x, lifetimeRange.y);
                entityManager.SetComponentData(instance, new Lifetime { Value = lifetime });
                
                float speed = UnityEngine.Random.Range(speedRange.x, speedRange.y);
                entityManager.SetComponentData(instance, new MovementSpeed { Value = speed });
            }
        }
    }

    private void OnDestroy()
    {
        blob.Dispose();
    }
}

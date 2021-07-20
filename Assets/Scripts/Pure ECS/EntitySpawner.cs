using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
public class EntitySpawner : MonoBehaviour
{
    [InspectorName("Current Spawn Mode")]
    [SerializeField] private Mode mode;

    [Header("MonoBehaviour")]
    [SerializeField] private GameObject monoObjectPrefab;
    [SerializeField] private GameObject monoObjectConversionPrefab;

    [Header("Unit")]
    [SerializeField] private Mesh demoMesh;
    [SerializeField] private Material demoMaterial;

    [Header("Range")]
    [SerializeField] private Range xSpawnUnitsRange;
    [SerializeField] private Range ySpawnUnitsRange;
    [SerializeField] private Range zSpawnUnitsRange;

    [Header("Amount")]
    [Range(0, 100000)]
    [SerializeField] private int currentUnitAmount;

    private World defaultWorld;
    private EntityManager entityManager;

    private EntityArchetype entityArchetype;

    private GameObjectConversionSettings settings;
    private Entity entityPrefab;

    private List<Entity> entities = new List<Entity>();

    private void Start()
    {
        SpawnEntities();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            AddUnits(5000, false);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            RemoveUnits(7500);
    }

    private void SpawnEntities()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        switch (mode)
        {
            case Mode.ECS_Conversion:
            {
                settings = GameObjectConversionSettings.FromWorld(defaultWorld, null) ;
                entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(monoObjectConversionPrefab, settings);
            }
            break;
            case Mode.ECS_Pure:
            {
                entityArchetype = entityManager.CreateArchetype(
                    typeof(Translation),
                    typeof(Rotation),
                    typeof(RenderMesh),
                    typeof(RenderBounds),
                    typeof(LocalToWorld)
                );
            } break;
        }

        AddUnits(currentUnitAmount, true);
    }
    private void AddUnits(int unitsAmount, bool initialize)
    {
        if(!initialize)
            currentUnitAmount += unitsAmount;

        for (int i = 0; i < unitsAmount; i++)
        {
            switch (mode)
            {
                case Mode.Classic:
                {
                    Vector3 objectPosition = new Vector3
                    (
                       UnityEngine.Random.Range(xSpawnUnitsRange.min, xSpawnUnitsRange.max),
                       UnityEngine.Random.Range(ySpawnUnitsRange.min, ySpawnUnitsRange.max),
                       UnityEngine.Random.Range(zSpawnUnitsRange.min, zSpawnUnitsRange.max)
                    );
                    Instantiate(monoObjectPrefab, objectPosition, Quaternion.identity, null);
                }
                break;
                case Mode.ECS_Conversion:
                {
                    float3 entityPosition = new float3
                    (
                        UnityEngine.Random.Range(xSpawnUnitsRange.min, xSpawnUnitsRange.max),
                        UnityEngine.Random.Range(ySpawnUnitsRange.min, ySpawnUnitsRange.max),
                        UnityEngine.Random.Range(zSpawnUnitsRange.min, zSpawnUnitsRange.max)
                    );
                    Entity demoEntity = entityManager.Instantiate(entityPrefab);
                    entityManager.SetComponentData(demoEntity, new Translation() { Value = entityPosition });
                    entityManager.SetName(demoEntity, "demoEntityConversion");

                    entities.Add(demoEntity);
                }
                break;
                case Mode.ECS_Pure:
                {
                    Entity demoEntity = entityManager.CreateEntity(entityArchetype);
                    float3 entityPosition = new float3
                    (
                        UnityEngine.Random.Range(xSpawnUnitsRange.min, xSpawnUnitsRange.max),
                        UnityEngine.Random.Range(ySpawnUnitsRange.min, ySpawnUnitsRange.max),
                        UnityEngine.Random.Range(zSpawnUnitsRange.min, zSpawnUnitsRange.max)
                    );
                    entityManager.AddComponentData(demoEntity, new Translation() { Value = entityPosition });
                    entityManager.AddComponentData(demoEntity, new Scale() { Value = .75f });
                    entityManager.SetName(demoEntity, "demoEntityPure");
                    entityManager.AddSharedComponentData(demoEntity, new RenderMesh()
                    {
                        mesh = demoMesh,
                        material = demoMaterial
                    });

                    entities.Add(demoEntity);
                }
                break;
            }
        }

        print("Current Units Amount: " + currentUnitAmount);
    }
    private void RemoveUnits(int unitsAmount)
    {
        unitsAmount = Mathf.Clamp(unitsAmount, 0, entities.Count);
        for(int i = 0; i < unitsAmount; i++)
        {
            Entity entity = entities[UnityEngine.Random.Range(0, entities.Count)];
            entities.Remove(entity);
            entityManager.DestroyEntity(entity);
        }
    }
}
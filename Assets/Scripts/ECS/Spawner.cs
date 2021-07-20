using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GridPosition gridPosition = GridPosition.Origin;
    [SerializeField] private GameObject gameObjectPrefab;

    [SerializeField] private Vector2 gridSize;

    private World defaultWorld;
    private EntityManager entityManager;
    private Entity entityPrefab;

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObjectPrefab, settings);

        InstantiateEntityGrid((int)gridSize.x, (int)gridSize.y, .5f);
    }

    private void InstantiateEntityGrid(int x, int z, float spacing = 1)
    {
        int fromI = 0, fromJ = 0, toI = 0, toJ = 0;
        switch (gridPosition)
        {
            case GridPosition.Center:
            {
                fromI = -x / 2; 
                fromJ = -z / 2;
                toI = x / 2;
                toJ = z / 2;
            } break;
            default:
            {
                fromI = fromJ = 0;
                toI = x;
                toJ = z;
            } break;
        }

        for(int i = fromI; i < toI; i++)
        {
            for(int j = fromJ; j < toJ; j++)
            {
                Entity newEntity = entityManager.Instantiate(entityPrefab);
                entityManager.SetComponentData(newEntity, new Translation() { Value = new float3(i * spacing, 0, j * spacing) });
            }
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "BasicObjectSpawnerItem", menuName = "Shop/ObjectSpawnerItem")]
public class ObjectSpawnerItem : StructureShopItem
{
    public int minNumberOfObjects = 8;
    public int maxNumberOfObjects = 32;
    public LayerMask objectLayer;
    public GameObject itemToSpawn;
    public float overlapCheckRadius = 0.5f;

    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 2f;


    
}


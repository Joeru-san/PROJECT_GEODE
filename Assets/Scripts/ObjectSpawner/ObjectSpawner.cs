using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(BoxCollider))]
public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] int minNumberOfObjects = 8;
    [SerializeField] int maxNumberOfObjects = 32;
    [SerializeField] LayerMask objectLayer;
    public GameObject itemToSpawn;
    [SerializeField] float overlapCheckRadius = 0.5f; // tunable in Inspector

    [SerializeField] Transform SpawnHeight;

    [Header("Delay Settings")]
    [SerializeField] float minSpawnDelay = 0.5f;
    [SerializeField] float maxSpawnDelay = 2f;
    [SerializeField] float spawnCycleDelay = 10f;

    const int MaxAttempts = 200;

    BoxCollider _box;
    Vector3 _localMin;
    Vector3 _localMax;
    int _targetCount;
    int _spawnedCount;
    string _nameOfSpawnedObject;

    void Awake()
    {
        _box = GetComponent<BoxCollider>();

        _localMin = _box.center - _box.size * 0.5f;
        _localMax = _box.center + _box.size * 0.5f;

        _targetCount = Random.Range(minNumberOfObjects, maxNumberOfObjects + 1);
        Debug.Log($"[ObjectSpawner] {name} will spawn up to {_targetCount} objects.");

        _nameOfSpawnedObject = itemToSpawn.GetComponent<Item>().scriptableObjectType.name;
    }

    void Start()
    {
        RegisterAndSpawn();
    }

    /// <summary>
    /// Register the object that we want to spawn in the pool
    /// </summary>
    void RegisterAndSpawn()
    {
        // TELL the pooler to create the pool right now
        Pool newPool = new Pool(_nameOfSpawnedObject, itemToSpawn, _targetCount);
        ObjectPooler.inst.AddPool(newPool);

        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Spawn an object from the pool at a random delay
    /// A coroutine that manages a repeating object spawn cycle.
    /// Spawns objects one by one up to a target count using an object pool,
    /// then waits for all spawned objects to be collected before restarting.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnLoop()
    {
        // Wait one frame to ensure initialization is fully finished
        yield return null; 

        while (_spawnedCount < _targetCount)
        {
            if (TryGetFreePosition(out Vector3 position))
            {
                GameObject reference = ObjectPooler.inst.SpawnFromPool(_nameOfSpawnedObject, position, Quaternion.identity, transform);
                _spawnedCount++;
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(spawnCycleDelay);
        yield return new WaitUntil(() => transform.childCount == 0);
        _spawnedCount = 0;
        Debug.Log($"Object spawner: {gameObject.name} is restarting the spawn cycle");
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Attempts to find a free spawn position by repeatedly sampling random positions
    /// and checking for overlapping colliders.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    bool TryGetFreePosition(out Vector3 result)
    {
        for (int i = 0; i < MaxAttempts; i++)
        {
            Vector3 candidate = GetRandomSpawnPosition();

            if (!Physics.CheckSphere(candidate, overlapCheckRadius, objectLayer))
            {
                result = candidate;
                return true;
            }
        }

        Debug.LogWarning($"[ObjectSpawner] {name} couldn't find a free position after {MaxAttempts} attempts.");
        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Take a random position in the box collider
    /// </summary>
    /// <returns></returns>
    Vector3 GetRandomSpawnPosition()
    {
        Vector3 local = new Vector3(
            Random.Range(_localMin.x, _localMax.x),
            SpawnHeight != null ? SpawnHeight.transform.position.y : 1f,
            Random.Range(_localMin.z, _localMax.z)
        );
        return transform.TransformPoint(local);
    }

    void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.green;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
        Gizmos.matrix = old;
        Gizmos.color = Color.white;
    }
}
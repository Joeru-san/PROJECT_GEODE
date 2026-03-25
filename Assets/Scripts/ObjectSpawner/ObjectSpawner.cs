using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] int minNumberOfObjects = 8;
    [SerializeField] int maxNumberOfObjects = 32;
    [SerializeField] LayerMask objectLayer;
    [SerializeField] GameObject itemToSpawn;
    [SerializeField] float overlapCheckRadius = 0.5f; // tunable in Inspector

    [Header("Delay Settings")]
    [SerializeField] float minSpawnDelay = 0.5f;
    [SerializeField] float maxSpawnDelay = 2f;

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

    void RegisterAndSpawn()
    {
        _box = GetComponent<BoxCollider>();
        _localMin = _box.center - _box.size * 0.5f;
        _localMax = _box.center + _box.size * 0.5f;

        _targetCount = Random.Range(minNumberOfObjects, maxNumberOfObjects + 1);
        
        // Get the tag name (assuming Item script exists)
        _nameOfSpawnedObject = itemToSpawn.GetComponent<Item>().scriptableObjectType.name;

        // TELL the pooler to create the pool right now
        Pool newPool = new Pool(_nameOfSpawnedObject, itemToSpawn, _targetCount);
        ObjectPooler.inst.AddPool(newPool);

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        // Wait one frame to ensure initialization is fully finished
        yield return null; 

        while (_spawnedCount < _targetCount)
        {
            if (TryGetFreePosition(out Vector3 position))
            {
                ObjectPooler.inst.SpawnFromPool(_nameOfSpawnedObject, position, Quaternion.identity, transform);
                _spawnedCount++;
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

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

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 local = new Vector3(
            Random.Range(_localMin.x, _localMax.x),
            _localMin.y, 
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
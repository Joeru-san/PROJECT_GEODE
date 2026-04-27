using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] int minNumberOfEnemies = 8;
    [SerializeField] int maxNumberOfEnemies = 32;
    public GameObject enemyToSpawn;
    [SerializeField] float overlapCheckRadius = 0.5f; // tunable in Inspector

    [Header("Delay Settings")]
    [SerializeField] float minSpawnDelay = 0.5f;
    [SerializeField] float maxSpawnDelay = 2f;
    
    [Header("Enemy Targets")]
    public GameObject[] enemyTargets;

    const int MaxAttempts = 200;

    BoxCollider _box;
    Vector3 _localMin;
    Vector3 _localMax;
    int _targetCount;
    int _spawnedCount;
    string _nameOfSpawnedEnemy;

    void Awake()
    {
        _box = GetComponent<BoxCollider>();

        _localMin = _box.center - _box.size * 0.5f;
        _localMax = _box.center + _box.size * 0.5f;

        _targetCount = Random.Range(minNumberOfEnemies, maxNumberOfEnemies + 1);

        _nameOfSpawnedEnemy = enemyToSpawn.GetComponent<MonoBehaviour>().GetType().Name;
        Debug.Log($"[EnemySpawner] {name} will spawn up to {_targetCount} {_nameOfSpawnedEnemy}.");
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
        Pool newPool = new Pool(_nameOfSpawnedEnemy, enemyToSpawn, _targetCount);
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
                GameObject reference = ObjectPooler.inst.SpawnFromPool(_nameOfSpawnedEnemy, position, Quaternion.identity, transform);
                reference.GetComponent<Enemy>().currentTarget = enemyTargets[Random.Range(0, enemyTargets.Length)];
                _spawnedCount++;
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
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

            if (!Physics.CheckSphere(candidate, overlapCheckRadius, LayerMask.NameToLayer("Default")))
            {
                result = candidate;
                return true;
            }
        }

        Debug.LogWarning($"[EnemySpawner] {name} couldn't find a free position after {MaxAttempts} attempts.");
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
            1f, 
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

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] int minNumberOfEnemies = 8;
    [SerializeField] int maxNumberOfEnemies = 32;
    public GameObject enemyToSpawn;
    [SerializeField] float overlapCheckRadius = 0.5f;
    [SerializeField] [Range(1,6)] int numberOfWaves = 1;
    int _actualWaveNumber = 1;

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
    bool _isSpawning = false;

    void Awake()
    {
        DayNightController.OnDayStateChange += RestartWaves;
        _box = GetComponent<BoxCollider>();

        _localMin = _box.center - _box.size * 0.5f;
        _localMax = _box.center + _box.size * 0.5f;

        _targetCount = Random.Range(minNumberOfEnemies, maxNumberOfEnemies + 1);

        _nameOfSpawnedEnemy = enemyToSpawn.GetComponent<MonoBehaviour>().GetType().Name;
        Debug.Log($"[EnemySpawner] {name} will spawn up to {_targetCount} {_nameOfSpawnedEnemy}.");
    }

    void Start()
    {
        RegisterInObjectPooler();
    }

    /// <summary>
    /// Register the object that we want to spawn in the pool.
    /// The ObjectPooler will apply a 33% buffer on top of the requested size.
    /// </summary>
    void RegisterInObjectPooler()
    {
        Pool newPool = new Pool(_nameOfSpawnedEnemy, enemyToSpawn, _targetCount);
        ObjectPooler.inst.AddPool(newPool);
    }

    /// <summary>
    /// Coroutine that manages a repeating object spawn cycle.
    /// Spawns objects one by one up to a target count using an object pool,
    /// then waits for all spawned objects to be collected before restarting.
    /// </summary>
    IEnumerator SpawnLoop()
    {
        _isSpawning = true;

        // Wait one frame to ensure initialization is fully finished
        yield return null;

        while (_spawnedCount < _targetCount)
        {
            if (TryGetFreePosition(out Vector3 position))
            {
                GameObject reference = ObjectPooler.inst.SpawnFromPool(_nameOfSpawnedEnemy, position, Quaternion.identity, transform);

                // Guard: SpawnFromPool can return null if the pool is empty and auto-expansion failed
                if (reference == null)
                {
                    Debug.LogWarning($"[EnemySpawner] SpawnFromPool returned null for '{_nameOfSpawnedEnemy}'. Skipping this spawn.");
                }
                else
                {
                    reference.GetComponent<Enemy>().currentTarget = enemyTargets[Random.Range(0, enemyTargets.Length)];
                    _spawnedCount++;
                }
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitUntil(() => transform.childCount == 0);

        if (_actualWaveNumber >= numberOfWaves)
        {
            Debug.Log($"[EnemySpawner] {name} has completed all {numberOfWaves} wave(s). Stopping.");
            _isSpawning = false;
            yield break;
        }

        _actualWaveNumber++;
        _spawnedCount = 0;
        Debug.Log($"[EnemySpawner] {name} is starting wave {_actualWaveNumber} of {numberOfWaves}.");
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Starts the spawn cycle from wave 1, triggered by OnDayStateChange.
    /// If a SpawnLoop is already running, the event is ignored for this spawner.
    /// </summary>
    public void RestartWaves()
    {
        if (_isSpawning)
        {
            Debug.Log($"[EnemySpawner] {name} is still spawning — ignoring OnDayStateChange.");
            return;
        }

        _actualWaveNumber = 1;
        _spawnedCount = 0;
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Attempts to find a free spawn position by repeatedly sampling random positions
    /// and checking for overlapping colliders.
    /// </summary>
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
    /// Returns a random position within the box collider bounds.
    /// </summary>
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
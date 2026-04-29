using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int numberOfObjectsInPool;

    public Pool(string tag, GameObject prefab, int numberOfObjectsInPool)
    {
        this.tag = tag;
        this.prefab = prefab;
        this.numberOfObjectsInPool = numberOfObjectsInPool;
    }
}

public class ObjectPooler : MonoBehaviour
{
    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    // Stores the prefab reference for each tag so auto-expansion can instantiate new objects
    private Dictionary<string, GameObject> _prefabRegistry = new Dictionary<string, GameObject>();

    public static ObjectPooler inst;

    void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(gameObject);
            return;
        }
        inst = this;
        DontDestroyOnLoad(gameObject);

        if (poolDictionary == null) poolDictionary = new Dictionary<string, Queue<GameObject>>();
        if (_prefabRegistry == null) _prefabRegistry = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// Function that allows spawners to dynamically create a pool of the objects they need to spawn.
    /// If the pool already exists, it expands it. The requested size includes a 33% buffer on top.
    /// </summary>
    public void AddPool(Pool pool)
    {
        int bufferedSize = pool.numberOfObjectsInPool + Mathf.CeilToInt(pool.numberOfObjectsInPool / 3f);

        // Always keep the prefab registry up to date
        _prefabRegistry[pool.tag] = pool.prefab;

        // If the pool already exists, expand it
        if (poolDictionary.ContainsKey(pool.tag))
        {
            Debug.Log($"[{GetType().Name}] Pool '{pool.tag}' already exists. Expanding by {bufferedSize} objects.");

            for (int i = 0; i < bufferedSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                poolDictionary[pool.tag].Enqueue(obj);
            }
            return;
        }

        // Create the new pool with the buffered size
        Queue<GameObject> currentObjectPool = new Queue<GameObject>();
        for (int i = 0; i < bufferedSize; i++)
        {
            GameObject obj = Instantiate(pool.prefab, transform);
            obj.SetActive(false);
            currentObjectPool.Enqueue(obj);
        }

        poolDictionary.Add(pool.tag, currentObjectPool);
        Debug.Log($"[{GetType().Name}] Pool '{pool.tag}' initialized with {bufferedSize} objects ({pool.numberOfObjectsInPool} requested + 33% buffer).");
    }

    /// <summary>
    /// Spawn an object from the pool. If the pool is empty, it auto-expands by 33% of
    /// the current pool size (counting both active and queued objects) before spawning.
    /// </summary>
    /// <param name="tag"> The tag of the object we want to spawn </param>
    /// <param name="position"> The position where we want to spawn the object </param>
    /// <param name="rotation"> The rotation at which we want to spawn the object </param>
    /// <param name="parent"> The parent we want to set for the object, default is null (world hierarchy) </param>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"[{GetType().Name}] Pool with tag {tag} doesn't exist.");
            return null;
        }

        // Auto-expand if the queue is empty
        if (poolDictionary[tag].Count == 0)
        {
            if (!_prefabRegistry.TryGetValue(tag, out GameObject prefab))
            {
                Debug.LogError($"[{GetType().Name}] Pool {tag} is empty and no prefab is registered. Cannot auto-expand.");
                return null;
            }

            // Expand by 1/3 of the total pool size (active objects in scene + queued)
            int activeCount = transform.childCount; // rough estimate: all pooler children
            int expandAmount = Mathf.Max(1, Mathf.CeilToInt(activeCount / 3f));

            Debug.LogWarning($"[{GetType().Name}] Pool {tag} is empty. Auto-expanding by {expandAmount} objects.");

            for (int i = 0; i < expandAmount; i++)
            {
                GameObject newObj = Instantiate(prefab, transform);
                newObj.SetActive(false);
                poolDictionary[tag].Enqueue(newObj);
            }
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // Safety check: object may have been destroyed externally
        if (objectToSpawn == null)
        {
            Debug.LogWarning($"[{GetType().Name}] Dequeued object for tag {tag} was null (destroyed externally). Skipping.");
            return null;
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.SetParent(parent != null ? parent : transform);

        if (objectToSpawn.GetComponent<Item>())
            objectToSpawn.GetComponent<Item>().Initialize();

        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    /// <summary>
    /// Re-add an object to its pool to be reused.
    /// </summary>
    /// <param name="tag"> To which pool to re-add it </param>
    /// <param name="obj"> Which object to re-add </param>
    public void ReAddToPool(string tag, GameObject obj)
    {
        if (obj.activeSelf)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                obj.SetActive(false);
                poolDictionary[tag].Enqueue(obj);
                obj.transform.SetParent(transform);
            }
        }
    }
}
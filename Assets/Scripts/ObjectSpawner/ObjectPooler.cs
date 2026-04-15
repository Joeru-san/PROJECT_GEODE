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
        
        // Ensure dictionary is ready even if Awake runs late
        if(poolDictionary == null) poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    // This method allows Spawners to register pools dynamically
    /// <summary>
    /// Function that allow the spawners to dynamically create a pool of the objects they need to spawn
    /// </summary>
    /// <param name="pool"></param>
    public void AddPool(Pool pool)
    {
        // If the pool already exists, we skip this
        // TODO, expand the pool size of this pool
        if (poolDictionary.ContainsKey(pool.tag))
        {
            Debug.Log($"Pool with tag {pool.tag} already exists. Skipping registration.");
            return;
        }

        // Create the new pool of objects
        Queue<GameObject> currentObjectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.numberOfObjectsInPool; i++)
        {
            GameObject obj = Instantiate(pool.prefab, transform);
            obj.SetActive(false);
            currentObjectPool.Enqueue(obj);
        }

        // Add the new pool to the dictionary
        poolDictionary.Add(pool.tag, currentObjectPool);
        Debug.Log($"Pool {pool.tag} initialized with {pool.numberOfObjectsInPool} objects.");
    }


    /// <summary>
    /// Spawn from a pool an object
    /// </summary>
    /// <param name="tag"> The tag of the object we want to spawn </param>
    /// <param name="position"> The position where we want to spawn the object </param>
    /// <param name="rotation"> The rotation at we want to spawn the object </param>
    /// <param name="parent"> The parent we want to set for the object, default is null, so in world hierarchy</param>
    /// <returns></returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        
        // Safety check: if object was destroyed externally
        if (objectToSpawn == null) return null;

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        if(parent != null)
            objectToSpawn.transform.SetParent(parent);
        else 
            objectToSpawn.transform.SetParent(transform);
        
        if(objectToSpawn.GetComponent<Item>())
                    objectToSpawn.GetComponent<Item>().Initialize();
        objectToSpawn.SetActive(true);
        
        return objectToSpawn;
    }

    /// <summary>
    /// Readd an object to the pool to be reused
    /// </summary>
    /// <param name="tag"> To which pool i want to re-add it</param>
    /// <param name="obj"> Which object i wanto to re-add </param>
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
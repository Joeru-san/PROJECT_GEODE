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
    // Dictionary is now initialized immediately to avoid null refs
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
    public void AddPool(Pool pool)
    {
        // If the pool already exists, we might want to expand it or skip
        if (poolDictionary.ContainsKey(pool.tag))
        {
            Debug.Log($"Pool with tag {pool.tag} already exists. Skipping registration.");
            return;
        }

        Queue<GameObject> currentObjectPool = new Queue<GameObject>();
        for (int i = 0; i < pool.numberOfObjectsInPool; i++)
        {
            GameObject obj = Instantiate(pool.prefab, transform);
            obj.SetActive(false);
            currentObjectPool.Enqueue(obj);
        }

        poolDictionary.Add(pool.tag, currentObjectPool);
        Debug.Log($"Pool {pool.tag} initialized with {pool.numberOfObjectsInPool} objects.");
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
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
        objectToSpawn.SetActive(true);
        
        return objectToSpawn;
    }

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
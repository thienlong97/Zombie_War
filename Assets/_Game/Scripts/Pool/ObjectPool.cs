using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : GenericSingleton<ObjectPool>
{
    [System.Serializable]
    public class Pool
    {
        public PoolType type;
        public GameObject prefab;
        public int initialSize;
    }

    public List<Pool> pools;
    private Dictionary<PoolType, Queue<GameObject>> poolDictionary;

    public override void Awake()
    {
        base.Awake();
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                CreateNewObject(pool.type, pool.prefab, objectPool);
            }

            poolDictionary.Add(pool.type, objectPool);
        }
    }

    private void CreateNewObject(PoolType type, GameObject prefab, Queue<GameObject> pool)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public GameObject SpawnFromPool(PoolType type, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Pool of type {type} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[type];
        
        // If the pool is empty, create a new object
        if (pool.Count == 0)
        {
            Pool poolInfo = pools.Find(p => p.type == type);
            if (poolInfo != null)
            {
                CreateNewObject(type, poolInfo.prefab, pool);
            }
        }

        GameObject objectToSpawn = pool.Dequeue();

        // Reset transform
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        // Initialize pooled object if interface is implemented
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        pooledObj?.OnObjectSpawn();

        return objectToSpawn;
    }

    public void ReturnToPool(PoolType type, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(type))
        {
            Debug.LogWarning($"Pool of type {type} doesn't exist.");
            return;
        }

        IPooledObject pooledObj = objectToReturn.GetComponent<IPooledObject>();
        pooledObj?.OnObjectReturn();

        objectToReturn.SetActive(false);
        poolDictionary[type].Enqueue(objectToReturn);
    }
} 
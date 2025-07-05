using System.Collections;
using System.Collections.Generic;
using TD.Utilities;
using UnityEngine;

namespace TD.DesignPattern.ObjectPooling
{
    public class PoolManager : MonoBehaviour
    {
        #region ___PROPERTIES___
        private static PoolManager _instance;
        public static PoolManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    var obj = new GameObject("Pool Manager");
                    _instance = obj.AddComponent<PoolManager>();
                }
                return _instance;
            }
        }

        private Dictionary<GameObject, Pool> poolTables = new Dictionary<GameObject, Pool>();
        #endregion

        #region ___MAIN METHODS___
        /// <summary>
        /// Create a new pool for the prefab
        /// </summary>
        /// <param name="prefab">the prefab</param>
        /// <param name="initialSize">initial pool size for the prefab</param>
        /// <param name="maxSize">max Size for the pool</param>
        /// <param name="autoExpand">Flag to enable/disable auto Expand of the Pool</param>
        /// <param name="parent">parent object to hold the pool's intances</param>
        public Pool CreatePool(GameObject prefab, int initialSize, int maxSize = 100, bool autoExpand = false, Transform parent = null)
        {
            if (!poolTables.ContainsKey(prefab))
            {
                var pool = new Pool(prefab, initialSize, maxSize, autoExpand, parent);
                poolTables.Add(prefab, pool);
                Debug.Log($"Pool Manager; Create new Pool for {prefab.name}");
                return pool;
            }else
            {
                return poolTables[prefab];
            }
        }

        public bool HasPoolObject(GameObject obj)
        {
            var pooledObject = ComponentCache.GetComponent<PooledObject>(obj);
            return pooledObject != null && pooledObject.Pool != null && poolTables.ContainsKey(pooledObject.Prefab);
        }
        /// <summary>
        /// Retrieves an object from the pool
        /// </summary>
        /// <param name="prefab">Object to retrieve</param>
        /// <returns></returns>
        public GameObject GetObject(GameObject prefab, int initialSize = 1, int maxSize = 100, bool autoExpand = false, Transform parent = null)
        {
            if (prefab == null) return null;

            if(!poolTables.TryGetValue(prefab, out var pool))
            {
                pool = CreatePool(prefab, initialSize, maxSize, autoExpand, parent);
            }
            return pool?.GetObject();
        }
        /// <summary>
        /// Clears the pool for a specific prefab.
        /// </summary>
        public void ReturnObject(GameObject obj)
        {
            if (obj == null) return;

            var pooledObject = ComponentCache.GetComponent<PooledObject>(obj);
            if(pooledObject != null && pooledObject.Pool != null)
            {
                pooledObject.Pool.ReturnObject(obj);
            }else
            {
                Debug.LogWarning($"Pool Manager: object does not belongs to any pool: {obj.name}");
                GameObject.Destroy(obj);
            }
        }
        /// <summary>
        /// Clears the pool for a specific prefab.
        /// </summary>
        /// <param name="obj">Prefab's pool to be clear</param>
        /// <param name="destroyAll">true to destroy all prefabs in the pool and from the table, otherwise just disable instantiated obj</param>
        public void ClearPool(GameObject obj, bool destroyAll = true)
        {
            if (obj == null) return;

            if (poolTables.TryGetValue(obj, out var pool))
            {
                pool.Clear(destroyAll);
                if(destroyAll) poolTables.Remove(obj);
            }
        }
        /// <summary>
        /// Clears all pools manages by the poolManager
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in poolTables.Values)
            {
                pool.Clear();
            }
            poolTables.Clear();
        }
        #endregion
    }

}

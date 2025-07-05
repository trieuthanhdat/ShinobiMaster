using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TD.GenericObjectPooling
{
    public class GenericObjectPooling : MonoSingleton<GenericObjectPooling>
    {
        private Dictionary<System.Type, Queue<MonoBehaviour>> poolDictionary = new Dictionary<System.Type, Queue<MonoBehaviour>>();
        private Dictionary<System.Type, List<MonoBehaviour>> activeInstances = new Dictionary<System.Type, List<MonoBehaviour>>();


        private Dictionary<int, Queue<GameObject>> gameObjectPoolDictionary = new Dictionary<int, Queue<GameObject>>();
        private Dictionary<int, List<GameObject>> activeGameObjects = new Dictionary<int, List<GameObject>>();

        public T GetObjectFromPool<T>(T prefab, int initialSize = 1, Transform parent = null, bool allowActiveInstance = false) where T : MonoBehaviour
        {
            System.Type type = typeof(T);

            if (!poolDictionary.ContainsKey(type))
            {
                poolDictionary[type] = new Queue<MonoBehaviour>();
                activeInstances[type] = new List<MonoBehaviour>();
                for (int i = 0; i < initialSize; i++)
                {
                    T obj = CreateNewObject(prefab, parent);
                    ReturnObjectToPool(obj);
                }
            }

            T pooledObject;
            if (poolDictionary[type].Count > 0)
            {
                pooledObject = poolDictionary[type].Dequeue() as T;
            }
            else if (allowActiveInstance && activeInstances[type].Count > 0)
            {
                pooledObject = activeInstances[type][0] as T;
            }
            else
            {
                pooledObject = CreateNewObject(prefab, parent);
            }

            if (parent != null)
            {
                pooledObject.transform.SetParent(parent);
            }

            pooledObject.gameObject.SetActive(true);
            if (!activeInstances[type].Contains(pooledObject))
            {
                activeInstances[type].Add(pooledObject);
            }

            return pooledObject;
        }
        public T GetObjectFromPool<T>(T prefab, Transform target, bool allowActiveInstance = false) where T : MonoBehaviour
        {
            T pooledObject = GetObjectFromPool(prefab, allowActiveInstance: allowActiveInstance);

            pooledObject.transform.position = target.position;
            pooledObject.transform.SetParent(null);
            pooledObject.gameObject.SetActive(true);

            return pooledObject;
        }
        public T GetObjectFromPool<T>(T prefab, Vector3 position, Quaternion rotation, bool allowActiveInstance = false) where T : MonoBehaviour
        {
            T pooledObject = GetObjectFromPool(prefab, allowActiveInstance: allowActiveInstance);
            pooledObject.transform.position = position;
            pooledObject.transform.rotation = rotation;
            pooledObject.transform.SetParent(null);
            pooledObject.gameObject.SetActive(true);
            return pooledObject;
        }
        // Overload: GetObjectFromPool with only prefab
        public T GetObjectFromPool<T>(T prefab) where T : MonoBehaviour
        {
            return GetObjectFromPool(prefab, initialSize: 1, parent: null, allowActiveInstance: false);
        }

        // Overload: GetObjectFromPool with prefab and initialSize
        public T GetObjectFromPool<T>(T prefab, int initialSize) where T : MonoBehaviour
        {
            return GetObjectFromPool(prefab, initialSize, parent: null, allowActiveInstance: false);
        }

        // Overload: GetObjectFromPool with prefab and parent
        public T GetObjectFromPool<T>(T prefab, Transform parent) where T : MonoBehaviour
        {
            return GetObjectFromPool(prefab, initialSize: 1, parent: parent, allowActiveInstance: false);
        }

        // Overload: GetObjectFromPool with prefab, initialSize, and parent
        public T GetObjectFromPool<T>(T prefab, int initialSize, Transform parent) where T : MonoBehaviour
        {
            return GetObjectFromPool(prefab, initialSize, parent, allowActiveInstance: false);
        }

        // Overload: GetObjectFromPool with prefab, initialSize, and allowActiveInstance
        public T GetObjectFromPool<T>(T prefab, int initialSize, bool allowActiveInstance) where T : MonoBehaviour
        {
            return GetObjectFromPool(prefab, initialSize, parent: null, allowActiveInstance: allowActiveInstance);
        }

        public void ReturnObjectToPool<T>(T obj) where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(null);
            if (!poolDictionary.ContainsKey(type))
            {
                poolDictionary[type] = new Queue<MonoBehaviour>();
            }
            poolDictionary[type].Enqueue(obj);
            if (activeInstances.ContainsKey(type))
                activeInstances[type].Remove(obj);
        }

        private T CreateNewObject<T>(T prefab, Transform parent) where T : MonoBehaviour
        {
            T newObj = Instantiate(prefab, parent);
            newObj.gameObject.SetActive(false);
            return newObj;
        }

        public void PreloadPool<T>(T prefab, int preloadCount, Transform parent = null) where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            if (!poolDictionary.ContainsKey(type))
            {
                poolDictionary[type] = new Queue<MonoBehaviour>();
                activeInstances[type] = new List<MonoBehaviour>();
            }

            for (int i = 0; i < preloadCount; i++)
            {
                T obj = CreateNewObject(prefab, parent);
                ReturnObjectToPool(obj);
            }
        }

        public void ClearPool<T>() where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            if (poolDictionary.ContainsKey(type))
            {
                while (poolDictionary[type].Count > 0)
                {
                    T obj = poolDictionary[type].Dequeue() as T;
                    Destroy(obj.gameObject);
                }
            }
            if (activeInstances.ContainsKey(type))
            {
                foreach (var instance in activeInstances[type])
                {
                    Destroy(instance.gameObject);
                }
                activeInstances[type].Clear();
            }
        }

        public void ClearAllPools()
        {
            foreach (var pool in poolDictionary)
            {
                while (pool.Value.Count > 0)
                {
                    MonoBehaviour obj = pool.Value.Dequeue();
                    Destroy(obj.gameObject);
                }
            }
            foreach (var active in activeInstances)
            {
                foreach (var instance in active.Value)
                {
                    Destroy(instance.gameObject);
                }
                active.Value.Clear();
            }
            poolDictionary.Clear();
            activeInstances.Clear();
        }

        /// <summary>
        /// Returns the number of pooled (inactive) objects for a given type.
        /// </summary>
        public int GetPooledCount<T>() where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            if (poolDictionary.TryGetValue(type, out var queue))
                return queue.Count;
            return 0;
        }

        /// <summary>
        /// Returns the number of active (in-use) objects for a given type.
        /// </summary>
        public int GetActiveCount<T>() where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            if (activeInstances.TryGetValue(type, out var list))
                return list.Count;
            return 0;
        }

        /// <summary>
        /// Returns all active instances of a given type.
        /// </summary>
        public IReadOnlyList<T> GetActiveInstances<T>() where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            if (activeInstances.TryGetValue(type, out var list))
                return list.ConvertAll(x => (T)x).AsReadOnly();
            return new List<T>().AsReadOnly();
        }

        /// <summary>
        /// Returns all pooled (inactive) instances of a given type.
        /// </summary>
        public IReadOnlyList<T> GetPooledInstances<T>() where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            if (poolDictionary.TryGetValue(type, out var queue))
                return new List<T>(queue.Count).Concat(queue).Cast<T>().ToList().AsReadOnly();
            return new List<T>().AsReadOnly();
        }

        /// <summary>
        /// Checks if a type is currently managed by the pool.
        /// </summary>
        public bool IsTypePooled<T>() where T : MonoBehaviour
        {
            return poolDictionary.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Returns all types currently managed by the pool.
        /// </summary>
        public IEnumerable<System.Type> GetAllPooledTypes()
        {
            return poolDictionary.Keys;
        }

        /// <summary>
        /// Returns the total number of pooled (inactive) objects across all types.
        /// </summary>
        public int GetTotalPooledCount()
        {
            int total = 0;
            foreach (var queue in poolDictionary.Values)
                total += queue.Count;
            return total;
        }

        /// <summary>
        /// Returns the total number of active (in-use) objects across all types.
        /// </summary>
        public int GetTotalActiveCount()
        {
            int total = 0;
            foreach (var list in activeInstances.Values)
                total += list.Count;
            return total;
        }

        /// <summary>
        /// Deactivates all active objects of a given type and returns them to the pool.
        /// </summary>
        public void ReturnAllActiveToPool<T>() where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            if (activeInstances.TryGetValue(type, out var list))
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    ReturnObjectToPool((T)list[i]);
                }
            }
        }

        /// <summary>
        /// Deactivates all active objects of all types and returns them to their pools.
        /// </summary>
        public void ReturnAllActiveToPool()
        {
            foreach (var type in activeInstances.Keys.ToList())
            {
                var list = activeInstances[type];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var obj = list[i];
                    obj.gameObject.SetActive(false);
                    obj.transform.SetParent(null);
                    poolDictionary[type].Enqueue(obj);
                }
                list.Clear();
            }
        }


        public GameObject GetGameObjectFromPool(GameObject prefab, int initialSize = 1, Transform parent = null, bool allowActiveInstance = false)
        {
            if (prefab == null)
                throw new System.ArgumentNullException(nameof(prefab));

            int prefabInstanceID = prefab.GetInstanceID();

            // Initialize pool and active list if not present
            if (!gameObjectPoolDictionary.TryGetValue(prefabInstanceID, out var pool))
            {
                PreloadGameObjectPool(prefab, initialSize, parent);
            }

            var activeList = activeGameObjects[prefabInstanceID];

            GameObject pooledObject = null;

            if (pool.Count > 0)
            {
                pooledObject = pool.Dequeue();
            }
            else if (allowActiveInstance && activeList.Count > 0)
            {
                pooledObject = activeList[0];
            }
            else
            {
                pooledObject = CreateNewGameObject(prefab, parent);
            }

            if (parent != null)
            {
                pooledObject.transform.SetParent(parent, false);
            }

            if (!pooledObject.activeSelf)
                pooledObject.SetActive(true);

            if (!activeList.Contains(pooledObject))
            {
                activeList.Add(pooledObject);
            }

            return pooledObject;
        }
        public GameObject GetGameObjectFromPool(GameObject prefab, Transform target, bool allowActiveInstance = false)
        {
            GameObject pooledObject = GetGameObjectFromPool(prefab, allowActiveInstance: allowActiveInstance);

            pooledObject.transform.position = target.position;
            pooledObject.transform.SetParent(null);
            return pooledObject;
        }
        public GameObject GetGameObjectFromPool(GameObject prefab, Vector3 position, Quaternion rotation, int initialSize = 1, bool allowActiveInstance = false)
        {
            GameObject pooledObject = GetGameObjectFromPool(prefab, initialSize, allowActiveInstance: allowActiveInstance);
            pooledObject.transform.position = position;
            pooledObject.transform.rotation = rotation;
            pooledObject.transform.SetParent(null);
            return pooledObject;
        }
        // Overload: GetObjectFromPool with only prefab
        public GameObject GetGameObjectFromPool(GameObject prefab)
        {
            return GetGameObjectFromPool(prefab, initialSize: 1, parent: null, allowActiveInstance: false);
        }

        // Overload: GetObjectFromPool with prefab and initialSize
        public GameObject GetGameObjectFromPool(GameObject prefab, int initialSize)
        {
            return GetGameObjectFromPool(prefab, initialSize, parent: null, allowActiveInstance: false);
        }

        // Overload: GetObjectFromPool with prefab and parent
        public GameObject GetGameObjectFromPool(GameObject prefab, Transform parent)
        {
            return GetGameObjectFromPool(prefab, initialSize: 1, parent: parent, allowActiveInstance: false);
        }

        // Overload: GetObjectFromPool with prefab, initialSize, and parent
        public GameObject GetGameObjectFromPool(GameObject prefab, int initialSize, Transform parent)
        {
            return GetGameObjectFromPool(prefab, initialSize, parent, allowActiveInstance: false);
        }

        // Overload: GetObjectFromPool with prefab, initialSize, and allowActiveInstance
        public GameObject GetGameObjectFromPool(GameObject prefab, int initialSize, bool allowActiveInstance)
        {
            return GetGameObjectFromPool(prefab, initialSize, parent: null, allowActiveInstance: allowActiveInstance);
        }

        public void ReturnGameObjectToPool(GameObject obj, GameObject prefab)
        {
            obj.SetActive(false);
            obj.transform.SetParent(null);
            int prefabInstanceID = prefab.GetInstanceID();
            if (!gameObjectPoolDictionary.ContainsKey(prefabInstanceID))
            {
                gameObjectPoolDictionary[prefabInstanceID] = new Queue<GameObject>();
            }
            gameObjectPoolDictionary[prefabInstanceID].Enqueue(obj);
            if (activeGameObjects.ContainsKey(prefabInstanceID))
                activeGameObjects[prefabInstanceID].Remove(obj);
        }

        private GameObject CreateNewGameObject(GameObject prefab, Transform parent)
        {
            GameObject newObj = Instantiate(prefab, parent);
            newObj.SetActive(false);
            return newObj;
        }

        public void PreloadGameObjectPool(GameObject prefab, int preloadCount, Transform parent = null)
        {
            int prefabInstanceID = prefab.GetInstanceID();
            if (!gameObjectPoolDictionary.ContainsKey(prefabInstanceID))
            {
                gameObjectPoolDictionary[prefabInstanceID] = new Queue<GameObject>();
                activeGameObjects[prefabInstanceID] = new List<GameObject>();
            }

            for (int i = 0; i < preloadCount; i++)
            {
                GameObject obj = CreateNewGameObject(prefab, parent);
                ReturnGameObjectToPool(obj, prefab);
            }
        }

        public void ClearGameObjectPool(GameObject prefab)
        {
            int prefabInstanceID = prefab.GetInstanceID();
            if (gameObjectPoolDictionary.ContainsKey(prefabInstanceID))
            {
                while (gameObjectPoolDictionary[prefabInstanceID].Count > 0)
                {
                    GameObject obj = gameObjectPoolDictionary[prefabInstanceID].Dequeue();
                    Destroy(obj);
                }
            }
            if (activeGameObjects.ContainsKey(prefabInstanceID))
            {
                foreach (var instance in activeGameObjects[prefabInstanceID])
                {
                    Destroy(instance);
                }
                activeGameObjects[prefabInstanceID].Clear();
            }
        }

        public void ClearAllGameObjectPools()
        {
            foreach (var pool in gameObjectPoolDictionary)
            {
                while (pool.Value.Count > 0)
                {
                    GameObject obj = pool.Value.Dequeue();
                    Destroy(obj);
                }
            }
            foreach (var active in activeGameObjects)
            {
                foreach (var instance in active.Value)
                {
                    Destroy(instance);
                }
                active.Value.Clear();
            }
            gameObjectPoolDictionary.Clear();
            activeGameObjects.Clear();
        }

        public int GetPooledGameObjectCount(GameObject prefab)
        {
            if (gameObjectPoolDictionary.TryGetValue(prefab.GetInstanceID(), out var queue))
                return queue.Count;
            return 0;
        }

        public int GetActiveGameObjectCount(GameObject prefab)
        {
            if (activeGameObjects.TryGetValue(prefab.GetInstanceID(), out var list))
                return list.Count;
            return 0;
        }

        public IReadOnlyList<GameObject> GetActiveGameObjects(GameObject prefab)
        {
            if (activeGameObjects.TryGetValue(prefab.GetInstanceID(), out var list))
                return list.AsReadOnly();
            return new List<GameObject>().AsReadOnly();
        }

        public IReadOnlyList<GameObject> GetPooledGameObjects(GameObject prefab)
        {
            if (gameObjectPoolDictionary.TryGetValue(prefab.GetInstanceID(), out var queue))
                return new List<GameObject>(queue).AsReadOnly();
            return new List<GameObject>().AsReadOnly();
        }

        public bool IsGameObjectPooled(GameObject prefab)
        {
            return gameObjectPoolDictionary.ContainsKey(prefab.GetInstanceID());
        }

        public IEnumerable<int> GetAllPooledGameObjectPrefabIDs()
        {
            return gameObjectPoolDictionary.Keys;
        }

        public int GetTotalPooledGameObjectCount()
        {
            int total = 0;
            foreach (var queue in gameObjectPoolDictionary.Values)
                total += queue.Count;
            return total;
        }

        public int GetTotalActiveGameObjectCount()
        {
            int total = 0;
            foreach (var list in activeGameObjects.Values)
                total += list.Count;
            return total;
        }

        public void ReturnAllActiveGameObjectsToPool(GameObject prefab)
        {
            if (activeGameObjects.TryGetValue(prefab.GetInstanceID(), out var list))
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    ReturnGameObjectToPool(list[i], prefab);
                }
            }
        }

        public void ReturnAllActiveGameObjectsToPool()
        {
            foreach (var prefab in activeGameObjects.Keys.ToList())
            {
                var list = activeGameObjects[prefab];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var obj = list[i];
                    obj.SetActive(false);
                    obj.transform.SetParent(null);
                    gameObjectPoolDictionary[prefab].Enqueue(obj);
                }
                list.Clear();
            }
        }

    }

}

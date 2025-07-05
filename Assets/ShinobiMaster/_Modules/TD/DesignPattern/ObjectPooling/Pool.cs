using System;
using System.Collections;
using System.Collections.Generic;
using TD.Utilities;
using UnityEngine;


namespace TD.DesignPattern.ObjectPooling
{
    [System.Serializable]
    public class Pool
    {
        public GameObject Prefab { get; private set; }

        private Stack<GameObject> objects;
        private Transform parent;
        private int maxSize;
        private bool autoExpand;

        public event Action<GameObject> OnObjectRetrived;
        public event Action<GameObject> OnObjectReturn;

        public Pool(GameObject prefab, int intialSize, int maxSize, bool autoExpand, Transform parent = null)
        {
            this.Prefab = prefab;
            this.maxSize = maxSize;
            this.autoExpand = autoExpand;
            this.parent = parent;
            objects ??= new Stack<GameObject>(intialSize);
            Preload(intialSize);
        }

        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = CreateObject();
                ReturnObject(obj);
            }
        }
        public IEnumerator PreLoadAsync(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = CreateObject();
                ReturnObject(obj);
                if(i % 10 == 0)
                {
                    yield return null; //Yield control back to unity To prevent FPS drops
                }
            }
        }
        public GameObject GetObject()
        {
            GameObject obj;
            if(objects.Count > 0)
            {
                obj = objects.Pop();
            }else if(autoExpand && objects.Count >= maxSize)
            {
                obj = CreateObject();
                Debug.Log($"Pool Create new Instance for {obj.name}");
            }else
            {
                Debug.LogError($"Pool is Empty and cannot Expand: {Prefab.name}");
                return null;
            }
            obj.SetActive(true);
            OnObjectRetrived?.Invoke(obj);
            return obj;
        }
        public void ReturnObject(GameObject obj)
        {
            if (obj == null) return;

            if(objects.Count >= maxSize && !autoExpand)
            {
                GameObject.Destroy(obj);
            }else
            {
                obj.SetActive(false);
                obj.transform.SetParent(parent);
                objects.Push(obj);
            }
            Debug.Log($"Pool return object {obj.name}");
            OnObjectRetrived?.Invoke(obj);
        }
        public void Clear(bool destroyAll = true)
        {
            if(destroyAll)
            {
                foreach (var obj in objects)
                {
                    GameObject.Destroy(obj);
                }
                objects.Clear();
            }
            else
            {
                foreach (var obj in objects)
                {
                    obj.SetActive(false);
                }
            }
        }

        private GameObject CreateObject()
        {
            var obj = GameObject.Instantiate(Prefab, parent);
            if(!ComponentCache.TryGetComponent<PooledObject>(obj, out PooledObject pooledObject))
            {
                pooledObject = obj.AddComponent<PooledObject>();
            }
            
            pooledObject.Prefab = Prefab;
            pooledObject.Pool = this;
            obj.SetActive(false);
            return obj;
        }
    }
}

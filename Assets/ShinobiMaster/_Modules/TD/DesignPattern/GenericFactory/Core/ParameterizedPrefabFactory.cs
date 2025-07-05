using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TD.DesignPattern.GenericFactory.Core
{
    public class ParameterizedPrefabFactory<T, TData> : IParameterizedFactory<T, TData> where T : MonoBehaviour, IInitializable<TData>
    {
        protected readonly T prefab;
        protected readonly Transform parent;

        public ParameterizedPrefabFactory(T prefab, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;
        }
        public T Create(TData data)
        {
            T instance;
            if(parent != null)
            {
                instance = GameObject.Instantiate(prefab, parent);
            }else
            {
                instance = GameObject.Instantiate(prefab);
            }
            instance.Initialize(data);
            return instance;
        }
    }

}

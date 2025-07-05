using System;
using System.Collections;
using System.Collections.Generic;
using TD.DesignPattern.GenericBuilder;
using TD.DesignPattern.GenericBuilder.Core;
using TD.DesignPattern.GenericBuilder.Validatable;
using UnityEngine;

namespace TD.DesignPattern.GenericFactory.Builder
{
    public class PrefabFactoryWithBuilder<T, TData> : IParameterizedFactory<T, TData>, IBuilderFactory<T, PrefabBuilder<T>> where T : MonoBehaviour, IInitializable<TData>, IValidatable
    {
        private T _instance;
        private T _prefab;
        private Transform _parent;
        private TData _data;

        public PrefabFactoryWithBuilder(T prefab, Transform parent, TData data)
        {
            this._prefab = prefab;
            this._parent = parent;
            this._data = data;
        }

        public T Build()
        {
            if(_instance == null)
            {
                Debug.LogError("[PrefabFactoryBuilder]: instance is null! => Please create one!");
                return default;
            }
            if(_instance is IValidatable)
            {
                var validator = _instance.Validate();
                if (!validator.IsValid)
                {
                    throw new InvalidOperationException($"Validation failed:\n{validator}");
                }
            }
            return _instance;
        }

        public T Create(TData data)
        {
            T instance;
            if(_parent != null)
            {
                instance = GameObject.Instantiate(_prefab, _parent);
            }else
            {
                instance = GameObject.Instantiate(_prefab);
            }
            _instance = instance;
            return _instance;
        }


        public PrefabBuilder<T> GetBuilder()
        {
            return new PrefabBuilder<T>(_prefab, _parent);
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using TD.DesignPattern.GenericBuilder.Validatable;
using UnityEngine;

namespace TD.DesignPattern.GenericBuilder.Core
{
    public class PrefabBuilder<T> : IBuilder<T> where T : MonoBehaviour, IValidatable
    {
        private T _instance;
        private Transform _parent;
        private T _prefab;

        public PrefabBuilder(T prefab, Transform parent = null)
        {
            this._prefab = prefab;
            this._parent = parent;
            if (parent != null)
            {
                _instance = GameObject.Instantiate(prefab, parent);
            } else
            {
                _instance = GameObject.Instantiate(prefab);
            }
        }
        public PrefabBuilder<T> With<TValue>(Action<T, TValue> setter, TValue value)
        {
            setter?.Invoke(_instance, value);
            return this;
        }
        public T Build()
        {
            if(_prefab == null)
            {
                Debug.LogError("[PrefabBuilder]: Prefab not found/Builder not Inited => Cannot Build the object!!");
            }
            
            _instance ??= GameObject.Instantiate(_prefab, _parent);
            if (_instance is IValidatable)
            {
                var validator = _instance.Validate();
                if (!validator.IsValid)
                {
                    throw new InvalidOperationException($"Validation failed:\n{validator}");
                }
            }
            return _instance;
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using TD.DesignPattern.GenericBuilder;
using UnityEngine;

namespace TD.DesignPattern.GenericBuilder.Core
{
    public class ScriptableObjectBuilder<T> : IBuilder<T> where T : ScriptableObject
    {
        private T _instance;

        public ScriptableObjectBuilder()
        {
            _instance = ScriptableObject.CreateInstance<T>();
        }

        public ScriptableObjectBuilder<T> With<TValue>(Action<T, TValue> setter, TValue value)
        {
            setter?.Invoke(_instance, value);
            return this;
        }

        public T Build()
        {
            return _instance ??= ScriptableObject.CreateInstance<T>();
        }
    }

}

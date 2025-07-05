using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.DesignPattern.GenericBuilder.Core
{
    public class GenericBuilder<T> : IBuilder<T> where T : new()
    {
        private readonly T _instance;

        public GenericBuilder()
        {
            _instance = new T();
        }

        public GenericBuilder<T> With<TValue>(Action<T, TValue> setter, TValue value)
        {
            setter?.Invoke(_instance, value);
            return this;
        }
       
        public T Build()
        {
            return _instance;
        }
    }

}

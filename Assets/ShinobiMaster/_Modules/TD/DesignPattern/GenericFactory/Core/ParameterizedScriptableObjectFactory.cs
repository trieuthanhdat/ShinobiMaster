using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.DesignPattern.GenericFactory.Core
{
    public class ParameterizedScriptableObjectFactory<T, TData> : IParameterizedFactory<T, TData> where T : ScriptableObject, IInitializable<TData>
    {
        public T Create(TData data)
        {
            T instance = ScriptableObject.CreateInstance<T>();
            instance.Initialize(data);
            return instance;
        }
    }

}

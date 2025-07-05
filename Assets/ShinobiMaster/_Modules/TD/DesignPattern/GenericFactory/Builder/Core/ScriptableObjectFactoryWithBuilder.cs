using TD.DesignPattern.GenericBuilder.Core;
using UnityEngine;

namespace TD.DesignPattern.GenericFactory.Builder
{
    public class ScriptableObjectFactoryWithBuilder<T, TData> :
        IParameterizedFactory<T, TData>, IBuilderFactory<T,
        ScriptableObjectBuilder<T>>
        where T : ScriptableObject, IInitializable<TData>
    {

        public virtual T Create(TData data)
        {
            T instance = ScriptableObject.CreateInstance<T>();
            instance.Initialize(data);
            return instance; 
        }

        public virtual ScriptableObjectBuilder<T> GetBuilder()
        {
            return new ScriptableObjectBuilder<T>();
        }
    }

}

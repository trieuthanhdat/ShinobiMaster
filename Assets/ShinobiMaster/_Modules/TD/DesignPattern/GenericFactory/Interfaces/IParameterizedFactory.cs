using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.DesignPattern.GenericFactory
{
    public interface IParameterizedFactory<T, TData>
    {
        T Create(TData data);
    }

}

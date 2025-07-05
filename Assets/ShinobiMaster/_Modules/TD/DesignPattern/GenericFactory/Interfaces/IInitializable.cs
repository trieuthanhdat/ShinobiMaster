using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TD.DesignPattern.GenericFactory
{
    public interface IInitializable<T>
    {
        T Data { get; set; }
        void Initialize(T data);
    }

}

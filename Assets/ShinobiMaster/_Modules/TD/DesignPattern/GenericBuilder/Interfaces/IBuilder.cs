using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TD.DesignPattern.GenericBuilder
{
    public interface IBuilder<T>
    {
        T Build();
    }

}

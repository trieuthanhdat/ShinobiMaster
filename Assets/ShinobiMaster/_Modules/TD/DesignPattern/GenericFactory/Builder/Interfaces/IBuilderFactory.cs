using System.Collections;
using System.Collections.Generic;
using TD.DesignPattern.GenericBuilder;
using UnityEngine;

namespace TD.DesignPattern.GenericFactory.Builder
{
    public interface IBuilderFactory<T, TBuilder> where TBuilder : IBuilder<T>
    {
        TBuilder GetBuilder();
    }
}


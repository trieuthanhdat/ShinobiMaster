using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.DesignPattern.GenericBuilder.Validatable
{
    public interface IValidatable
    {
        ValidationResult Validate();
    }
}



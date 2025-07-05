using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.DesignPattern.GenericBuilder.Validatable
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public override string ToString()
        {
            return IsValid ? "Validation Passed" : string.Join("\n", Errors);
        }
    }
}

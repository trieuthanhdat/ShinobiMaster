using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.DesignPattern.GenericFactory.Shared
{
    public class SerializedDataWrapper : ScriptableObject
    {
        [HideInInspector] private object data;

        public void SetData(object data)
        {
            this.data = data;

            // Copy fields into SerializedDataWrapper for drawing
            foreach (var field in data.GetType().GetFields())
            {
                GetType().GetField(field.Name)?.SetValue(this, field.GetValue(data));
            }
        }
    }

}


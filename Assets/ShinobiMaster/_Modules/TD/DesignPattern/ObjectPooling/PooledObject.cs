using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TD.DesignPattern.ObjectPooling
{
    /// <summary>
    /// Component That Identifies a gameobject as being Pooled
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        /// <summary>
        /// The prefab this object was instantiated from
        /// </summary>
        public GameObject Prefab { get; set; }
        /// <summary>
        /// Reference to the pool this object belongs to
        /// </summary>
        internal Pool Pool { get; set; }
    }
}


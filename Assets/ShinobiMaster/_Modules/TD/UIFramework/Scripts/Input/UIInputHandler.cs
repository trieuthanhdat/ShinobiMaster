
using System.Collections.Generic;
using System;
using Unity.IO;
using UnityEngine;

namespace TD.UIFramework.UIInput
{
    public class UIInputHandler : MonoBehaviour, IUIInputHandler
    {
        public event Action<KeyCode> onInputRaised;
        public event Action onKeyBackRaised;

        // List of keys to monitor
        [SerializeField] 
        private List<KeyCode> monitoredKeys = new List<KeyCode>
        {
            KeyCode.Escape,  // Typically the back/close key
            KeyCode.O,       // Test Open popup additional key
            KeyCode.Return   // Example additional key
        };

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnKeyBackRaised();
            }

            foreach (KeyCode key in monitoredKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    OnInputRaised(key);
                }
            }
        }

        // Add or remove keys from the monitoring list dynamically
        public void AddKeyToMonitor(KeyCode key)
        {
            if (!monitoredKeys.Contains(key))
                monitoredKeys.Add(key);
        }

        public void RemoveKeyToMonitor(KeyCode key)
        {
            if (monitoredKeys.Contains(key))
                monitoredKeys.Remove(key);
        }

        public void OnInputRaised(KeyCode key)
        {
            onInputRaised?.Invoke(key);
        }

        public void OnKeyBackRaised()
        {
            onKeyBackRaised?.Invoke();
        }
    }
}



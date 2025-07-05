using System.Reflection;
using UnityEngine;

namespace TD.Utilities
{
    public static class ComponentUtils
    {
        /// <summary>
        /// Copies all components from one GameObject to another, including their fields and properties.
        /// </summary>
        /// <param name="source">The GameObject to copy components from.</param>
        /// <param name="destination">The GameObject to copy components to.</param>
        public static void CopyComponents(GameObject source, GameObject destination)
        {
            if (source == null || destination == null)
            {
                Debug.LogWarning("[ComponentUtility] Source or Destination is null. Cannot copy components.");
                return;
            }

            foreach (Component originalComponent in source.GetComponents<Component>())
            {
                if (originalComponent is Transform) continue; // Skip Transform (since it's unique to each GameObject)

                System.Type componentType = originalComponent.GetType();
                Component copiedComponent = destination.AddComponent(componentType);
                CopyComponentValues(originalComponent, copiedComponent);
            }
        }

        /// <summary>
        /// Copies all fields and properties from one component to another.
        /// </summary>
        /// <param name="source">The component to copy values from.</param>
        /// <param name="destination">The component to copy values to.</param>
        private static void CopyComponentValues(Component source, Component destination)
        {
            if (source == null || destination == null) return;

            System.Type type = source.GetType();

            // Copy all fields
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.IsNotSerialized) continue; // Skip non-serializable fields
                field.SetValue(destination, field.GetValue(source));
            }

            // Copy all properties (only those with a setter)
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanWrite || property.GetIndexParameters().Length > 0) continue; // Skip properties without a setter or indexed properties

                try
                {
                    property.SetValue(destination, property.GetValue(source));
                }
                catch
                {
                    Debug.LogWarning($"[ComponentUtility] Failed to copy property {property.Name} on {type.Name}. Skipping...");
                }
            }
        }
    }

}

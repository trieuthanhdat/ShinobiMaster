using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TD.SerializableInterface
{
    [Serializable]
    public class SerializableInterface<T> where T : class
    {
        [SerializeField]
        private UnityEngine.Object obj;

        private T instance;

        public T Instance
        {
            get => instance ??= GetInstance();
            set => SetInstance(value);
        }

        private T GetInstance()
        {
            if (obj is T castInstance)
            {
                instance = castInstance;
            }
            else if (obj is GameObject go && go.TryGetComponent<T>(out var component))
            {
                instance = component;
            }
            else
            {
                instance = null;
            }
            return instance;
        }

        private void SetInstance(T newInstance)
        {
            instance = newInstance;
            obj = newInstance as UnityEngine.Object;
        }
    }

#if UNITY_EDITOR

    namespace TD.SerializableInterface.EditorPropertyDrawers
    {

        [CustomPropertyDrawer(typeof(SerializableInterface<>), true)]
        public class SerializableInterfacePropertyDrawer : PropertyDrawer
        {
            private Type genericType = null;
            private List<Component> compatibleComponents = new List<Component>();

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                InitializeGenericType();

                SerializedProperty objProperty = property.FindPropertyRelative("obj");
                EditorGUI.BeginChangeCheck();

                var newObject = EditorGUI.ObjectField(position, label, objProperty.objectReferenceValue, typeof(UnityEngine.Object), true);

                if (EditorGUI.EndChangeCheck())
                {
                    HandleNewObjectAssignment(newObject, objProperty);
                }
            }

            private void InitializeGenericType()
            {
                if (genericType != null) return;

                var fieldType = fieldInfo.FieldType;

                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                    fieldType = fieldType.GetGenericArguments()[0];
                else if (fieldType.IsArray)
                    fieldType = fieldType.GetElementType();

                var typeArguments = fieldType.GetGenericArguments();
                if (typeArguments != null && typeArguments.Length == 1)
                    genericType = typeArguments[0];
            }

            private void HandleNewObjectAssignment(UnityEngine.Object newObject, SerializedProperty objProperty)
            {
                if (newObject == null)
                {
                    objProperty.objectReferenceValue = null;
                }
                else if (genericType.IsAssignableFrom(newObject.GetType()))
                {
                    objProperty.objectReferenceValue = newObject;
                }
                else if (newObject is GameObject gameObject)
                {
                    ShowComponentSelectionMenu(gameObject, objProperty);
                }
                else
                {
                    Debug.LogWarning($"The assigned object is not compatible with {genericType.Name}");
                }
            }

            private void ShowComponentSelectionMenu(GameObject gameObject, SerializedProperty objProperty)
            {
                compatibleComponents.Clear();
                gameObject.GetComponents(genericType, compatibleComponents);

                if (compatibleComponents.Count == 1)
                {
                    objProperty.objectReferenceValue = compatibleComponents[0];
                }
                else if (compatibleComponents.Count > 1)
                {
                    var menu = new GenericMenu();
                    for (int i = 0; i < compatibleComponents.Count; i++)
                    {
                        var component = compatibleComponents[i];
                        menu.AddItem(new GUIContent($"{i + 1} - {component.GetType().Name}"), false, () =>
                        {
                            objProperty.objectReferenceValue = component;
                            objProperty.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    menu.ShowAsContext();
                }
                else
                {
                    Debug.LogWarning($"No components found on {gameObject.name} that implement {genericType.Name}");
                }
            }
        }
    }
#endif

}

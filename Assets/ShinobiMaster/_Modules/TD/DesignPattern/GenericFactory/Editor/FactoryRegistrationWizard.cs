using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TD.DesignPattern.GenericFactory.Core;
using TD.DesignPattern.GenericFactory.Builder;
using TD.DesignPattern.GenericFactory;
using TD.DesignPattern.GenericBuilder.Validatable;
using System.Linq;
using TD.DesignPattern.GenericFactory.Shared;
using System.Reflection;

public class FactoryRegistrationWizard : EditorWindow
{
    [MenuItem("Tools/Factory Registration Wizard")]
    public static void ShowWindow()
    {
        GetWindow<FactoryRegistrationWizard>("Factory Registration");
    }

    #region ___FIELDS AND PROPERTIES___
    private Dictionary<Type, UnityEngine.Object> registeredObjects = new Dictionary<Type, UnityEngine.Object>();
    private Vector2 scrollPosition;

    #endregion

    #region ___UNITY METHODS___
    void OnGUI()
    {
        GUILayout.Label("Dynamic Factory Registration", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        try
        {
            // Registered Objects UI
            var keys = new List<Type>(registeredObjects.Keys);
            foreach (var type in keys)
            {
                EditorGUILayout.LabelField($"Registered Type: {type.Name}", EditorStyles.boldLabel);
                registeredObjects[type] = EditorGUILayout.ObjectField("Prefab/ScriptableObject", registeredObjects[type], type, false);

                if (GUILayout.Button($"Register {type.Name} Factory"))
                {
                    RegisterFactory(type, registeredObjects[type]);
                }

                EditorGUILayout.Space();
            }

            // Registered Factories Display
            var registeredFactories = FactorySystem.GetAllRegisteredFactories();
            GUILayout.Label("Registered Factories", EditorStyles.boldLabel);

            foreach (var kvp in registeredFactories)
            {
                var factoryType = kvp.Key;
                object factory = kvp.Value;
                Debug.Log($"Factory Key: {factoryType.Name}, Factory Value Type: {factory?.GetType().Name}");

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Factory Type: {factoryType.Name}", EditorStyles.boldLabel);
                Debug.Log("xxx is factory's Value FactoryTestingObject " + (factory is FactoryTestingObject));
                // Check if the factory is a PrefabFactoryWithBuilder
                if (factory is PrefabFactoryWithBuilder<FactoryTestingObject, TestingData> prefabFactory)
                {
                    // Use the factory to create or retrieve the actual FactoryTestingObject instance
                    var factoryObject = prefabFactory.Build(); // Or use .Create() if applicable

                    if (factoryObject != null)
                    {
                        // Now access the Data property
                        var data = factoryObject.Data;
                        Debug.Log($"Successfully retrieved Data from FactoryTestingObject: {data?.name}, {data?.id}");
                    }
                    else
                    {
                        Debug.LogError("FactoryTestingObject instance is null.");
                    }
                }
                else
                {
                    Debug.LogError($"Factory is NOT a PrefabFactoryWithBuilder for FactoryTestingObject. Actual type: {factory?.GetType().Name}");
                }
                var dataProperty = factoryType.GetProperty("Data", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                object factoryData = null;

                if (dataProperty != null)
                {
                    try
                    {
                        // Safely retrieve 'Data' value without assuming its type
                        factoryData = (TestingData)dataProperty.GetValue(factory, null);

                        // Log the runtime type of 'Data' for debugging
                        if (factoryData != null)
                        {
                            Debug.Log($"Successfully retrieved Data of type: {factoryData.GetType().Name}");
                        }
                    }
                    catch (TargetInvocationException ex)
                    {
                        Debug.LogError($"Failed to get value of 'Data' property: {ex.InnerException?.Message ?? ex.Message}");
                    }
                    catch (InvalidCastException ex)
                    {
                        Debug.LogError($"Invalid cast while retrieving 'Data': {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Unexpected error while retrieving 'Data': {ex.Message}");
                    }
                }

                // Safely display factoryData
                if (factoryData != null)
                {
                    GUILayout.Label("Factory Data:", EditorStyles.miniBoldLabel);

                    var dataType = factoryData.GetType();

                    if (typeof(ScriptableObject).IsAssignableFrom(dataType))
                    {
                        // Handle ScriptableObject
                        EditorGUILayout.ObjectField("ScriptableObject Data", (ScriptableObject)factoryData, typeof(ScriptableObject), false);
                    }
                    else if (dataType.IsSerializable || dataType.IsClass || dataType.IsValueType)
                    {
                        GUILayout.Label($"Data Type: {dataType.Name}", EditorStyles.miniLabel);
                        DrawSerializedFields(factoryData);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Unsupported Data Type", EditorStyles.boldLabel);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Failed to Retrieve Factory Data.", EditorStyles.boldLabel);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }
        finally
        {
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Add New Type", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Prefab Type"))
        {
            AddNewTypeEntry<GameObject>();
        }

        if (GUILayout.Button("Add ScriptableObject Type"))
        {
            AddNewTypeEntry<ScriptableObject>();
        }
    }

    
    #endregion

    #region ___MAIN METHODS___
    private void AddNewTypeEntry<T>() where T : UnityEngine.Object
    {
        if (!registeredObjects.ContainsKey(typeof(T)))
        {
            registeredObjects.Add(typeof(T), null);
        }
        else
        {
            Debug.LogWarning($"Type {typeof(T).Name} is already added.");
        }
    }

    private void RegisterFactory(Type type, UnityEngine.Object obj)
    {
        if (obj == null)
        {
            Debug.LogError($"Cannot register factory for type {type.Name}: Object is null.");
            return;
        }

        if (type == typeof(GameObject))
        {
            var prefab = obj as GameObject;
            if (prefab == null)
            {
                Debug.LogError($"The selected object is not a valid prefab.");
                return;
            }

            var monoBehaviour = prefab.GetComponent<MonoBehaviour>();
            if (monoBehaviour == null)
            {
                Debug.LogError($"Prefab does not contain a valid MonoBehaviour component.");
                return;
            }

            var componentType = monoBehaviour.GetType();

            // Dynamically detect the generic argument of IInitializable<>
            var iInitializableInterface = componentType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IInitializable<>));

            if (iInitializableInterface == null || !typeof(IValidatable).IsAssignableFrom(componentType))
            {
                Debug.LogError($"MonoBehaviour component must implement IInitializable<T> and IValidatable.");
                return;
            }

            var dataType = iInitializableInterface.GetGenericArguments()[0]; // Extract T from IInitializable<T>

            try
            {
                // Create the factory dynamically for the detected types
                var factoryType = typeof(PrefabFactoryWithBuilder<,>).MakeGenericType(componentType, dataType);
                var constructor = factoryType.GetConstructor(new[] { componentType, typeof(Transform), dataType });

                if (constructor == null)
                {
                    Debug.LogError($"Constructor not found for {factoryType.Name}. Ensure the parameters match.");
                    return;
                }

                object factoryInstance = constructor.Invoke(new object[] { monoBehaviour, null, null });

                // Register the factory dynamically
                var registerFactoryMethod = typeof(FactorySystem).GetMethod("RegisterFactory").MakeGenericMethod(componentType, dataType);
                registerFactoryMethod.Invoke(null, new[] { factoryInstance });

                Debug.Log($"Prefab Factory for {componentType.Name} with DataType {dataType.Name} Registered.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create factory for {componentType.Name}: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Unsupported type for registration: {type.Name}.");
        }
    }
    #endregion

    #region ___HELPER METHODS___
    private void DrawSerializedFields(object target)
    {
        var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var value = field.GetValue(target);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(field.Name, GUILayout.Width(150));

            if (value is int)
                field.SetValue(target, EditorGUILayout.IntField((int)value));
            else if (value is float)
                field.SetValue(target, EditorGUILayout.FloatField((float)value));
            else if (value is string)
                field.SetValue(target, EditorGUILayout.TextField((string)value));
            else if (value is bool)
                field.SetValue(target, EditorGUILayout.Toggle((bool)value));
            else if (value != null && value.GetType().IsClass)
                EditorGUILayout.LabelField("Complex Data", EditorStyles.miniLabel);
            else
                EditorGUILayout.LabelField("Unsupported Type");

            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Draw fields for Serializable data
    /// </summary>
    public void DrawSerializableData(object data)
    {
        SerializedObject serializedData = new SerializedObject(CreateScriptableObjectWrapper(data));
        SerializedProperty property = serializedData.GetIterator();
        property.NextVisible(true);

        while (property.NextVisible(false))
        {
            EditorGUILayout.PropertyField(property, true);
        }
        serializedData.ApplyModifiedProperties();
    }

    /// <summary>
    /// Wrap data in a ScriptableObject to draw it in the Inspector.
    /// </summary>
    public ScriptableObject CreateScriptableObjectWrapper(object data)
    {
        var wrapper = ScriptableObject.CreateInstance<SerializedDataWrapper>();
        wrapper.SetData(data);
        return wrapper;
    }
    #endregion
}

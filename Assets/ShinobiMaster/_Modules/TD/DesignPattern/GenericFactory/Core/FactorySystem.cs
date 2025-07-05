using System;
using System.Collections;
using System.Collections.Generic;
using TD.DesignPattern.GenericBuilder;
using TD.DesignPattern.GenericFactory.Builder;
using System.Collections.Concurrent;
using UnityEngine;
using System.Linq;

namespace TD.DesignPattern.GenericFactory.Core
{
    public static class FactorySystem
    {
        #region ___FIELDS AND PROPERTIES___
        private static readonly ConcurrentDictionary<Type, object> _factories = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, (object Factory, string Metadata)> _factoriesWithMetadata = new();

        #endregion

        #region ___MAIN METHODS___

        public static void RegisterFactoryWithMetadata<T, TData>(IParameterizedFactory<T, TData> factory, string metadata)
        {
            _factoriesWithMetadata.AddOrUpdate(typeof(T), (factory, metadata), (key, existing) => (factory, metadata));
        }

        public static IEnumerable<(Type Type, string Metadata)> GetAllFactoriesWithMetadata()
        {
            return _factoriesWithMetadata.Select(f => (f.Key, f.Value.Metadata));
        }

        public static bool TryGetFactory<T, TData>(out IParameterizedFactory<T, TData> factory)
        {
            if (_factories.TryGetValue(typeof(T), out var result))
            {
                factory = result as IParameterizedFactory<T, TData>;
                return factory != null;
            }
            factory = null;
            return false;
        }

        public static IParameterizedFactory<T, TData> RegisterFactory<T, TData>(IParameterizedFactory<T, TData> factory)
        {
            _factories.AddOrUpdate(typeof(T), factory, (key, existingFactory) => factory);
            return factory;
        }

        public static IParameterizedFactory<T, TData> GetFactory<T, TData>()
        {
            if (_factories.TryGetValue(typeof(T), out var factory))
            {
                return factory as IParameterizedFactory<T, TData>;
            }

            LogError($"Factory for {typeof(T)} is not registered. Ensure you have called RegisterFactory<T, TData> before attempting to retrieve it.");
            throw new InvalidOperationException($"Factory for {typeof(T)} is not registered.");
        }

        public static IBuilderFactory<T, TBuilder> GetBuilderFactory<T, TBuilder>() where TBuilder : IBuilder<T>
        {
            if (_factories.TryGetValue(typeof(T), out var factory))
            {
                return factory as IBuilderFactory<T, TBuilder>;
            }

            LogError($"Builder Factory for {typeof(T)} is not registered. Ensure you have registered a compatible factory.");
            throw new InvalidOperationException($"Builder Factory for {typeof(T)} is not registered.");
        }

        public static void RegisterFactories(IEnumerable<KeyValuePair<Type, object>> factories)
        {
            foreach (var pair in factories)
            {
                _factories.AddOrUpdate(pair.Key, pair.Value, (key, existingFactory) => pair.Value);
            }
        }

        public static void UnregisterFactory<T>(Action cleanupCallback = null)
        {
            var key = typeof(T);
            if (_factories.TryRemove(key, out _))
            {
                cleanupCallback?.Invoke();
                Log($"Factory for {typeof(T)} has been deregistered.");
            }
            else
            {
                LogWarning($"Factory for {typeof(T)} is not registered.");
            }
        }

        #endregion

        #region ___HELPER METHODS___
        // Automatically clear factories at app start
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeSystem()
        {
            ClearFactories();
        }

        private static void ClearFactories()
        {
            _factories.Clear();
        }

        public static IEnumerable<Type> GetRegisteredFactories()
        {
            return _factories.Keys;
        }

        public static ConcurrentDictionary<Type, object> GetAllRegisteredFactories()
        {
            return _factories;
        }
        #region LOG METHODS
        private static void Log(string message, string context = "FactorySystem")
        {
            Debug.Log($"[{context}] {message}");
        }

        private static void LogError(string message, string context = "FactorySystem")
        {
            Debug.LogError($"[{context}] {message}");
        }

        private static void LogWarning(string message, string context = "FactorySystem")
        {
            Debug.LogWarning($"[{context}] {message}");
        }

        #endregion

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Factory System/Show Registered Factories")]
        public static void ShowRegisteredFactories()
        {
            foreach (var factory in _factories)
            {
                Debug.Log($"Registered Factory: {factory.Key.Name}, Instance: {factory.Value?.GetType().Name}");
            }
        }
#endif

        #endregion
    }

}

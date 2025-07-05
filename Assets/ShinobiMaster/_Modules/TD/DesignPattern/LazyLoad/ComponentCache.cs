using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TD.Utilities
{
    public static class ComponentCache
    {
        #region ___FIELDS___

        // Use ConditionalWeakTable to avoid memory leaks and allow GC of GameObjects
        private static ConditionalWeakTable<GameObject, Dictionary<Type, Component>> cache = new();
        private static ConditionalWeakTable<GameObject, Dictionary<Type, Component[]>> cacheMultiple = new();

        private static ConditionalWeakTable<GameObject, Dictionary<Type, Component>> cacheInChildren = new();
        private static ConditionalWeakTable<GameObject, Dictionary<Type, Component[]>> cacheInChildrenArray = new();

        private static ConditionalWeakTable<GameObject, Dictionary<Type, Component>> cacheInParent = new();
        private static ConditionalWeakTable<GameObject, Dictionary<Type, Component[]>> cacheInParentArray = new();

        private static ConditionalWeakTable<GameObject, Dictionary<Type, object>> interfaceCache = new();
        private static ConditionalWeakTable<GameObject, Dictionary<Type, object[]>> interfaceCacheMultiple = new();

        private static ConditionalWeakTable<GameObject, Dictionary<Type, object>> interfaceCacheInParent = new();

        private static ConditionalWeakTable<GameObject, Dictionary<Type, object>> interfaceCacheInChildren = new();
        private static ConditionalWeakTable<GameObject, Dictionary<Type, object[]>> interfaceCacheInChildrenArray = new();

  
        
        #endregion

        #region ___METHODS FOR COMPONENTS___

        public static bool TryGetComponentInParent<T>(GameObject gameObject, out T target) where T : Component
        {
            target = GetComponentInParent<T>(gameObject);
            return target != null;
        }

        public static bool TryGetComponent<T>(GameObject gameObject, out T target) where T : Component
        {
            target = GetComponent<T>(gameObject);
            return target != null;
        }

        public static T GetComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null) return null;
            if (cache.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var comp) && comp != null)
                return (T)comp;

            T component = gameObject.GetComponent<T>();
            if (component != null)
                AddToCache(gameObject, component);
            return component;
        }

        public static T[] GetComponents<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null) return Array.Empty<T>();
            if (cacheMultiple.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var comps) && comps != null && comps.Length > 0)
                return (T[])comps;

            T[] components = gameObject.GetComponents<T>();
            if (components.Length > 0)
                AddToCacheMultiple(gameObject, components);
            return components;
        }

        private static void AddToCacheMultiple(GameObject gameObject, Component[] components)
        {
            if (!cacheMultiple.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, Component[]>();
                cacheMultiple.Add(gameObject, typeDict);
            }
            typeDict[components[0].GetType()] = components;
        }

        public static T GetComponentInChildren<T>(GameObject gameObject, bool includeInactive = false) where T : Component
        {
            if (gameObject == null) return null;
            if (cacheInChildren.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var comp) && comp != null)
                return (T)comp;

            T component = gameObject.GetComponentInChildren<T>(includeInactive);
            if (component != null)
                AddToCacheInChildren(gameObject, component);
            return component;
        }

        public static T[] GetComponentsInChildren<T>(GameObject gameObject, bool includeInactive = false) where T : Component
        {
            if (gameObject == null) return Array.Empty<T>();
            if (cacheInChildrenArray.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var comps) && comps != null)
                return comps as T[];

            T[] components = gameObject.GetComponentsInChildren<T>(includeInactive);
            if (components.Length > 0)
                AddToCacheInChildrenArray(gameObject, components);
            return components;
        }

        public static T GetComponentInParent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null) return null;
            if (cacheInParent.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var comp) && comp != null)
                return (T)comp;

            T component = gameObject.GetComponentInParent<T>();
            if (component != null)
                AddToCacheInParent(gameObject, component);
            return component;
        }

        public static T[] GetComponentsInParent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null) return Array.Empty<T>();
            if (cacheInParentArray.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var comps) && comps != null)
                return comps as T[];

            T[] components = gameObject.GetComponentsInParent<T>();
            if (components.Length > 0)
                AddToCacheInParentArray(gameObject, components);
            return components;
        }

        public static T GetComponentInChildren<T>(GameObject gameObject, int depth) where T : Component
        {
            if (gameObject == null) return null;
            if (cacheInChildren.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var comp) && comp != null)
                return (T)comp;

            T component = FindComponentInNthDepthChild<T>(gameObject.transform, depth);
            if (component != null)
                AddToCacheInChildren(gameObject, component);
            return component;
        }

        public static T[] GetComponentsInChildren<T>(GameObject gameObject, int depth, bool includeInactive = false) where T : Component
        {
            if (gameObject == null) return Array.Empty<T>();
            if (cacheInChildrenArray.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var comps) && comps != null)
                return comps as T[];

            List<T> components = FindComponentsInNthDepthChildren<T>(gameObject.transform, depth, includeInactive);
            if (components.Count > 0)
                AddToCacheInChildrenArray(gameObject, components.ToArray());
            return components.ToArray();
        }
        public static bool TryGetComponentInChildren<T>(GameObject gameObject, out T target, bool includeInactive = false) where T : Component
        {
            target = GetComponentInChildren<T>(gameObject, includeInactive);
            return target != null;
        }
        #endregion

        #region ___INTERFACE METHODS___

        public static T GetInterface<T>(GameObject gameObject) where T : class
        {
            if (gameObject == null) return null;
            if (interfaceCache.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var iface) && iface != null)
                return iface as T;

            T component = gameObject.GetComponent(typeof(T)) as T;
            if (component != null)
                AddToInterfaceCache(gameObject, component);
            return component;
        }

        public static T[] GetInterfaces<T>(GameObject gameObject) where T : class
        {
            if (gameObject == null) return Array.Empty<T>();
            if (interfaceCacheMultiple.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var ifaces) && ifaces != null && ifaces.Length > 0)
                return ifaces as T[];

            var arr = gameObject.GetComponents(typeof(T));
            T[] components = arr is T[] tArr ? tArr : Array.ConvertAll(arr, x => x as T);
            if (components.Length > 0)
                AddToInterfaceCacheMultiple(gameObject, components);
            return components;
        }

        public static T GetInterfaceInParent<T>(GameObject gameObject) where T : class
        {
            if (gameObject == null) return null;
            if (interfaceCacheInParent.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var iface) && iface != null)
                return (T)iface;

            T component = gameObject.GetComponentInParent<T>();
            if (component != null)
                AddToInterfaceCacheInParent(gameObject, component);
            return component;
        }

        public static T GetInterfaceInChildren<T>(GameObject gameObject, int depth) where T : class
        {
            if (gameObject == null) return null;
            if (interfaceCacheInChildren.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var iface) && iface != null)
                return iface as T;

            T foundInterface = FindInterfaceInNthDepthChild<T>(gameObject.transform, depth);
            if (foundInterface != null)
                AddToInterfaceCacheInChildren(gameObject, foundInterface);
            return foundInterface;
        }

        public static T[] GetInterfacesInChildren<T>(GameObject gameObject, int depth, bool includeInactive = false) where T : class
        {
            if (gameObject == null) return Array.Empty<T>();
            if (interfaceCacheInChildrenArray.TryGetValue(gameObject, out var typeDict) && typeDict.TryGetValue(typeof(T), out var ifaces) && ifaces != null)
                return ifaces as T[];

            List<T> foundInterfaces = FindInterfacesInNthDepthChildren<T>(gameObject.transform, depth, includeInactive);
            if (foundInterfaces.Count > 0)
                AddToInterfaceCacheInChildrenArray(gameObject, foundInterfaces.ToArray());
            return foundInterfaces.ToArray();
        }

        public static bool TryGetInterface<T>(GameObject gameObject, out T target) where T : class
        {
            target = GetInterface<T>(gameObject);
            return target != null;
        }

        public static bool TryGetInterfaces<T>(GameObject gameObject, out T[] target) where T : class
        {
            target = GetInterfaces<T>(gameObject);
            return target != null;
        }

        public static bool TryGetInterfaceInParent<T>(GameObject gameObject, out T target) where T : class
        {
            target = GetInterfaceInParent<T>(gameObject);
            return target != null;
        }
        public static bool TryGetInterfaceInChildren<T>(GameObject gameObject, out T target, int depth, bool includeInactive = false) where T : class
        {
            target = GetInterfaceInChildren<T>(gameObject, depth);
            if (target != null)
                return true;

            // If not found in cache, search manually (respecting includeInactive)
            if (gameObject != null)
            {
                var found = FindInterfacesInNthDepthChildren<T>(gameObject.transform, depth, includeInactive);
                if (found.Count > 0)
                {
                    target = found[0];
                    AddToInterfaceCacheInChildren(gameObject, target);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region ___CACHE HELPER METHODS___

        private static T FindComponentInNthDepthChild<T>(Transform parent, int depth) where T : Component
        {
            if (depth < 0 || parent == null) return null;
            if (depth == 0) return parent.GetComponent<T>();
            foreach (Transform child in parent)
            {
                T component = FindComponentInNthDepthChild<T>(child, depth - 1);
                if (component != null) return component;
            }
            return null;
        }

        private static List<T> FindComponentsInNthDepthChildren<T>(Transform parent, int depth, bool includeInactive) where T : Component
        {
            List<T> components = new();
            if (depth < 0 || parent == null) return components;
            if (depth == 0)
            {
                foreach (var c in parent.GetComponents<T>())
                    if (includeInactive || c.gameObject.activeSelf)
                        components.Add(c);
            }
            else
            {
                foreach (Transform child in parent)
                    components.AddRange(FindComponentsInNthDepthChildren<T>(child, depth - 1, includeInactive));
            }
            return components;
        }

        private static T FindInterfaceInNthDepthChild<T>(Transform parent, int depth) where T : class
        {
            if (depth < 0 || parent == null) return null;
            if (depth == 0)
            {
                foreach (var component in parent.GetComponents<Component>())
                    if (component is T targetInterface)
                        return targetInterface;
            }
            else
            {
                foreach (Transform child in parent)
                {
                    T foundInterface = FindInterfaceInNthDepthChild<T>(child, depth - 1);
                    if (foundInterface != null) return foundInterface;
                }
            }
            return null;
        }

        private static List<T> FindInterfacesInNthDepthChildren<T>(Transform parent, int depth, bool includeInactive) where T : class
        {
            List<T> foundInterfaces = new();
            if (depth < 0 || parent == null) return foundInterfaces;
            if (depth == 0)
            {
                foreach (var component in parent.GetComponents<Component>())
                    if (component is T targetInterface && (includeInactive || component.gameObject.activeSelf))
                        foundInterfaces.Add(targetInterface);
            }
            else
            {
                foreach (Transform child in parent)
                    foundInterfaces.AddRange(FindInterfacesInNthDepthChildren<T>(child, depth - 1, includeInactive));
            }
            return foundInterfaces;
        }

        private static void AddToCache<T>(GameObject gameObject, T component) where T : Component
        {
            if (!cache.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, Component>();
                cache.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = component;
        }

        private static void AddToCacheInChildren<T>(GameObject gameObject, T component) where T : Component
        {
            if (!cacheInChildren.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, Component>();
                cacheInChildren.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = component;
        }

        private static void AddToCacheInChildrenArray<T>(GameObject gameObject, T[] components) where T : Component
        {
            if (!cacheInChildrenArray.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, Component[]>();
                cacheInChildrenArray.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = components;
        }

        private static void AddToCacheInParent<T>(GameObject gameObject, T component) where T : Component
        {
            if (!cacheInParent.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, Component>();
                cacheInParent.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = component;
        }

        private static void AddToCacheInParentArray<T>(GameObject gameObject, T[] components) where T : Component
        {
            if (!cacheInParentArray.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, Component[]>();
                cacheInParentArray.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = components;
        }

        private static void AddToInterfaceCache<T>(GameObject gameObject, T component) where T : class
        {
            if (!interfaceCache.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, object>();
                interfaceCache.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = component;
        }

        private static void AddToInterfaceCacheMultiple<T>(GameObject gameObject, T[] components) where T : class
        {
            if (!interfaceCacheMultiple.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, object[]>();
                interfaceCacheMultiple.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = components;
        }

        private static void AddToInterfaceCacheInParent<T>(GameObject gameObject, T component) where T : class
        {
            if (!interfaceCacheInParent.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, object>();
                interfaceCacheInParent.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = component;
        }

        private static void AddToInterfaceCacheInChildren<T>(GameObject gameObject, T foundInterface) where T : class
        {
            if (!interfaceCacheInChildren.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, object>();
                interfaceCacheInChildren.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = foundInterface;
        }

        private static void AddToInterfaceCacheInChildrenArray<T>(GameObject gameObject, T[] foundInterfaces) where T : class
        {
            if (!interfaceCacheInChildrenArray.TryGetValue(gameObject, out var typeDict))
            {
                typeDict = new Dictionary<Type, object[]>();
                interfaceCacheInChildrenArray.Add(gameObject, typeDict);
            }
            typeDict[typeof(T)] = foundInterfaces;
        }

        #endregion

        #region ___DISPOSE METHODS___

        public static void ClearCache()
        {
#if UNITY_EDITOR
            // Debug log to check if caches have elements
            bool hasElements =
                cache.Count() > 0 ||
                cacheMultiple.Count() > 0 ||
                cacheInChildren.Count() > 0 ||
                cacheInChildrenArray.Count() > 0 ||
                cacheInParent.Count() > 0 ||
                cacheInParentArray.Count() > 0 ||
                interfaceCache.Count() > 0 ||
                interfaceCacheMultiple.Count() > 0 ||
                interfaceCacheInParent.Count() > 0 ||
                interfaceCacheInChildren.Count() > 0 ||
                interfaceCacheInChildrenArray.Count() > 0;

            Debug.Log($"[ComponentCache] ClearCache called. Any cache not empty: {hasElements}");
#endif
            // Recreate the ConditionalWeakTables to force clear all cached entries
            UnsafeClear(ref cache);
            UnsafeClear(ref cacheMultiple);
            UnsafeClear(ref cacheInChildren);
            UnsafeClear(ref cacheInChildrenArray);
            UnsafeClear(ref cacheInParent);
            UnsafeClear(ref cacheInParentArray);
            UnsafeClear(ref interfaceCache);
            UnsafeClear(ref interfaceCacheMultiple);
            UnsafeClear(ref interfaceCacheInParent);
            UnsafeClear(ref interfaceCacheInChildren);
            UnsafeClear(ref interfaceCacheInChildrenArray);
        }

        private static void UnsafeClear<T, U>(ref ConditionalWeakTable<T, U> table)
            where T : class
            where U : class
        {
            table = new ConditionalWeakTable<T, U>();
        }
     
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSceneLoad()
        {
            // No explicit clear needed; ConditionalWeakTable will release entries when GameObjects are GC'd.
            ClearCache();
        }

        // Add this method if not already present
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ClearCache();
        }

        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            return TryGetComponent(go, out T comp) ? comp : go.AddComponent<T>();
        }
        // Add this static constructor to ensure cache is cleared after scene load
        static ComponentCache()
        {
            // Register to scene loaded event to clear cache on scene reload
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        #endregion
    }
}

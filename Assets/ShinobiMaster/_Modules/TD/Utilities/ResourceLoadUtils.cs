using System;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Utilities
{
    /// <summary>
    /// Utility class for efficient and dynamic loading/unloading of Unity resources.
    /// </summary>
    public static class ResourceLoadUtils
    {

        /// <summary>
        /// Event triggered when a resource is loaded.
        /// </summary>
        public static event Action<UnityEngine.Object> ResourceLoaded;
        /// <summary>
        /// Event triggered when a resource is unloaded.
        /// </summary>
        public static event Action<UnityEngine.Object> ResourceUnloaded;
        // Cache for loaded resources to avoid redundant disk access
        private static readonly Dictionary<string, UnityEngine.Object> _resourceCache = new Dictionary<string, UnityEngine.Object>(StringComparer.Ordinal);

        /// <summary>
        /// Loads a resource by name, searching all subfolders, with optional caching.
        /// </summary>
        /// <typeparam name="T">Type of resource.</typeparam>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="useCache">Whether to use the cache for faster repeated access.</param>
        /// <returns>The loaded resource, or null if not found.</returns>
        public static T LoadResourceByName<T>(string resourceName, bool useCache = true) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(resourceName))
                return null;

            string cacheKey = typeof(T).FullName + ":" + resourceName;
            if (useCache && _resourceCache.TryGetValue(cacheKey, out var cachedObj))
                return cachedObj as T;

            // Try direct path first (fastest)
            T direct = Resources.Load<T>(resourceName);
            if (direct != null)
            {
                if (useCache)
                    _resourceCache[cacheKey] = direct;
                return direct;
            }

            // Fallback: search all assets (slow, but necessary if not found by direct path)
            T[] allAssets = Resources.LoadAll<T>("");
            for (int i = 0; i < allAssets.Length; i++)
            {
                var asset = allAssets[i];
                if (asset != null && asset.name == resourceName)
                {
                    if (useCache)  _resourceCache[cacheKey] = asset;

                    Debug.Log($"Resource '{resourceName}' loaded from Resources folder but with a slow performance!. Please Fix/Update the Resource Path !!");
                    return asset;
                }
            }
            return null;
        }

        /// <summary>
        /// Loads all resources of type T in the Resources folder and its subfolders, with optional caching.
        /// </summary>
        /// <typeparam name="T">Type of resource.</typeparam>
        /// <param name="useCache">Whether to cache the loaded resources.</param>
        /// <returns>Array of loaded resources.</returns>
        public static T[] LoadAllResources<T>(bool useCache = false) where T : UnityEngine.Object
        {
            T[] allAssets = Resources.LoadAll<T>("");
            if (useCache)
            {
                string typePrefix = typeof(T).FullName + ":";
                foreach (var asset in allAssets)
                {
                    if (asset != null)
                        _resourceCache[typePrefix + asset.name] = asset;
                }
            }
            return allAssets;
        }

        /// <summary>
        /// Loads a resource by its path relative to the Resources folder, with optional caching.
        /// </summary>
        /// <typeparam name="T">Type of resource.</typeparam>
        /// <param name="resourcePath">Path relative to Resources.</param>
        /// <param name="useCache">Whether to use the cache for faster repeated access.</param>
        /// <returns>The loaded resource, or null if not found.</returns>
        public static T LoadResourceByPath<T>(string resourcePath, bool useCache = true) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(resourcePath))
                return null;

            string cacheKey = typeof(T).FullName + "|" + resourcePath;
            if (useCache && _resourceCache.TryGetValue(cacheKey, out var cachedObj))
                return cachedObj as T;

            T asset = Resources.Load<T>(resourcePath);
            if (asset != null && useCache)
                _resourceCache[cacheKey] = asset;
            return asset;
        }

        /// <summary>
        /// Loads all resources of type T in a specific subfolder of the Resources folder, with optional caching.
        /// </summary>
        /// <typeparam name="T">Type of resource.</typeparam>
        /// <param name="folderPath">Subfolder path.</param>
        /// <param name="useCache">Whether to cache the loaded resources.</param>
        /// <returns>Array of loaded resources.</returns>
        public static T[] LoadAllResourcesInFolder<T>(string folderPath, bool useCache = false) where T : UnityEngine.Object
        {
            T[] allAssets = Resources.LoadAll<T>(folderPath);
            if (useCache)
            {
                string typePrefix = typeof(T).FullName + ":";
                foreach (var asset in allAssets)
                {
                    if (asset != null)
                        _resourceCache[typePrefix + asset.name] = asset;
                }
            }
            return allAssets;
        }

        /// <summary>
        /// Loads all resources of type T whose names match the given predicate, with optional caching.
        /// </summary>
        /// <typeparam name="T">Type of resource.</typeparam>
        /// <param name="namePredicate">Predicate to match resource names.</param>
        /// <param name="useCache">Whether to cache the loaded resources.</param>
        /// <returns>List of matching resources.</returns>
        public static List<T> LoadResourcesByPredicate<T>(Predicate<string> namePredicate, bool useCache = false) where T : UnityEngine.Object
        {
            T[] allAssets = Resources.LoadAll<T>("");
            List<T> result = new List<T>();
            string typePrefix = typeof(T).FullName + ":";
            for (int i = 0; i < allAssets.Length; i++)
            {
                var asset = allAssets[i];
                if (asset != null && namePredicate(asset.name))
                {
                    result.Add(asset);
                    if (useCache)
                        _resourceCache[typePrefix + asset.name] = asset;
                }
            }
            return result;
        }

        /// <summary>
        /// Unloads a resource from memory and removes it from the cache.
        /// </summary>
        /// <param name="resource">Resource to unload.</param>
        public static void UnloadResource(UnityEngine.Object resource)
        {
            if (resource != null)
            {
                // Remove from cache
                foreach (var key in new List<string>(_resourceCache.Keys))
                {
                    if (_resourceCache[key] == resource)
                        _resourceCache.Remove(key);
                }
                Resources.UnloadAsset(resource);
            }
        }

        /// <summary>
        /// Unloads all unused assets from memory and clears the cache.
        /// </summary>
        public static void UnloadUnusedResources()
        {
            _resourceCache.Clear();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Clears the internal resource cache without unloading assets.
        /// </summary>
        public static void ClearCache()
        {
            _resourceCache.Clear();
        }
    }

}


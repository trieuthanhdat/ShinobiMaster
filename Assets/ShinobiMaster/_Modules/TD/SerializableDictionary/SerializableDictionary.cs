using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace TD.SerializableDictionary
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [SerializeField]
        private List<SerializableKeyValuePair> _keyValuePairs = new List<SerializableKeyValuePair>();

        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        public Dictionary<TKey, TValue> ContentTable { get { return _dictionary; } set { _dictionary = value; } }

        public Dictionary<TKey, TValue>.ValueCollection GetValues()
        {
            return _dictionary.Values;
        }

        public int GetCount()
        {
            return _dictionary.Count;
        }

        public TKey GetKeyAtIndex(int index)
        {
            if (_keyValuePairs == null) return default;
            var keyPair = _keyValuePairs[index];
            return keyPair != null ? keyPair.Key : default;
        }

        public TValue GetValueAtIndex(int index)
        {
            if (_keyValuePairs == null) return default;
            var keyPair = _keyValuePairs[index];
            return keyPair != null ? keyPair.Value : default;
        }

        public void OnBeforeSerialize()
        {
            _keyValuePairs.Clear();
            foreach (var pair in _dictionary)
            {
                _keyValuePairs.Add(new SerializableKeyValuePair(pair.Key, pair.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<TKey, TValue>();
            RemapListKeyPair(ref _keyValuePairs);
            // Create a copy of the keyValuePairs list to avoid modification during iteration
            List<SerializableKeyValuePair> tmpKeypair = new List<SerializableKeyValuePair>(_keyValuePairs);
            foreach (var pair in tmpKeypair)
            {
                var nextKey = pair.Key;
                Add(nextKey, pair.Value);
            }
        }

        private void RemapListKeyPair(ref List<SerializableKeyValuePair> listKeyPair)
        {
            List<SerializableKeyValuePair> listToMap = new List<SerializableKeyValuePair>();

            foreach (var pair in listKeyPair)
            {
                if (pair == null || pair.Value == null || pair.Key == null)
                    continue;
                if (ContentTable.ContainsKey(pair.Key))
                {
                    listToMap.Add(pair);
                }
            }
            if (listToMap.Count == 0) return;
            listKeyPair.Clear();
            listKeyPair.AddRange(listToMap);
            listKeyPair.Distinct();
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null || value == null) return;
            // Check if the key already exists before adding
            if (!_dictionary.ContainsKey(key))
            {
                _dictionary.Add(key, value);
                _keyValuePairs.Add(new SerializableKeyValuePair(key, value));
            }
            else
            {
                // Add the entry with the modified key after a delay
                Task.Delay(TimeSpan.FromSeconds(0.3)).ContinueWith(_ =>
                {
                    TKey modifiedKey = ModifyKeyToMakeUnique(key);
                    _dictionary.Add(modifiedKey, value);
                    _keyValuePairs.Add(new SerializableKeyValuePair(modifiedKey, value));
                });
            }
        }

        private Dictionary<TKey, int> _keyCollisionCount = new Dictionary<TKey, int>();

        private TKey ModifyKeyToMakeUnique(TKey key)
        {
            int collisionCount;
            TKey nextKey;

            // Remove entries from keyCollisionCount that are not in _dictionary
            List<TKey> keysToRemove = _keyCollisionCount.Keys.Where(k => !_dictionary.ContainsKey(k)).ToList();
            foreach (var keyToRemove in keysToRemove)
            {
                _keyCollisionCount.Remove(keyToRemove);
            }

            do
            {
                collisionCount = _keyCollisionCount.TryGetValue(key, out int count) ? count + 1 : 1;
                _keyCollisionCount[key] = collisionCount;

                if (typeof(TKey) == typeof(string))
                {
                    string stringKey = key.ToString();
                    nextKey = (TKey)(object)(stringKey + "_" + collisionCount);
                }
                else if (typeof(TKey) == typeof(int))
                {
                    int intKey = (int)(object)key;
                    nextKey = (TKey)(object)(intKey + collisionCount);
                }
                else if (typeof(TKey) == typeof(float))
                {
                    float floatKey = (float)(object)key;
                    nextKey = (TKey)(object)(floatKey + 0.1f * collisionCount);
                }
                else if (typeof(TKey) == typeof(long))
                {
                    long longKey = (long)(object)key;
                    nextKey = (TKey)(object)(longKey + collisionCount);
                }
                else if (typeof(TKey) == typeof(char))
                {
                    char charKey = (char)(object)key;
                    nextKey = (TKey)(object)(charKey == char.MaxValue ? char.MinValue : (char)(charKey + collisionCount));
                }
                else if (typeof(TKey) == typeof(double))
                {
                    double doubleKey = (double)(object)key;
                    nextKey = (TKey)(object)(doubleKey + 0.1 * collisionCount);
                }
                else if (typeof(TKey) == typeof(bool))
                {
                    // Toggle the boolean value
                    nextKey = (TKey)(object)(!(bool)(object)key);
                }
                else if (typeof(TKey).IsEnum)
                {
                    var underlyingType = Enum.GetUnderlyingType(typeof(TKey));
                    if (underlyingType == typeof(int))
                    {
                        int intKey = (int)(object)key;
                        intKey += collisionCount;
                        nextKey = (TKey)Enum.ToObject(typeof(TKey), intKey);
                    }
                    else if (underlyingType == typeof(byte))
                    {
                        byte byteKey = (byte)(object)key;
                        byteKey += (byte)collisionCount;
                        nextKey = (TKey)Enum.ToObject(typeof(TKey), byteKey);
                    }
                    else if (underlyingType == typeof(long))
                    {
                        long longKey = (long)(object)key;
                        longKey += collisionCount;
                        nextKey = (TKey)Enum.ToObject(typeof(TKey), longKey);
                    }
                    else
                    {
                        Debug.LogError($"Unsupported underlying enum type: {underlyingType}");
                        return key;
                    }
                }
                else
                {
                    Debug.LogError($"Unsupported key type: {typeof(TKey)}");
                    return key;
                }
            } while (_dictionary.ContainsKey(nextKey));

            return nextKey;
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _dictionary.ContainsValue(value);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out _))
            {
                _dictionary.Remove(key);
                _keyValuePairs.RemoveAll(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key));
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _dictionary.Clear();
            _keyValuePairs.Clear();
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return _dictionary.TryAdd(key, value);
        }
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        // Add indexer for direct access by key
        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        // Copy constructor method
        public static SerializableDictionary<TKey, TValue> CopyFromDictionary(Dictionary<TKey, TValue> source)
        {
            var newDictionary = new SerializableDictionary<TKey, TValue>();
            foreach (var pair in source)
            {
                newDictionary.Add(pair.Key, pair.Value);
            }
            return newDictionary;
        }

        [Serializable]
        public class SerializableKeyValuePair
        {
            [SerializeField]
            private TKey key;
            [SerializeField]
            private TValue value;

            public SerializableKeyValuePair(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }

            public TKey Key => key;
            public TValue Value => value;

            public override string ToString()
            {
                return base.ToString() + " => key: " + key + " value: " + value;
            }
        }

        // Ensure the dictionary is initialized before use
        public SerializableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
            _keyValuePairs = new List<SerializableKeyValuePair>();
        }

        // Implement IEnumerable interface for foreach support
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

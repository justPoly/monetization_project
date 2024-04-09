using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papae.UnitySDK.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<K, V> ConvertToDictionary<K, V>(string dictionaryString, char valueSeparator = '=', char sequenceSeparator = '|')
        {
            Dictionary<K, V> dict = new Dictionary<K, V>();

            UnityEngine.Debug.Log(dictionaryString);

            dictionaryString = dictionaryString.Replace('{', ' ');
            dictionaryString = dictionaryString.Replace('}', ' ');

            if (!string.IsNullOrEmpty(dictionaryString.Trim()))
            {
                string[] cells = dictionaryString.Split(sequenceSeparator);

                foreach (string cell in cells)
                {
                    UnityEngine.Debug.Log(cell);

                    if (!string.IsNullOrEmpty(cell.Trim()))
                    {
                        string[] pairs = cell.Split(valueSeparator);
                        //K key = pairs [0] as K;
                        K key = (K)Convert.ChangeType(pairs[0], typeof(K));
                        //V value = pairs [1] as V;
                        V value = (V)Convert.ChangeType(pairs[1], typeof(V));

                        dict.Add(key, value);
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Adds a key/value pair to the dictionary and returns its value
        /// </summary>
        public static TValue AddAndReturnValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary.Add(key, value);
            return value;
        }

        public static void AddIfNotExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            NullCheck(dictionary);
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            NullCheck(dictionary);
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }

            UnityEngine.Debug.Log("Kay: " + key);
        }

        public static bool AreKeysEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            NullCheck(dictionary);
            return dictionary.All(x => x.Key == null);
        }

        public static bool AreValuesEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            NullCheck(dictionary);
            return dictionary.All(x => x.Value == null);
        }

        public static void DeleteKeyIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            NullCheck(dictionary);
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
        }

        public static void DeleteValueIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value)
        {
            NullCheck(dictionary);
            if (dictionary.ContainsValue(value))
            {
                var key = dictionary.GetKeyFromValue(value);
                dictionary.Remove(key);
            }
        }

        public static Dictionary<T1, T2> Merge<T1, T2>(this Dictionary<T1, T2> first, Dictionary<T1, T2> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            var merged = new Dictionary<T1, T2>();
            first.ToList().ForEach(kv => merged[kv.Key] = kv.Value);
            second.ToList().ForEach(kv => merged[kv.Key] = kv.Value);

            return merged;
        }

        public static Dictionary<K, V> MergeLeft<K, V>(this Dictionary<K, V> left, params IDictionary<K, V>[] others)
        {
            if (left == null) throw new ArgumentNullException("first");
            if (others == null) throw new ArgumentNullException("others");

            var newMap = new Dictionary<K, V>(left, left.Comparer);
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { left }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }

        public static void Update<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> pair)
        {
            NullCheck(dictionary);
            //CheckKeyValuePairIsNull(pair);
            if (dictionary.ContainsKey(pair.Key))
            {
                dictionary[pair.Key] = pair.Value;
            }
        }

        public static void Update<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            NullCheck(dictionary);
            //CheckKeyValuePairIsNull(key, value);
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
        }

        public static TKey GetKeyFromValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value)
        {
            var keys = new List<TKey>();
            foreach (var pair in dictionary)
            {
                AddToKeysList(keys, pair, value);
            }

            GreaterThanZeroCheck(keys.Count, value);
            return !keys.Any() ? default(TKey) : keys.First();
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> constructor)
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }

            value = constructor();
            dict[key] = value;
            return value;
        }

        public static void AddToKeysList<TKey, TValue>(List<TKey> keys, KeyValuePair<TKey, TValue> pair, TValue value)
        {
            if (pair.Value.Equals(value))
            {
                keys.Add(pair.Key);
            }
        }

        public static string ToStringFull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, string value_separator = " = ", char sequence_separator = '|')
        {
            return dictionary == null ? "" : dictionary.Aggregate("", (str, v) =>
                str + v.Key.ToString()
                + value_separator
                + v.Value.ToString()
                + sequence_separator)
                    .TrimEnd(sequence_separator);
        }

        private static void GreaterThanZeroCheck<TValue>(int count, TValue value)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException("count");
            if (count > 1) throw new ArgumentException("value");
        }

        private static void NullCheck<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
        }

        /*
        private static void CheckKeyValuePairIsNull<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
        {
            if (pair.Key == null || pair.Value == null) throw new ArgumentNullException(nameof(pair));
        }

        private static void CheckKeyValuePairIsNull<TKey, TValue>(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
        }
        */
    }
}

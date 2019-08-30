using System.Collections.Generic;

namespace CoCo.Utils
{
    public static class CollectionExtensions
    {
        /// <summary>
        ///  Removes the value associated with the <paramref name="key"/>
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false. This method also
        /// returns false if <paramref name="key"/> was not found in <paramref name="dictionary"/>
        /// </returns>
        public static bool TryRemove<K, V>(this IDictionary<K, V> dictionary, K key, out V value)
        {
            if (dictionary.TryGetValue(key, out value) && dictionary.Remove(key))
            {
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Try to add the <paramref name="value"/> and the <paramref name="key"/>
        /// </summary>
        /// <returns>
        /// true if the element is successfully added; otherwise, false. This method also
        /// returns false if <paramref name="key"/> was added before to <paramref name="dictionary"/>
        /// </returns>
        public static bool TryAdd<K, V>(this IDictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }
            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// Deconstructs input <paramref name="pair"/> to (<paramref name="key"/>, <paramref name="value"/>)
        /// </summary>
        public static void Deconstruct<K, V>(this KeyValuePair<K, V> pair, out K key, out V value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
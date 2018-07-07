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
        public static bool TryRemoveValue<K, V>(this IDictionary<K, V> dictionary, K key, out V value)
        {
            if (dictionary.TryGetValue(key, out value) && dictionary.Remove(key))
            {
                return true;
            }
            // TODO: use C# 7.1
            value = default(V);
            return false;
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
namespace GuaLanguage.Utility 
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public static class DictionaryExtension
    {
        /// <summary>
        /// 尝试往一个Map中丢入一个key和val，返回值表示Add时是否出错，也就是说如果返回true，表示没有问题，否则表示有问题（即之前已经存在这个Key）；但是不论返回值如何，都会正确的覆盖Map中key所对应的值。
        /// </summary>
        /// <param name="Dictionary<K"></param>
        /// <param name="map"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns>true represents process is OK</returns>
        public static bool TryAdd<K, V>(this Dictionary<K, V> map, K key, V val) 
        {
            if(map.ContainsKey(key)) 
            {
                map[key] = val;
                return false;
            }
            else 
            {
                map.Add(key, val);
                return true;
            }
        }

        /// <summary>
        /// <seealso cref="TryAdd" />的Set版本，但是返回值表示了相反的意思，即false表示之前不存在这个值。
        /// </summary>
        /// <param name="Dictionary<K"></param>
        /// <param name="map"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public static bool TrySet<K, V>(this Dictionary<K, V> map, K key, V val)
        {
            return !map.TryAdd(key, val);
        } 

        /// <summary>
        /// 检查这个表中key对应的val和参数中的val相同（单纯的==比较）。
        /// </summary>
        /// <param name="Dictionary<K"></param>
        /// <param name="map"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public static bool CheckIsSameVal<K, V>(this Dictionary<K, V> map, K key, V val)
        {
            if(map.ContainsKey(key) == false) 
            {
                return false;
            }

            // just call object.Equals() cuz need (val == map[key])
            return val.Equals(map[key]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValue<TKey, TValue> (this Dictionary<TKey, TValue> dictionary, TKey item, TValue Default = default(TValue))
        {
            if (dictionary == null || !dictionary.TryGetValue(item, out TValue value)) return Default;
            return value;
                
        }
    }
}

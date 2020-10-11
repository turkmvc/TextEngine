using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using TextEngine.Misc;

namespace TextEngine
{
    public class SDEneme
    {
        public int Deneme { get; set; }
        public string this[int index]
        {
            get
            {
                return "Ne var";
            }
        }

    }
    public static class PhpFuctions
    {
#pragma warning disable IDE1006 // Adlandırma Stilleri
        public static bool empty(object item)
        {
            if(item is string s)
            {
                return string.IsNullOrEmpty(s);
            }
            else if(item is StringBuilder sb)
            {
                return sb.Length == 0;
            }
            else if(item is Array arr)
            {
                return arr.Length == 0;
            }
            return item == null;
        }
        public static bool in_array(string key, string[] array)
        {
            return array.Any(e => e == key);
        }

        public static bool char_equalsany(char key, params char[] chars)

        {
            return chars.Any(e => e == key);
        }
        public static bool is_string(object obj)
        {
            return obj is string;
        }

        public static bool is_array(object obj)

        {
            if (obj == null) return false;
            return obj is IList;
        }

        public static bool is_object(object obj)
        {
            return obj is ExpandoObject;
        }

        public static bool str_equalsany(string key, params string[] keys)
        {
            return keys.Any(e => e == key);
        }
        public static bool is_indis(object obj)
        {
            if (obj == null) return false;
            var props = obj.GetType().GetProperty("Item")?.GetIndexParameters();
            var prop = obj.GetType().GetProperty("Item");
            return props != null && props.Length > 0;

        }
        public static bool not_empty(object obj)
        {
            if (obj == null) return false;
            if(obj is bool b)
            {
                return b;
            }
            if(TypeUtil.IsNumericType(obj))
            {
                double d = (double) Convert.ChangeType(obj, TypeCode.Double);
                return d > 0;
            }
            return obj != null;
        }
    #pragma warning restore IDE1006 // Adlandırma Stilleri
    }
}

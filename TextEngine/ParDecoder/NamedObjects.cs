using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public class NamedObjects
    {
        private List<object> _values;
        private List<string> _keys;
        private bool keysincluded;
        public object First()
        {
            if (Count == 0) return null;
            return this[0];
        }
        public T First<T>() where T: class
        {
            return First() as T;
        }
        public NamedObjects()
        {
            _values = new List<object>();
            _keys = new List<string>();
        }
        public int Count
        {
            get
            {
                return _values.Count;
            }
        }
        public object this[int index]
        {
            get
            {
                if (index < 0 || index > _values.Count) return null;
                return _values[index];
            }
            set
            {
                if (index < 0 || index > _values.Count) return;
                    _values[index] = value;
            }
        }
        public object this[string key]
        {
            get
            {
                return this[this.GetIndexByName(key)];
            }
            set
            {
                this.AddObject(key, value);
            }
        }
        public int GetIndexByName(string name)
        {
            if (name == "") return -1;
            return _keys.IndexOf(name);
        }
        public void AddObject(object value)
        {
            this.AddObject("", value);
        }
        public void AddObject(string key, object value)
        {
            if(key == "")
            {
                this._keys.Add("");
                this._values.Add(value);
            }
            else
            {
                keysincluded = true;
                int index = this.GetIndexByName(key);
                if(index == -1)
                {
                    this._keys.Add(key);
                    this._values.Add(value);
                }
                else
                {
                    this._values[index] = value;
                }
            }
        }
        public object[] GetObjects()
        {
            return this._values.ToArray();
        }
        public string[] GetKeys()
        {
            return this._keys.ToArray();
        }
        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < this._keys.Count; i++)
            {
                if (this._keys[i] == "") continue;
                dict.Add(this._keys[i], this._values[i]);
            }
            return dict;
        }
        public ExpandoObject ToExpandoObject()
        {
            ExpandoObject eobj = new ExpandoObject();
            IDictionary<string, object> dict = (IDictionary<string, object>)eobj;
            for (int i = 0; i < this._keys.Count; i++)
            {
                if (this._keys[i] == "") continue;
                dict.Add(this._keys[i], this._values[i]);
            }
            return eobj;

        }
        public bool KeysIncluded()
        {
            return keysincluded;
        }
    }
}

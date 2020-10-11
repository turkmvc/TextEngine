using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TextEngine.Text
{
    [DebuggerDisplay("Count = {Count}")]
    public class TextElementArray : IList
    {
        List<object> values = new List<object>();
        List<string> keys = new List<string>();
        public object this[string key]
        {
            get
            {
                var index = keys.IndexOf(key);
                if (index == -1) return null;
                return values[index];
            }
            set
            {
                this[key] = value;
            }
        }

        public object this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count
        {
            get
            {
                return values.Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }
        public void Add(string name, object value)
        {
            this.keys.Add(name);
            this.values.Add(value);
        }

        public void Clear()
        {
            this.keys.Clear();
            this.values.Clear();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }
        public bool Contains(string name)
        {
            return this.keys.Contains(name);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {

            if (!(value is string str)) return;
            var index = this.keys.IndexOf(str);
            RemoveAt(index);
        }
        public object[] ToArray()
        {
            return values.ToArray();
        }

        public void RemoveAt(int index)
        {
            keys.RemoveAt(index);
            values.RemoveAt(index);
        }
    }
}

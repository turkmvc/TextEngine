using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{
    public class KeyValues<TObject> : IEnumerable<KeyValue<TObject>> 
    {
        private List<KeyValue<TObject>> inner;
        public bool AutoInitialize { get; set; } = true;
        public int Count
        {
            get
            {
                return inner.Count;
            }
        }
        public KeyValues()
        {
            inner = new List<KeyValue<TObject>>();
        }
        public TObject this[int index]
        {
            get
            {
                return (index < 0 || index > Count) ? default(TObject) : this.inner[index].Value; 
            }
        }
        public TObject this[string name]
        {
            get
            {
                return this[GetIdByName(name)];
            }
            set
            {
                int id = GetIdByName(name);
                KeyValue<TObject> kv = null;
                if(id == -1)
                {
                    kv = new KeyValue<TObject>()
                    {
                        Name = name
                    };
                    this.inner.Add(kv);
                }
                else
                {
                    kv = inner[id];
                }
                
                kv.Value = value;
            }
        }
        public void Set(string name, TObject value)
        {
            int id = this.GetIdByName(name);
            if (id == -1)
            {
                this.Add(name, value);
            }
            else
            {
                this.inner[id].Value = value;
            }
        }
        public bool Delete(string name)
        {
            int id = this.GetIdByName(name);
            if (id == -1) return false;
            this.inner.RemoveAt(id);
            return true;
        }
        public void Add(string name, TObject value)
        {

            this.inner.Add(new KeyValue<TObject>() { Name = name, Value = value });
        }
        public int GetIdByName(string name)
        {
            for (int i = 0; i < this.inner.Count; i++)
            {
                if(this.inner[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }
        public IEnumerator<KeyValue<TObject>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }
        public override string ToString()
        {
            return $"Total Keys: {Count}";
        }
    }
}

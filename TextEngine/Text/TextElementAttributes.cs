using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextEngine.Text
{
    public class TextElementAttributes : IList<TextElementAttribute>, ICloneable
    {
        private List<TextElementAttribute> inner;
        public TextElementAttributes()
        {
            inner = new List<TextElementAttribute>();
        }
        public TextElementAttribute this[string name]
        {
            get
            {
                return inner.Where(e => e.Name == name).FirstOrDefault();
            }
        }
        public TextElementAttribute this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {

            }
        }
        public bool HasAttribute(string attrib, bool cs = false)
        {
            string lower = attrib;
            if(cs)
            {
                lower = lower.ToLower();
            }
            for (int i = 0; i < this.Count; i++)
            {
                var attr = this[i];
                if(cs)
                {
                    if (attr.Name.ToLower() == lower) return true;

                }
                else
                {
                    if (attr.Name == lower) return true;

                }
            }
            return false;
        }
        public TextElementAttribute FirstAttribute
        {
            get
            {
                if (this.Count == 0) return null;
                return this[0];
            }
        }
        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(TextElementAttribute item)
        {
            this.inner.Add(item);
        }

        public void Clear()
        {
            this.inner.Clear();
        }

        public bool Contains(TextElementAttribute item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TextElementAttribute[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TextElementAttribute> GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        public int IndexOf(TextElementAttribute item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, TextElementAttribute item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TextElementAttribute item)
        {
           return  this.inner.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.inner.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }
        public string GetAttribute(string name, string @default = null)
        {
            var item = this[name];
            if (item != null) return item.Value;
            return @default;
        }
        public void SetAttribute(string name, string value)
        {
            var item = this[name];
            if (item == null)
            {
                item = new TextElementAttribute()
                {
                    Name = name
                };
                this.Add(item);
            }
            item.Value = value;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public TextElementAttributes CloneWCS()
        {
            return (TextElementAttributes) Clone();
        }
    }
}

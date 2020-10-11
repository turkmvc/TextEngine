using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextEngine.XPathClasses;

namespace TextEngine.Text
{
    public class TextElements : IList<TextElement>
    {
        private List<TextElement> inner;
        public TextElements()
        {
            inner = new List<TextElement>();
        }
        public void SortItems()
        {
            inner.Sort(CompareTextElements);
        }
        private static int CompareTextElements(TextElement a, TextElement b)
        {
            if(a.Depth == b.Depth)
            {
                if(a.Index > b.Index)
                {
                    return 1;
                }
                else if(b.Index > a.Index)
                {
                    return -1;
                }
                return 0;
            }

            if(a.Depth > b.Depth)
            {
                int depthfark = Math.Abs(a.Depth - b.Depth);
                TextElement next = a;
                for (int i = 0; i < depthfark; i++)
                {
                    next = next.Parent;
                }
                return CompareTextElements(next, b);
            }
            else
            {
                int depthfark = Math.Abs(a.Depth - b.Depth);
                TextElement next = b;
                for (int i = 0; i < depthfark; i++)
                {
                    next = next.Parent;
                }
                return CompareTextElements(a, next);
            }
        }
        public TextElement this[string name]
        {
            get
            {
                return inner.Where(e => e.ElemName == name).FirstOrDefault();
            }
        }
        public TextElement this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {

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
        public void AddRange(IEnumerable<TextElement> items)
        {
            this.inner.AddRange(items);
        }
        public void Add(TextElement item)
        {
            item.Index = this.inner.Count;
            this.inner.Add(item);
        }

        public void Clear()
        {
            this.inner.Clear();
        }

        public bool Contains(TextElement item)
        {
            return this.inner.Contains(item);
        }

        public void CopyTo(TextElement[] array, int arrayIndex)
        {
            this.inner.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TextElement> GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        public int IndexOf(TextElement item)
        {
            return inner.IndexOf(item);
        }

        public void Insert(int index, TextElement item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TextElement item)
        {
            return this.inner.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.inner.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }
        public TextElements FindByXPath(XPathBlock xblock)
        {
            TextElements elements = new TextElements();
            for (int j = 0; j < this.Count; j++)
            {
                var elem = this[j];
                var nextelems = elem.FindByXPath(xblock);
                for (int k = 0; k < nextelems.Count; k++)
                {
                    if (elements.Contains(nextelems[k])) continue;
                    elements.Add(nextelems[k]);
                }
            }
            return elements;
        }
    }
}

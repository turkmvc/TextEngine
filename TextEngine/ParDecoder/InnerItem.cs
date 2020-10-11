using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public enum InnerType
    {
         TYPE_STRING = 0,
         TYPE_NUMERIC = 1,
         TYPE_BOOLEAN = 2,
         TYPE_VARIABLE = 3
    }
    public class InnerItem
    {
        public InnerItem()
        {
        }
        public object Value { get; set; }
	    public char Quote { get; set; }
        public bool IsOperator { get; set; }
        public InnerType InnerType { get; set; }
        public InnerItemsList InnerItems { get; set; }
        public InnerItem Parent { get; set; }
        public virtual bool IsGroup()
        {
            return false;
        }
        public virtual bool IsObject()
        {
            return false;
        }
        public virtual bool IsParItem()
        {
            return false;
        }
        public virtual bool IsArray()
        {
            return this.InnerItems != null;
        }

    }
}

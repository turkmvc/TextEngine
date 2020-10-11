using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Extensions;

namespace TextEngine.XPathClasses
{
    public class XPathExpressionItem : IXPathExpressionItem
    {
        private object value;
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.IsNumeric = false;
                this.IsBool = false;
                if (!IsOperator && QuotChar == '\0' && value is string s)
                {
                    if (s.IsNumeric())
                    {
                        value = double.Parse(s);
                        this.IsNumeric = true;
                    }
                    else if (s.IsBool())
                    {
                        value = s.ToBool();
                        this.IsBool = true;
                    }
                }
                this.value = value;
            }
        }
        public char QuotChar { get; set; }
        public bool IsNumeric { get; private set; }
        public bool IsBool { get; private set; }
        public bool IsOperator { get; set; }
        public bool IsVariable
        {
            get
            {
                return !IsOperator && !IsNumeric && !IsBool &&  QuotChar == '\0';
            }
        }

        public bool IsSubItem
        {
            get
            {
                return false;
            }
        }
        public char ParChar { get { return '\0'; } set { } }

    }
}

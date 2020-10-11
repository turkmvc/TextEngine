using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public class XPathExpressionSubItem : IXPathExpressionItems
    {
        public XPathExpressionSubItem()
        {
            XPathExpressions = new XPathExpressions();
            ParChar = '(';
        }
        public XPathExpressions XPathExpressions { get;  set; }

        public bool IsSubItem
        {
            get
            {
                return true;
            }
        }

        public char ParChar { get; set; }
    }
}

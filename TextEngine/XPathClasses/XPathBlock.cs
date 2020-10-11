using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public class XPathBlock : IXPathExpressionItems
    {
        public XPathBlock()
        {
            XPathExpressions = new XPathExpressions();
        }
        public bool IsAttributeSelector { get; set; }
        public XPathBlockType BlockType { get; set; }
        private string blockName;
        public string BlockName
        {
            get
            {
                return blockName;
            }
            set
            {
                blockName = value;
            }
        }
        private XPathExpressions xexpressions;
        public XPathExpressions XPathExpressions
        {
            get
            {
                return xexpressions;
            }
            set
            {
                xexpressions = value;
            }
        }

        public bool IsSubItem
        {
            get
            {
                return true;
            }
        }
        public char ParChar { get { return '\0'; } set { } }

        public bool IsXPathPar()
        {
            return false;
        }
        private IXPathBlockContainer parent;
        public IXPathBlockContainer Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }
    }
}

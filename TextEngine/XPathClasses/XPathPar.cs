using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public class XPathPar : IXPathBlockContainer, IXPathList, IXPathExpressionItems
    {
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

        private XPathBlockContainer xpathBlockList;
        public XPathBlockContainer XPathBlockList
        {
            get
            {
                return xpathBlockList;
            }
            set
            {
                xpathBlockList = value;
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
                return false;
            }
        }

        public char ParChar
        {
            get

            {
                return '\0';
            }
            set
            {
                
            }
        }

        public bool IsXPathPar()
        {
            return true;
        }

        public bool IsBlocks()
        {
            return false;
        }

        public XPathPar()
        {
            this.XPathExpressions = new XPathExpressions();
            this.XPathBlockList = new XPathBlockContainer();
        }
        public bool Any()
        {
            return this.XPathBlockList.Count > 0 || this.XPathExpressions.Count > 0;
        }

        public bool IsOr()
        {
            return false;
        }
    }
}

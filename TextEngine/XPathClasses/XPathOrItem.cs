using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public class XPathOrItem : IXPathBlockContainer, IXPathList
    {
        public XPathBlockContainer XPathBlockList
        {
            get
            {
                return null;
            }
            set
            {

            }
        }
        public IXPathBlockContainer Parent
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public bool Any()
        {
            return true;
        }

        public bool IsBlocks()
        {
            return false;
        }

        public bool IsOr()
        {
            return true;
        }

        public bool IsXPathPar()
        {
            return false;
        }
    }
}

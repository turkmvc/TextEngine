using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public class XPathBlocks : List<XPathBlock>, IXPathList
    {
        public bool IsBlocks()
        {
            return true;
        }
        public bool Any()
        {
            return this.Count > 0;
        }

        public bool IsOr()
        {
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public class XPathBlockContainer : List<IXPathList>
    {
        public IXPathList Last()
        {
            if (this.Count == 0) return null;
            return this[this.Count - 1];
        }
    }
}

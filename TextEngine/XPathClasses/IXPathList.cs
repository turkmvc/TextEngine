using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public interface IXPathList
    {
        bool IsBlocks();
        bool Any();
        bool IsOr();
    }
}

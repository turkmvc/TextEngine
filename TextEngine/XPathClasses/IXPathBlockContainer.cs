using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public interface IXPathBlockContainer
    {
        bool IsXPathPar();
        XPathBlockContainer XPathBlockList { get; set; }
        IXPathBlockContainer Parent { get; set; }

    }
}

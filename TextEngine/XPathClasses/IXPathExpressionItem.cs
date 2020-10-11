using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public interface IXPathExpressionItem 
    {
        bool IsSubItem { get; }
        char ParChar { get; set; }
    }
}

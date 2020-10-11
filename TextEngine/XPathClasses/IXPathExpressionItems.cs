using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public interface IXPathExpressionItems : IXPathExpressionItem
    {
        XPathExpressions XPathExpressions { get; set; }
    }
}

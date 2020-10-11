using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public enum TextElementType
    {
        ElementNode = 1,
        AttributeNode = 2,
        TextNode = 3,
        CDATASection = 4,
        EntityReferenceNode = 5,
        CommentNode = 8,
        Document = 9,
        Parameter = 16,
        XMLTag = 17
    }
}

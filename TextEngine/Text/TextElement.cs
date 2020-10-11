using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Evulator;
using TextEngine.Misc;
using TextEngine.XPathClasses;

namespace TextEngine.Text
{
    public class TextElement
    {
        public TextElement()
        {
            this.ElementType = TextElementType.ElementNode;
            this.SubElements = new TextElements();
            this.ElemAttr = new TextElementAttributes();
        }

        public int Depth
        {
            get
            {
                var parent = this.Parent;
                int total = 0;
                while (parent != null && parent.ElemName != "#document")
                {
                    total++;
                    parent = parent.Parent;
                }
                return total;
            }
        }

        private string elemName;
        public string ElemName
        {
            get { return elemName; }
            set { elemName = value; }
        }
        private TextElementAttributes elemAttr;
        public TextElementAttributes ElemAttr
        {
            get { return elemAttr; }
            set { elemAttr = value; }
        }
        public TextElementType ElementType { get; set; }

        public TextEvulator BaseEvulator { get; set; }
        private bool closed;
        public bool Closed
        {
            get { return closed; }
            set
            {
                closed = value;
                if (this.BaseEvulator != null)
                {
                    this.BaseEvulator.OnTagClosed(this);
                }
            }
        }
        private string value;

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        private TextElements subElements;
        public TextElements SubElements
        {
            get { return subElements; }
            set { subElements = value; }
        }
        public TextElement FirstChild
        {
            get
            {
                if (this.SubElementsCount > 0)
                    return this.SubElements[0];
                return null;
            }
        }
        public TextElement LastChild
        {
            get
            {
                if (this.SubElementsCount > 0)
                    return this.SubElements[this.SubElementsCount - 1];
                return null;
            }
        }
        public int SubElementsCount { get { return SubElements.Count; } }
        private bool slashused;
        public bool SlashUsed
        {
            get { return slashused; }
            set { this.slashused = value; }
        }
        private TextElement parent;
        public TextElement Parent
        {
            get { return parent; }
            set { this.parent = value; }
        }
        private bool directclosed;
        public bool DirectClosed
        {
            get { return directclosed; }
            set { this.directclosed = value; }
        }

        private bool autoadded;
        public bool AutoAdded
        {
            get { return autoadded; }
            set { this.autoadded = value; }
        }
        private string aliasName;
        public string AliasName
        {
            get { return aliasName; }
            set { this.aliasName = value; }
        }
        private bool autoclosed;
        public bool AutoClosed
        {
            get { return autoclosed; }
            set { this.autoclosed = value; }
        }
        //private int index;
        public int Index
        {
            get
            {
                if (this.Parent == null) return -1;
                return this.Parent.SubElements.IndexOf(this);
            }
            set
            {

            }
        }
        private string tag_attrib;
        public string TagAttrib
        {
            get { return tag_attrib; }
            set { this.tag_attrib = value; }
        }
        public void AddElement(TextElement element)
        {
            element.Index = this.SubElements.Count;
            this.SubElements.Add(element);
        }
        public string GetAttribute(string name, string @default = null)
        {
            return this.ElemAttr.GetAttribute(name, @default);
        }
        public void SetAttribute(string name, string value)
        {
            this.ElemAttr.SetAttribute(name, value);
        }
        public bool NameEquals(string name, bool matchalias = false)
        {
            if (this.ElemName.ToUpperInvariant() == name.ToUpperInvariant()) return true;
            if (matchalias)
            {
                if (this.BaseEvulator.Aliasses.ContainsKey(name))
                {
                    var alias = this.BaseEvulator.Aliasses[name];
                    if (alias?.ToString().ToUpperInvariant() == this.ElemName.ToUpperInvariant()) return true;
                    return true;
                }
            }
            return false;
        }
        public TextElement SetInner(string text)
        {
            this.SubElements.Clear();
            this.BaseEvulator.Parse(this, text);
            return this;
        }
        public string Outer(bool outputformat = false)
        {

            if (this.ElemName == "#document")
            {
                return this.Inner();
            }
            if (this.ElemName == "#text")
            {
                return this.value;
            }
            if (this.ElementType == TextElementType.CommentNode)
            {
                return this.BaseEvulator.LeftTag.ToString() + "--" + this.Value + "--" + this.BaseEvulator.RightTag.ToString();
            }
            StringBuilder text = new StringBuilder();
            StringBuilder additional = new StringBuilder();
            if (!PhpFuctions.empty(this.TagAttrib))
            {
                additional.Append("=" + this.TagAttrib);
            }
            if (this.ElementType == TextElementType.Parameter)
            {
                text.Append(this.BaseEvulator.LeftTag.ToString() + this.BaseEvulator.ParamChar.ToString() + this.ElemName + HTMLUTIL.ToAttribute(this.ElemAttr) + this.BaseEvulator.RightTag.ToString());
            }
            else
            {
                if (this.AutoAdded)
                {
                    if (this.SubElementsCount == 0) return string.Empty;
                }
                text.Append(this.BaseEvulator.LeftTag.ToString() + this.ElemName + additional.ToString() + HTMLUTIL.ToAttribute(this.ElemAttr));
                if (this.DirectClosed)
                {
                    text.Append(" /" + this.BaseEvulator.RightTag);
                }
                else if (this.AutoClosed)
                {
                    text.Append(this.BaseEvulator.RightTag);
                }
                else
                {
                    text.Append(this.BaseEvulator.RightTag);
                    text.Append(this.Inner(outputformat));
                    var eName = this.ElemName;
                    if (!PhpFuctions.empty(this.AliasName))
                    {
                        eName = this.AliasName;
                    }
                    text.Append(this.BaseEvulator.LeftTag + '/' + eName + this.BaseEvulator.RightTag.ToString());
                }
            }
            return text.ToString();
        }
        public string Header(bool outputformat = false)
        {
            if (this.AutoAdded && this.SubElementsCount == 0) return null;

            StringBuilder text = new StringBuilder();
            if (outputformat)
            {
                text.Append('\t', this.Depth);
            }
            if (this.ElementType == TextElementType.XMLTag)
            {
                text.Append(this.BaseEvulator.LeftTag.ToString() + "?" + this.ElemName + HTMLUTIL.ToAttribute(this.ElemAttr) + "?" + this.BaseEvulator.RightTag.ToString());
            }
            if (this.ElementType == TextElementType.Parameter)
            {
                text.Append(this.BaseEvulator.LeftTag.ToString() + this.BaseEvulator.ParamChar.ToString() + this.ElemName + HTMLUTIL.ToAttribute(this.ElemAttr) + this.BaseEvulator.RightTag.ToString());
            }
            else if (this.ElementType == TextElementType.ElementNode)
            {
                StringBuilder additional = new StringBuilder();
                if (!PhpFuctions.empty(this.TagAttrib))
                {
                    additional.Append('=' + this.TagAttrib);
                }
                text.Append(this.BaseEvulator.LeftTag.ToString() + this.ElemName + additional.ToString() + HTMLUTIL.ToAttribute(this.ElemAttr));
                if (this.DirectClosed)
                {
                    text.Append(" /" + this.BaseEvulator.RightTag.ToString());
                }
                else if (this.AutoClosed)
                {
                    text.Append(this.BaseEvulator.RightTag);
                }
                else
                {
                    text.Append(this.BaseEvulator.RightTag);

                }
            }
            else if (this.ElementType == TextElementType.CDATASection)
            {
                text.Append(this.BaseEvulator.LeftTag.ToString() + "![CDATA[" + this.Value + "]]" + this.BaseEvulator.RightTag.ToString());
            }
            else if (this.ElementType == TextElementType.CommentNode)
            {
                text.Append(this.BaseEvulator.LeftTag.ToString() + "--" + this.Value + "--" + this.BaseEvulator.RightTag.ToString());
            }
            if (outputformat && this.FirstChild?.ElemName != "#text")
            {
                text.Append("\r\n");
            }
            return text.ToString();
        }
        public string Footer(bool outputformat = false)
        {
            if (this.SlashUsed || this.DirectClosed || this.AutoClosed) return null;
            var text = new StringBuilder();
            if (this.ElementType == TextElementType.ElementNode)
            {
                if (outputformat)
                {
                    if (this.LastChild?.ElemName != "#text")
                    {
                        text.Append('\t', this.Depth);

                    }
                }
                var eName = this.ElemName;
                if (!PhpFuctions.empty(this.AliasName))
                {
                    eName = this.AliasName;
                }
                text.Append(this.BaseEvulator.LeftTag.ToString() + '/' + eName + this.BaseEvulator.RightTag.ToString());

            }
            if (outputformat)
            {
                text.Append("\r\n");
            }
            return text.ToString();
        }
        public string Inner(bool outputformat = false)
        {
            StringBuilder text = new StringBuilder();

            if (this.ElementType == TextElementType.CommentNode || this.ElementType == TextElementType.XMLTag)
            {
                return text.ToString();
            }
            if (this.ElemName == "#text" || this.ElementType == TextElementType.CDATASection)
            {
                if (this.ElementType == TextElementType.EntityReferenceNode)
                {
                    text.Append("&" + this.Value + ";");
                    return text.ToString();
                }
                text.Append(this.Value);
                return text.ToString();
            }
            if (this.SubElementsCount == 0) return text.ToString();
            foreach (var subElement in this.SubElements)
            {

                if (subElement.ElemName == "#text")
                {
                    text.Append(subElement.Inner(outputformat));
                }
                else if (subElement.ElementType == TextElementType.CDATASection)
                {
                    text.Append(subElement.Header());
                }
                else if (subElement.ElementType == TextElementType.CommentNode)
                {
                    text.Append(subElement.Outer(outputformat));
                }
                else if (subElement.ElementType == TextElementType.Parameter)
                {
                    text.Append(subElement.Header());
                }
                else
                {
                    text.Append(subElement.Header(outputformat));
                    text.Append(subElement.Inner(outputformat));
                    text.Append(subElement.Footer(outputformat));
                }

            }
            return text.ToString();
        }
        public TextElement PreviousElementWN(params string[] names)
        {
            var prev = this.PreviousElement();
            while (prev != null)
            {
                if (prev.ElementType == TextElementType.Parameter || prev.ElemName == "#text")
                {
                    prev = prev.PreviousElement();
                    continue;
                }
                if (PhpFuctions.in_array(prev.ElemName, names))
                {
                    return prev;
                }
                prev = prev.PreviousElement();
            }
            return null;
        }
        public TextElement NextElementWN(params string[] names)
        {
            var next = this.NextElement();
            while (next != null)
            {
                if (next.ElementType == TextElementType.Parameter || next.ElemName == "#text")
                {
                    next = next.NextElement();
                    continue;
                }
                if (PhpFuctions.in_array(next.ElemName, names))
                {
                    return next;
                }
                next = next.NextElement();
            }
            return null;
        }
        public TextElement PreviousElement()
        {
            if (this.Index - 1 >= 0)
            {
                return this.Parent.SubElements[this.Index - 1];
            }
            return null;
        }
        public TextElement NextElement()
        {
            if (this.Index + 1 < this.Parent.SubElementsCount)
            {
                return this.Parent.SubElements[this.Index + 1];
            }
            return null;
        }
        public TextElement GetSubElement(params string[] names)
        {

            for (int i = 0; i < this.SubElementsCount; i++)
            {
                if (PhpFuctions.in_array(this.SubElements[i].ElemName, names))
                {
                    return this.SubElements[i];
                }
            }
            return null;
        }
        public string InnerText()
        {
            if (this.ElemName == "#text" || this.ElementType == TextElementType.CDATASection)
            {
                if (this.ElementType == TextElementType.EntityReferenceNode)
                {
                    this.BaseEvulator.AmpMaps.TryGetValue(this.Value, out string nval);
                    return nval;
                }
                return this.Value;
            }
            StringBuilder text = new StringBuilder();
            if (this.SubElementsCount == 0) return text.ToString();
            foreach (var subElement in this.SubElements)
            {
                if (subElement.ElemName == "#text" || subElement.ElementType == TextElementType.CDATASection)
                {
                    if (subElement.ElementType == TextElementType.EntityReferenceNode)
                    {
                        this.BaseEvulator.AmpMaps.TryGetValue(subElement.Value, out string nval);
                        text.Append(nval);
                    }
                    else
                    {
                        text.Append(subElement.Value);

                    }
                }
                else
                {
                    text.Append(subElement.InnerText());
                }

            }
            return text.ToString();
        }
        public TextEvulateResult EvulateValue(int start = 0, int end = 0, object vars = null)
        {
            var result = new TextEvulateResult();
            if (this.ElementType == TextElementType.CommentNode)
            {
                return null;
            }
            if (this.ElemName == "#text")
            {
                result.TextContent = this.value;
                return result;
            }
            if (this.ElementType == TextElementType.Parameter)
            {
                if (this.BaseEvulator.EvulatorTypes.Param != null)
                {
                    var evulator = Activator.CreateInstance(this.BaseEvulator.EvulatorTypes.Param) as BaseEvulator;
                    evulator.SetEvulator(this.BaseEvulator);
                    var vresult = evulator.Render(this, vars);
                    result.Result = vresult.Result;
                    if (vresult.Result == TextEvulateResultEnum.EVULATE_TEXT)
                    {
                        result.TextContent += vresult.TextContent;
                    }
                    return result;
                }
            }
            if (end == 0 || end > this.SubElementsCount) end = this.SubElementsCount;
            for (int i = start; i < end; i++)
            {
                var subElement = this.SubElements[i];
                Type targetType = null;
                if (subElement.ElementType == TextElementType.Parameter)
                {
                    targetType = this.BaseEvulator.EvulatorTypes.Param;
                }
                else
                {
                    if (subElement.ElemName != "#text")
                    {
                        targetType = this.BaseEvulator.EvulatorTypes[subElement.ElemName];
                        if (targetType == null)
                        {
                            targetType = this.BaseEvulator.EvulatorTypes.GeneralType;
                        }
                    }

                }
                TextEvulateResult vresult = null;
                if (targetType != null)
                {
                    var evulator = Activator.CreateInstance(targetType) as BaseEvulator;
                    evulator.SetEvulator(this.BaseEvulator);
                    vresult = evulator.Render(subElement, vars);
                    if (vresult == null) continue;
                    if (vresult.Result == TextEvulateResultEnum.EVULATE_DEPTHSCAN)
                    {
                        vresult = subElement.EvulateValue(vresult.Start, vresult.End, vars);
                    }
                }
                else
                {
                    vresult = subElement.EvulateValue(0, 0, vars);
                    if (vresult == null) continue;
                }
                if (vresult.Result == TextEvulateResultEnum.EVULATE_TEXT)
                {
                    result.TextContent += vresult.TextContent;
                }
                else if (vresult.Result == TextEvulateResultEnum.EVULATE_RETURN || vresult.Result == TextEvulateResultEnum.EVULATE_BREAK || vresult.Result == TextEvulateResultEnum.EVULATE_CONTINUE)
                {

                    result.Result = vresult.Result;
                    result.TextContent += vresult.TextContent;
                    break;
                }
            }
            return result;
        }
        public TextElements GetElementsHasAttributes(string name, bool depthscan = false, int limit = 0)
        {
            var elements = new TextElements();
            var lower = name.ToLower();
            for (int i = 0; i < this.subElements.Count; i++)
            {
                var elem = this.subElements[i];
                if (elem.ElemAttr.Count > 0 && lower == "*")
                {
                    elements.Add(elem);
                }
                else
                {
                    if (elem.ElemAttr.HasAttribute(lower))
                    {
                        elements.Add(elem);

                    }
                }

                if (depthscan && elem.SubElementsCount > 0)
                {
                    elements.AddRange(elem.GetElementsHasAttributes(name, depthscan));
                }

            }
            return elements;
        }
        public TextElements GetElementsByTagName(string name, bool depthscan = false, int limit = 0)
        {
            var elements = new TextElements();
            var lower = name.ToLower();
            for (int i = 0; i < this.subElements.Count; i++)
            {
                var elem = this.subElements[i];
                if (elem.ElemName.ToLower() == lower || lower == "*")
                {
                    elements.Add(elem);
                    if (limit > 0 && elements.Count >= limit)
                    {
                        break;
                    }
                }
                if (depthscan && elem.SubElementsCount > 0)
                {
                    elements.AddRange(elem.GetElementsByTagName(name, depthscan));
                }

            }
            return elements;
        }
        public TextElements GetElementsByPath(List<XPathBlock> block)
        {
            TextElements elements = new TextElements();
            for (int i = 0; i < this.SubElementsCount; i++)
            {
                var subelem = this.SubElements[i];
                if (subelem.ElementType != TextElementType.ElementNode) continue;
                for (int j = 0; j < block.Count; j++)
                {
                    var curblock = block[j];
                    if (curblock.IsAttributeSelector)
                    {
                        if (curblock.BlockName == "*")
                        {
                            if (subelem.ElemAttr.Count == 0)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (!subelem.ElemAttr.HasAttribute(curblock.BlockName))
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (curblock.BlockName != "*" && curblock.BlockName != subelem.ElemName)
                        {
                            continue;
                        }
                    }
                    if (elements.Contains(subelem) || (curblock.XPathExpressions.Count == 0 || XPathActions.XExpressionSuccess(subelem, curblock.XPathExpressions)))
                    {
                        elements.Add(subelem);
                        XPathActions.Eliminate(elements, curblock);
                    }
                    break;

                }
            }


            return elements;
        }
        public TextElements FindByXPath(XPathBlock block)
        {
            TextElements foundedElems = new TextElements();
            if (block.IsAttributeSelector)
            {
                foundedElems = this.GetElementsHasAttributes(block.BlockName, block.BlockType == XPathBlockType.XPathBlockScanAllElem);
            }
            else
            {
                if (!string.IsNullOrEmpty(block.BlockName))
                {
                    if (block.BlockName == ".")
                    {
                        foundedElems.Add(this);
                        return foundedElems;
                    }
                    else if (block.BlockName == "..")
                    {
                        foundedElems.Add(this.Parent);
                        return foundedElems;

                    }
                    else
                    {
                        foundedElems = this.GetElementsByTagName(block.BlockName, block.BlockType == XPathBlockType.XPathBlockScanAllElem);
                    }
                }
            }
            if (block.XPathExpressions.Count > 0 && foundedElems.Count > 0)
            {
                for (int i = 0; i < block.XPathExpressions.Count; i++)
                {
                    var exp = block.XPathExpressions[i];
                    foundedElems = XPathActions.Eliminate(foundedElems, exp);
                    if (foundedElems.Count == 0)
                    {
                        break;
                    }
                }


            }
            return foundedElems;
        }
        public TextElements FindByXPath(string xpath)
        {
            var elements = new TextElements();
            var xpathItem = XPathItem.ParseNew(xpath);
            elements = this.FindByXPathByBlockContainer(xpathItem.XPathBlockList);
            elements.SortItems();
            return elements;
        }
        private TextElements FindByXPathByBlockContainer(XPathBlockContainer container, TextElements senderitems = null)
        {
            var elements = new TextElements();
            bool inor = true;
            for (int i = 0; i < container.Count; i++)
            {
                var curblocks = container[i];
                if(curblocks.IsOr())
                {
                    inor = true;
                    continue;
                }
                if(!inor)
                {

                    if (curblocks.IsBlocks())
                    {
                        elements  = this.FindByXPathBlockList(curblocks as XPathBlocks, elements);
                    }
                    else
                    {
                        elements.AddRange(this.FindByXPathPar(curblocks as XPathPar, senderitems));
                    }
                }
                else
                {
                    if (curblocks.IsBlocks())
                    {
                        elements.AddRange(this.FindByXPathBlockList(curblocks as XPathBlocks));
                    }
                    else
                    {
                        elements.AddRange(this.FindByXPathPar(curblocks as XPathPar));
                    }
                }

                inor = false;
            }
            
            return elements;
        }

        public TextElements FindByXPathPar(XPathPar xpar, TextElements senderitems = null)
        {
            var elements = new TextElements();
            elements = this.FindByXPathByBlockContainer(xpar.XPathBlockList, senderitems);
            if (xpar.XPathExpressions.Count > 0 && elements.Count > 0)
            {
                elements.SortItems();
                for (int j = 0; j < xpar.XPathExpressions.Count; j++)
                {
                    var exp = xpar.XPathExpressions[j];
                    elements = XPathActions.Eliminate(elements, exp);
                    if (elements.Count == 0)
                    {
                        break;
                    }
                }
            }
            return elements;
        }
        public TextElements FindByXPathBlockList(XPathBlocks blocks, TextElements senderlist = null)
        {
            var elements = senderlist;
            for (int i = 0; i < blocks.Count; i++)
            {

                var xblock = blocks[i];
                if (i == 0 && senderlist == null)
                {
                    elements = FindByXPath(xblock);
                }
                else
                {
                    elements = elements.FindByXPath(xblock);
                }
            }
            return elements;
        }
        public TextElements FindByXPathOld(string xpath)
        {
            var elements = new TextElements();
            XPathFunctions fn = new XPathFunctions();
            var xpathblock = XPathItem.Parse(xpath);
            XPathActions actions = new XPathActions();
            actions.XPathFunctions = new XPathFunctions();
            for (int i = 0; i < xpathblock.XPathBlocks.Count; i++)
            {
                var xblock = xpathblock.XPathBlocks[i];
                if (i == 0)
                {
                    elements = FindByXPath(xblock);
                }
                else
                {
                    TextElements newelements = new TextElements();
                    for (int j = 0; j < elements.Count; j++)
                    {
                        var elem = elements[j];
                        var nextelems = elem.FindByXPath(xblock);
                        for (int k = 0; k < nextelems.Count; k++)
                        {
                            if (newelements.Contains(nextelems[k])) continue;
                            newelements.Add(nextelems[k]);
                        }
                    }
                    elements = newelements;
                }
            }
            return elements;
        }
        public bool XPathSuccessSingle(XPathBlock block)
        {
            if (this.ElementType != TextElementType.ElementNode || (block.BlockName != "*" && block.BlockName != this.ElemName)) return false;
            if(block.XPathExpressions.Count > 0)
            {
                int myIndex = this.Index;
                for (int i = 0; i < block.XPathExpressions.Count; i++)
                {
                    if (!XPathActions.XExpressionSuccess(this, block.XPathExpressions[i], null, myIndex)) return false;
                }
            }
            return true;
        }
        public TextElements XPathTest(XPathBlockContainer xcontainer)
        {
            for (int i = 0; i < this.SubElements.Count; i++)
            {
                var elem = this.SubElements[i];
                for (int j = 0; j < xcontainer.Count; j++)
                {
                    var blocks = xcontainer[j];
                    if(blocks.IsBlocks())
                    {

                    }
                }
            }

            return null;
        }
    }
}

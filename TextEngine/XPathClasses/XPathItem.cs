using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.XPathClasses
{
    public class XPathItem : IXPathBlockContainer
    {
        private XPathBlocks blocks;
        public XPathBlocks XPathBlocks
        {
            get
            {
                return blocks;
            }
            set
            {
                blocks = value;
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

        public XPathItem()
        {
            blocks = new XPathBlocks();
            xpathBlockList = new XPathBlockContainer();
        }
        public static XPathItem ParseNew(string xpath)
        {
            bool expisparexp = false;
            var pathitem = new XPathItem();
            var curblock = new XPathBlock();
            StringBuilder curstr = new StringBuilder();
            IXPathBlockContainer current = pathitem;
            XPathBlocks blocks = new XPathBlocks();
            current.XPathBlockList.Add(blocks);
            XPathExpressions curexp = curblock.XPathExpressions;
            for (int i = 0; i < xpath.Length; i++)
            {
                var cur = xpath[i];
                var next = (i + 1 < xpath.Length) ? xpath[i + 1] : '\0';
                if(cur == '|' ||cur == ')' || cur == '(')
                {
                    if (string.IsNullOrEmpty(curblock.BlockName))
                    {
                        curblock.BlockName = curstr.ToString();

                    }
                    if (!string.IsNullOrEmpty(curblock.BlockName) || curblock.IsAttributeSelector)
                    {
                        blocks.Add(curblock);
                    }
                    curstr.Clear();
                }
                if (cur == '[')
                {
                    if (curblock.XPathExpressions.Count == 0)
                    {
                        curblock.BlockName = curstr.ToString();
                        curstr.Clear();
                    }
                    XPathExpression newexp = null;
                    newexp = XPathExpression.Parse(xpath, ref i);
                    curexp.Add(newexp);
                    continue;
                }
                else if (cur == '|' || cur == '(')
                {
                    var lastitem = current.XPathBlockList.Last();
                    if (lastitem != null)
                    {
                        if (!lastitem.Any())
                        {
                           current.XPathBlockList.RemoveAt(current.XPathBlockList.Count - 1);
                        }
                    }

                    curblock = new XPathBlock();
                    curexp = curblock.XPathExpressions;
                    if (cur == '(')
                    {
                       var xpar = new XPathPar();
                        xpar.Parent = current;
                        current.XPathBlockList.Add(xpar);
                        current = xpar;
                    }
                    else
                    {
                        current.XPathBlockList.Add(new XPathOrItem());
                    }
                    blocks = new XPathBlocks();
                    current.XPathBlockList.Add(blocks);
                    continue;
                }
                else if(cur == ')')
                {
                    var lastitem = current.XPathBlockList.Last();
                    if (lastitem != null)
                    {
                        if (!lastitem.Any())
                        {
                           current.XPathBlockList.RemoveAt(current.XPathBlockList.Count - 1);
                        }
                    }
                    curexp = (current as XPathPar).XPathExpressions;
                    current = current.Parent;
                    expisparexp = true;
                    if (current == null)
                    {
                        throw new Exception("Syntax error");
                    }
                    blocks = new XPathBlocks();
                    current.XPathBlockList.Add(blocks);
                   // current.XPathBlockList.Add(blocks);
                    curblock = new XPathBlock();
                    continue;
                }
                else if (cur == '/')
                {

                    if (string.IsNullOrEmpty(curblock.BlockName))
                    {
                        curblock.BlockName = curstr.ToString();
                    }
                    if (string.IsNullOrEmpty(curblock.BlockName))
                    {
                        if (next == '/')
                        {
                            curblock.BlockType = XPathBlockType.XPathBlockScanAllElem;
                            i += 1;
                        }
                    }
                    else
                    {

                        blocks.Add(curblock);
                        curblock = new XPathBlock();
                        if (next == '/')
                        {
                            curblock.BlockType = XPathBlockType.XPathBlockScanAllElem;
                        }
                        curstr.Clear();
                    }
                    if(expisparexp)
                    {
                        curexp = curblock.XPathExpressions;
                        expisparexp = false;
                    }
                    continue;
                }
                else if (cur == '@')
                {
                    if (string.IsNullOrEmpty(curblock.BlockName))
                    {
                        curblock.IsAttributeSelector = true;
                    }
                    else
                    {
                        throw new Exception("Syntax Error");
                    }
                    continue;
                }
                curstr.Append(cur);
            }
            if (string.IsNullOrEmpty(curblock.BlockName))
            {
                curblock.BlockName = curstr.ToString();

            }
            if (!string.IsNullOrEmpty(curblock.BlockName) || curblock.IsAttributeSelector)
            {
                blocks.Add(curblock);
                //current.XPathBlockList.Add(curblock);
            }
            var sonitem = current.XPathBlockList.Last();
            if (sonitem != null)
            {
                if (!sonitem.Any())
                {
                    current.XPathBlockList.RemoveAt(current.XPathBlockList.Count - 1);
                }
            }
            return pathitem;
        }
        public static XPathItem Parse(string xpath)
        {
            var pathitem = new XPathItem();
            var curblock = new XPathBlock();
            XPathExpression curexp = null;
            StringBuilder curstr = new StringBuilder();
            for (int i = 0; i < xpath.Length; i++)
            {
                var cur = xpath[i];
                var next = (i + 1 < xpath.Length) ? xpath[i + 1] : '\0';

                if (cur == '[')
                {
                    if (curblock.XPathExpressions.Count == 0)
                    {
                        curblock.BlockName = curstr.ToString();
                        curstr.Clear();
                    }
                    curexp = XPathExpression.Parse(xpath, ref i);
                    curblock.XPathExpressions.Add(curexp);
                    curexp = null;
                    continue;
                }
                else if (cur == '/')
                {
                    if (string.IsNullOrEmpty(curblock.BlockName))
                    {
                        curblock.BlockName = curstr.ToString();
                    }
                    if (string.IsNullOrEmpty(curblock.BlockName))
                    {
                        if (next == '/')
                        {
                            curblock.BlockType = XPathBlockType.XPathBlockScanAllElem;
                            i += 1;
                        }
                    }
                    else
                    {
                        pathitem.XPathBlocks.Add(curblock);
                        curblock = new XPathBlock();
                        if (next == '/')
                        {
                            curblock.BlockType = XPathBlockType.XPathBlockScanAllElem;
                        }
                        curstr.Clear();
                    }
                    continue;
                }
                else if (cur == '@')
                {
                    if (string.IsNullOrEmpty(curblock.BlockName))
                    {
                        curblock.IsAttributeSelector = true;
                    }
                    else
                    {
                        throw new Exception("Syntax Error");
                    }
                    continue;
                }
                curstr.Append(cur);
            }
            if (string.IsNullOrEmpty(curblock.BlockName))
            {
                curblock.BlockName = curstr.ToString();

            }
            if (!string.IsNullOrEmpty(curblock.BlockName) || curblock.IsAttributeSelector)
            {
                pathitem.XPathBlocks.Add(curblock);
            }
            return pathitem;
        }

        public bool IsXPathPar()
        {
            return true;
        }
    }
}

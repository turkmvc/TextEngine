using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.ParDecoder;
using TextEngine.Text;

namespace TextEngine.XPathClasses
{
    public class XPathActions
    {
        public static bool XExpressionSuccess(TextElement item, object expressions, TextElements baselist = null, int curindex = -1, int totalcounts = -1)
        {
            XPathActions actions = new XPathActions
            {
                XPathFunctions = new XPathFunctions()
            };
            actions.XPathFunctions.BaseItem = item;
            if(totalcounts != -1)
            {
                actions.XPathFunctions.TotalItems = totalcounts;
            }
            else
            {
                if (baselist != null)
                {
                    actions.XPathFunctions.TotalItems = baselist.Count;
                }
                else
                {
                    actions.XPathFunctions.TotalItems = item.Parent.SubElementsCount;
                }
            }

            if(curindex != -1)
            {
                actions.XPathFunctions.ItemPosition = curindex;
            }
            else
            {
                actions.XPathFunctions.ItemPosition = item.Index;
            }
            object[] result = null;
            if (expressions is XPathExpression eexp)
            {
                result = actions.EvulateActionSingle(eexp);

            }
            else if (expressions is IXPathExpressionItems expitem)
            {
                result = actions.EvulateAction(expitem);

            }
            if (result[0] == null || (result[0] is bool b) && !b)
            {
                return false;
            }
            else if (TypeUtil.IsNumericType(result[0]))
            {
                int c = (int)Convert.ChangeType(result[0], TypeCode.Int32) - 1;
                int totalcount = 0;
                if (totalcounts != -1)
                {
                    totalcount = totalcounts;
                }
                else
                {
                    if (baselist != null)
                    {
                        totalcount = baselist.Count;

                    }
                    else
                    {
                        totalcount = item.Parent.SubElementsCount;
                    }
                }
                if (c < -1 || c >= totalcount)
                {
                    return false;
                }
                else
                {
                    return c == actions.XPathFunctions.ItemPosition;
                }

            }
            return true;
        }
        public static TextElements Eliminate(TextElements items, object expressions, bool issecondary = true)
        {
            int total = 0;
            int totalcount = items.Count;
            for (int i = 0; i < items.Count; i++)
            {
                bool result = false;
                if(issecondary)
                {
                    result = XExpressionSuccess(items[i], expressions, items, total, totalcount);
                }
                else
                {
                    result = XExpressionSuccess(items[i], expressions);
                }
                if(!result)
                {
                    items.RemoveAt(i);
                    i--;
                    total++;
                    continue;
                }
                total++;

            }
            return items;
        }
        public XPathFunctions XPathFunctions { get; set; }
        public object[] EvulateActionSingle(XPathExpression item, IXPathExpressionItem sender = null)
        {
            object curvalue = null;
            IXPathExpressionItem previtem = null;
            XPathExpressionItem xoperator = null;
            object waitvalue = null;
            string waitop = null;
            List<object> values = new List<object>();
            for (int j = 0; j < item.XPathExpressionItems.Count; j++)
            {
                var curitem = item.XPathExpressionItems[j];
                var nextitem = (j + 1 < item.XPathExpressionItems.Count) ? item.XPathExpressionItems[j + 1] : null;
                string nextop = null;
                XPathExpressionItem nextExp = null;
                if (nextitem != null && !nextitem.IsSubItem)
                {
                    nextExp = nextitem as XPathExpressionItem;
                    nextop = (nextExp != null && nextExp.IsOperator) ? nextExp.Value.ToString() : null;
                }
                object expvalue = null;
                if (curitem.IsSubItem)
                {
                    expvalue = EvulateAction((IXPathExpressionItems)curitem);
                    if (!previtem.IsSubItem)
                    {
                        var prevexp = previtem as XPathExpressionItem;
                        if (prevexp.IsOperator)
                        {
                            expvalue = ((object[])expvalue)[0];
                        }
                        else
                        {
                            if (XPathFunctions != null)
                            {
                                if (curitem.ParChar == '[')
                                {
                                    var xitems = XPathFunctions.BaseItem.FindByXPath(prevexp.Value.ToString());
                                    if (xitems.Count > 0)
                                    {
                                        xitems = Eliminate(xitems, (IXPathExpressionItems)curitem);
                                    }
                                    if (xitems.Count > 0)
                                    {
                                        expvalue = true;
                                    }
                                    else
                                    {
                                        expvalue = false;
                                    }
                                    if (curvalue == null)
                                    {
                                        curvalue = expvalue;
                                        previtem = curitem;
                                        continue;
                                    }
                                }
                                else if (curitem.ParChar == '(')
                                {
                                    var method = XPathFunctions.GetMetohdByName(prevexp.Value.ToString());
                                    if (method != null)
                                    {
                                        expvalue = ComputeActions.CallMethodDirect(XPathFunctions, method, (object[])expvalue);
                                        if (curvalue == null)
                                        {
                                            curvalue = expvalue;
                                            previtem = curitem;
                                            continue;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    var expItem = curitem as XPathExpressionItem;
                    if (nextitem != null && nextitem.IsSubItem)
                    {
                        previtem = curitem;
                        continue;
                    }

                    if (expItem.IsOperator)
                    {
                        if (expItem.Value.ToString() == ",")
                        {
                            if (waitop != null)
                            {
                                curvalue = ComputeActions.OperatorResult(waitvalue, curvalue, waitop);
                                waitvalue = null;
                                waitop = null;
                            }
                            values.Add(curvalue);
                            curvalue = null;
                            xoperator = null;
                            continue;
                        }
                        string opstr = expItem.Value.ToString();
                        if (opstr == "||" || opstr == "|" || opstr == "or" || opstr == "&&" || opstr == "&" || opstr == "and")
                        {
                            if (waitop != null)
                            {
                                curvalue = ComputeActions.OperatorResult(waitvalue, curvalue, waitop);
                                waitvalue = null;
                                waitop = null;
                            }
                            bool state = PhpFuctions.not_empty(curvalue);
                            if (opstr == "||" || opstr == "|" || opstr == "or")
                            {
                                if (state)
                                {
                                    values.Add(true);
                                    return values.ToArray();
                                }
                                else curvalue = null;
                            }
                            else
                            {
                                if (!state)
                                {
                                    values.Add(false);
                                    return values.ToArray();
                                }
                                else curvalue = null;
                            }
                            xoperator = null;
                        }
                        else
                        {
                            xoperator = expItem;
                        }
                    
                        previtem = curitem;
                        continue;
                    }
                    else
                    {
                        if (expItem.IsVariable)
                        {
                            if (expItem.Value.ToString().StartsWith("@"))
                            {
                                string s = expItem.Value.ToString().Substring(1);

                                if ((nextExp == null || !nextExp.IsOperator) && (sender is null || !sender.IsSubItem || sender.ParChar != '('))
                                {
                                    expvalue = this.XPathFunctions.BaseItem.ElemAttr.HasAttribute(s);
                                }
                                else
                                {
                                    expvalue = this.XPathFunctions.BaseItem.GetAttribute(s);
                                }
                                if (expvalue == null) expvalue = false;
                            }
                            else
                            {
                                var items = this.XPathFunctions.BaseItem.FindByXPath(expItem.Value.ToString());
                                if (items.Count == 0)
                                {
                                    expvalue = false;
                                }
                                else
                                {
                                    expvalue = items[0].Inner();
                                }
                            }
                        }
                        else
                        {
                            expvalue = expItem.Value;

                        }
                        if (curvalue == null)
                        {
                            curvalue = expvalue;
                            previtem = curitem;
                            continue;
                        }


                    }
                }
                if (xoperator != null)
                {
                    if (XPathExpression.priotirystop.Contains(xoperator.Value.ToString()))
                    {
                        if (waitop != null)
                        {
                            curvalue = ComputeActions.OperatorResult(waitvalue, curvalue, waitop);
                            waitvalue = null;
                            waitop = null;
                        }
                    }
                    if ((xoperator.Value.ToString() != "+" && xoperator.Value.ToString() != "-") || nextop == null || XPathExpression.priotirystop.Contains(nextop))
                    {
                        curvalue = ComputeActions.OperatorResult(curvalue, expvalue, xoperator.Value.ToString());

                    }
                    else
                    {
                        waitvalue = curvalue;
                        waitop = xoperator.Value.ToString();
                        curvalue = expvalue;
                    }
                    xoperator = null;
                }
                previtem = curitem;
            }
            if (waitop != null)
            {
                curvalue = ComputeActions.OperatorResult(waitvalue, curvalue, waitop);
                waitvalue = null;
                waitop = null;
            }
            values.Add(curvalue);
            return values.ToArray();
        }
        public object[] EvulateAction(IXPathExpressionItems item)
        {
            List<object> values = new List<object>();
            for (int i = 0; i < item.XPathExpressions.Count; i++)
            {
                var curExp = item.XPathExpressions[i];
                var results = EvulateActionSingle(curExp, item);
                values.AddRange(results);
            }
            return values.ToArray();
        }
    }
}

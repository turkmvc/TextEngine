using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Extensions;

namespace TextEngine.XPathClasses
{
    public class XPathExpression
    {
        public static List<string> protirystop = new List<string>()
        {
            "and",
            "&&",
            "||",
            "|",
            "==",
            "=",
            ">",
            "<",
            ">=",
            "<=",
            "or",
            "+",
            "-",
            ","
        };
        private static List<string> operators = new List<string>
        {
            "and",
            "mod",
            "div",
            "or",
            "!=",
            "==",
            ">=",
            "<=",
            "&&",
            "||",
            "+",
            "-",
            "*",
            "/",
            "%",
            "=",
            "<",
            ">",
            ","
        };
        public XPathExpressionItems XPathExpressionItems { get; set; }
        public XPathExpression()
        {
            XPathExpressionItems = new XPathExpressionItems();
        }
        public XPathExpression Parent { get; set; }
        public static XPathExpression Parse(string input, ref int istate)
        {
            bool inquot = false;
            char quotchar = '\0';
            bool inspec = false;
            StringBuilder curstr = new StringBuilder();
            var elem = new XPathExpression();
            if(input[istate] == '['  || input[istate] == '(')
            {
                istate++;
            }
            for (int i = istate; i < input.Length; i++)
            {

                var cur = input[i];
                var next = (i + 1 < input.Length) ? input[i + 1] : '\0';
                if (inspec)
                {
                    inspec = false;
                    curstr.Append(cur);
                    continue;
                }
                if (!inquot && cur == '\'' || cur == '"')
                {
                    if (curstr.Length > 0 || quotchar != '\0')
                    {
                        elem.XPathExpressionItems.Add(new XPathExpressionItem()
                        {
                            QuotChar = quotchar,
                            IsOperator = quotchar == '\0' && operators.Contains(curstr.ToString()),
                            Value = curstr.ToString()
                        });
                    }
                    curstr.Clear();
                    inquot = true;
                    quotchar = cur;
                    curstr.Clear();
                    continue;
                }
                if (inquot)
                {
                    if (cur == quotchar)
                    {
                        inquot = false;
                    }
                    else
                    {
                        curstr.Append(cur);
                    }
                    continue;
                }

                if (cur == '\\')
                {
                    inspec = true;
                    continue;
                }
                if (cur == ' ' && next == ' ') continue;

                if(cur == '-' || cur == '/')
                {

                    if(curstr.Length > 0 && !curstr.ToString().IsNumeric())
                    {
                        curstr.Append(cur);
                        continue;
                    }
                }
                if(cur != ' ' && curstr.Length > 0)
                {
                    if(!operators.Contains(cur.ToString()) && operators.Contains(curstr.ToString()) || operators.Contains(cur.ToString()) && !operators.Contains(curstr.ToString()))
                    {
                        if (curstr.Length > 0 || quotchar != '\0')
                        {
                            elem.XPathExpressionItems.Add(new XPathExpressionItem()
                            {
                                QuotChar = quotchar,
                                IsOperator = quotchar == '\0' && operators.Contains(curstr.ToString()),
                                Value = curstr.ToString()
                            });
                        }
                        curstr.Clear();
                    }
                }
                if (cur == ' ' || cur == ':' || cur == ']' || cur == ')' )
                {
                    
                    if(curstr.Length > 0 || quotchar != '\0')
                    {
                        elem.XPathExpressionItems.Add(new XPathExpressionItem()
                        {
                            QuotChar = quotchar,
                            IsOperator = quotchar == '\0' && operators.Contains(curstr.ToString()),
                            Value = curstr.ToString()
                        });
                    }
                    quotchar = '\0';
                    curstr.Clear();

                    if (cur == ']' || cur == ')')
                    {
                        istate = i;
                        break;
                    }
                    continue;
                }
                
                if(cur == '(' || cur == '[')
                {
                    
                    if(curstr.Length > 0 || quotchar != '\0')
                    {
                        elem.XPathExpressionItems.Add(new XPathExpressionItem()
                        {

                            QuotChar = quotchar,
                            IsOperator = quotchar == '\0' && operators.Contains(curstr.ToString()),
                            Value = curstr.ToString()

                        });
                        curstr.Clear();
                    }
                    var subElem = XPathExpression.Parse(input, ref i);
                    var subitem = new XPathExpressionSubItem();
                    subitem.ParChar = cur;
                    subElem.Parent = elem;
                    subitem.XPathExpressions.Add(subElem);
                    elem.XPathExpressionItems.Add(subitem);
                    continue;
                }

                curstr.Append(cur);

            }
            if(curstr.Length > 0 || quotchar != '\0')
            {
                elem.XPathExpressionItems.Add(new XPathExpressionItem()
                {

                    QuotChar = quotchar,
                    IsOperator = quotchar == '\0' && operators.Contains(curstr.ToString()),
                    Value = curstr.ToString()
                      
                });
            }
            return elem;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Extensions;

namespace TextEngine.ParDecoder
{
    public class ParDecode
    {
        public string Text { get; set; }
        private int TextLength { get { return this.Text.Length; } }
        private int pos;
        public ParItem items;
        public ParItem Items
        {
            get
            {
                return this.items;
            }
            set
            {
                this.items = value;
            }
        }

        public ParDecode(string text)
        {
            this.Text = text;
            this.Items = new ParItem();
            this.Items.ParName = "(";
        }

        public void Decode()
        {
            InnerItem parentItem = this.Items;
            bool isopened = false;
            for (int i = 0; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                var prev = '\0';
                if (i - 1 >= 0)
                {
                    prev = this.Text[i - 1];
                }
                if ((prev != ')' && prev != ']' && prev != '}' ) && (cur == '=' || cur == '>' || cur == '<' || cur == '?' || cur == ':'))
                {
                    if(isopened)
                    {
                        InnerItem item = new InnerItem();
                        item.IsOperator = true;
                        if((prev == '>' && cur == '=') || (prev == '<' && cur == '=') || (prev == '!' && cur == '=') || (prev == '=' && cur == '>'))
                        {
                            item.Value = prev.ToString() + cur.ToString();
                        }
                        else

                        {
                            item.Value = cur;
                        }
                        parentItem = parentItem.Parent;
                        isopened = false;
                        parentItem.InnerItems.Add(item);
                        i--;
   
                    }
                    else
                    {
                        var item = new ParItem
                        {
                            Parent = parentItem,
                            ParName = "("
                        };
                        parentItem.InnerItems.Add(item);
                        parentItem = item;
                        isopened = true;
                    }
                    continue;
                }
                if (cur == '(' || cur == '[' || cur == '{')
                {
                    if(isopened)
                    {
                        //isopened = false;   
                    }
                    var item = new ParItem
                    {
                        Parent = parentItem,
                        ParName = cur.ToString(),
                    };
                    parentItem.InnerItems.Add(item);
                    parentItem = item;
                    continue;
                }
                else if (cur == ')' || cur == ']' || cur == '}')
                {
                    parentItem = parentItem.Parent;
                    if (parentItem == null)
                    {
                        throw new Exception("Syntax Error");
                    }
                    continue;
                }
                var result = this.DecodeText(i, isopened);
                parentItem.InnerItems.AddRange(result);
                i = this.pos;
            }
        }
        private InnerItemsList DecodeText(int start, bool autopar = false)
        {
            var inspec = false;
            var inquot = false;
            char qutochar = '\0';
            var innerItems = new InnerItemsList();
            StringBuilder value = new StringBuilder();
            for (int i = start; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                char next = '\0';
                if (i + 1 < this.TextLength)
                {
                    next = this.Text[i + 1];
                }
                if (inspec)
                {
                    value.Append(cur);
                    inspec = false;
                    continue;
                }
                if (cur == '\\')
                {
                    inspec = true;
                    continue;
                }
                if (!inquot)
                {
                    if (cur == ' ' || cur == '\t')
                    {
                        continue;
                    }
                    if (cur == '\'' || cur == '\"')
                    {
                        inquot = true;
                        qutochar = cur;
                        continue;
                    }
                    if (cur == '+' || cur == '-' || cur == '*' ||
                    cur == '/' || cur == '%' || cur == '!' ||
                    cur == '=' || cur == '&' || cur == '|' ||
                    cur == ')' || cur == '(' || cur == ',' ||
                    cur == '[' || cur == ']' || cur == '^' ||
                    cur == '<' || cur == '>' || cur == '{' ||
                    cur == '}' || (cur == ':' && next != ':') || cur == '?' || cur == '.')
                    {
                        if (value.Length > 0)
                        {
                            innerItems.Add(this.Inner(value.ToString(), qutochar));
                            value.Clear();
                        }
                        if (cur == '[' || cur == '(' || cur == '{')
                        {
                           
                            this.pos = i - 1;
                            return innerItems;
                        }
                        if(autopar && (cur == '?' || cur == ':' || cur == '=' || cur == '<' || cur == '>' || (cur == '!' && next == '=')))
                        {
                           
                            if ((cur == '=' && next == '>') || (cur == '!' && next == '=') || (cur == '>' && next == '=') || (cur == '<' && next == '='))
                            {
                                this.pos = i;
                            }
                            else
                            {
                                this.pos = i - 1;
                            }
                            return innerItems;
                        }

                        if (cur != '(' && cur != ')' && cur != '[' && cur != ']' && cur != '{' && cur != '}')
                        {
                            var inner2 = new InnerItem
                            {
                                IsOperator = true
                            };
                            if ((cur == '=' && next == '>') || (cur == '!' && next == '=') || (cur == '>' && next == '=') || (cur == '<' && next == '='))
                            {
                                inner2.Value = cur.ToString() + next.ToString();
                                i++;
                            }
                            else if ((cur == '=' || cur == '&' || cur == '|') && cur == next)
                            {
                                inner2.Value = cur.ToString() + next.ToString();
                                i++;
                            }
                            else
                            {
                                inner2.Value = cur.ToString();
                            }
                            string valuestr =(string) inner2.Value;
                            innerItems.Add(inner2);
                            qutochar = '\0';
                            if (valuestr == "=" ||valuestr == "<=" || valuestr == ">=" || valuestr == "<" || valuestr == ">" || valuestr == "!=" || valuestr == "==")
                            {
                                this.pos = i - 1;
                                return innerItems;
                            }

                        }
                        else
                        {
                            this.pos = i - 1;
                            return innerItems;
                        }
                        continue;
                    }
                }
                else
                {
                    if (cur == qutochar)
                    {
                        inquot = false;
                        continue;
                    }
                }

                if (cur == ':' && next == ':')
                {
                    value.Append(':');
                    i++;
                }
                value.Append(cur);

            }
            if (value.Length > 0)
            {
                innerItems.Add(this.Inner(value.ToString(), qutochar));
            }
            this.pos = this.TextLength;

            return innerItems;
        }
        private InnerItem Inner(string current, char quotchar)
        {
            var inner = new InnerItem
            {
                Value = current,
                Quote = quotchar,
                InnerType = InnerType.TYPE_STRING
            };

            if (inner.Quote != '\'' && inner.Quote != '"')
            {
                if (current == "true" || current == "false")
                {
                    inner.InnerType = InnerType.TYPE_BOOLEAN;
                    if(current == "true")
                    {
                        inner.Value = true;
                    }
                    else
                    {
                        inner.Value = false;
                    }
                }
                else if (inner.Quote == '\0' && current.IsNumeric())
                {
                    inner.InnerType = InnerType.TYPE_NUMERIC;
                    inner.Value = double.Parse(current.ToString());
                }
                else
                {
                    inner.InnerType = InnerType.TYPE_VARIABLE;
                }
            }
            return inner;
        }
    }
}

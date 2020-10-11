using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public class TextEvulatorParser
    {
        public string Text { get; set; }
        private int pos = 0;
        private bool in_noparse = false;
       
        public int TextLength
        {
            get
            {
                return this.Text.Length;
            }
        }
        private TextEvulator evulator;
        public TextEvulator Evulator
        {
            get
            {
                return evulator;
            }
            private set
            {
                evulator = value;
            }
        }
        public TextEvulatorParser(TextEvulator baseevulator)
        {
            this.evulator = baseevulator;
        }

        public void Parse(TextElement baseitem, string text)
        {
            this.Text = text;
            this.Evulator.IsParseMode = true;
            TextElement currenttag = null;
            if (baseitem == null)
            {
                currenttag = this.Evulator.Elements;
            }
            else
            {
                currenttag = baseitem;
            }
            currenttag.BaseEvulator = this.Evulator;
            for (int i = 0; i < TextLength; i++)
            {
                TextElement tag = this.ParseTag(i, currenttag);
                if (tag == null)
                {
                    i = this.pos;
                    continue;
                }
                tag.BaseEvulator = this.Evulator;

                if (!tag.SlashUsed)
                {

                    currenttag.AddElement(tag);
                    if (tag.DirectClosed)
                    {
                        this.Evulator.OnTagClosed(tag);
                    }
                }
                if (tag.SlashUsed)
                {
                    TextElement prevtag = this.GetNotClosedPrevTag(tag);
                    //$alltags = $this->GetNotClosedPrevTagUntil($tag, $tag->elemName);
                    // int total = 0;
                    /** @var TextElement $baseitem */
                    TextElement previtem = null;
                    while (prevtag != null)
                    {

                        if (!prevtag.NameEquals(tag.ElemName, true))
                        {
                            var elem = new TextElement
                            {
                                ElemName = prevtag.ElemName,
                                ElemAttr = prevtag.ElemAttr.CloneWCS(),
                                AutoAdded = true,
                                BaseEvulator = this.Evulator
                            };
                            prevtag.Closed = true;

                            if (previtem != null)
                            {
                                previtem.Parent = elem;
                                elem.AddElement(previtem);

                            }
                            else
                            {
                                currenttag = elem;
                            }
                            previtem = elem;

                        }
                        else
                        {
                            if (prevtag.ElemName != tag.ElemName)
                            {
                                prevtag.AliasName = tag.ElemName;
                                //Alias
                            }
                            if (previtem != null)
                            {
                                previtem.Parent = prevtag.Parent;
                                previtem.Parent.AddElement(previtem);
                            }
                            else
                            {
                                currenttag = prevtag.Parent;
                            }
                            prevtag.Closed = true;
                            break;
                        }
                        prevtag = this.GetNotClosedPrevTag(prevtag);


                    }
                    if (prevtag == null && this.Evulator.ThrowExceptionIFPrevIsNull)
                    {
                        this.Evulator.IsParseMode = false;
                        throw new Exception("Syntax Error");
                    }
                }
                else if (!tag.Closed)
                {
                    currenttag = tag;
                }


                i = this.pos;
            }
            this.pos = 0;
            this.in_noparse = false;
            this.Evulator.IsParseMode = false;
        }
        private TextElements GetNotClosedPrevTagUntil(TextElement tag, string name)
        {
            var array = new TextElements();
            var stag = this.GetNotClosedPrevTag(tag);
            while (stag != null)
            {

                if (stag.ElemName == name)
                {
                    array.Add(stag);
                    break;
                }
                array.Add(stag);
                stag = this.GetNotClosedPrevTag(stag);
            }
            return array;
        }

        private TextElement GetNotClosedPrevTag(TextElement tag)
        {
            var parent = tag.Parent;
            while (parent != null)
            {
                if (parent.Closed || parent.ElemName == "#document")
                {
                    return null;
                }
                return parent;
            }
            return null;
        }

        private TextElement GetNotClosedTag(TextElement tag, string name)
        {
            var parent = tag.Parent;
            while (parent != null)
            {
                if (parent.Closed) return null;
                if (parent.NameEquals(name))
                {
                    return parent;
                }
                parent = parent.Parent;
            }
            return null;
        }
        private string DecodeAmp(int start, bool decodedirect = true)
        {
            StringBuilder current = new StringBuilder();

            for (int i = start; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                if (cur == ';')
                {
                    this.pos = i;
                    if (decodedirect)
                    {
                        if (this.Evulator.AmpMaps.TryGetValue(current.ToString(), out string key))
                        {
                            return key;
                        }
                    }
                    else
                    {
                        return current.ToString();
                    }

                    return null;
                }
                if (!char.IsLetterOrDigit(cur))
                {
                    break;
                }
                current.Append(cur);
            }
            this.pos = this.TextLength;
            return '&' + current.ToString();
        }
        private TextElement ParseTag(int start, TextElement parent = null)
        {
            bool inspec = false;
            TextElement tagElement = new TextElement
            {
                Parent = parent
            };
            bool istextnode = false;
            bool intag = false;
            for (int i = start; i < this.TextLength; i++)
            {
                if (this.Evulator.NoParseEnabled && this.in_noparse)
                {
                    istextnode = true;
                    tagElement.ElemName = "#text";
                    tagElement.ElementType = TextElementType.TextNode;
                    tagElement.Closed = true;
                }
                else
                {
                    var cur = this.Text[i];
                    if (!inspec)
                    {
                        if (cur == this.Evulator.LeftTag)
                        {
                            if (intag)
                            {
                                this.Evulator.IsParseMode = false;
                                throw new Exception("Syntax Error");
                            }
                            intag = true;
                            continue;
                        }
                        else if (this.Evulator.DecodeAmpCode && cur == '&')
                        {
                            string ampcode = this.DecodeAmp(i + 1, false);
                            i = this.pos;
                            if (ampcode.StartsWith("&"))
                            {
                                this.Evulator.IsParseMode = false;
                                throw new Exception("Syntax Error");
                            }
                            tagElement.AutoClosed = true;
                            tagElement.Closed = true;
                            tagElement.Value = ampcode;
                            tagElement.ElementType = TextElementType.EntityReferenceNode;
                            tagElement.ElemName = "#text";
                            return tagElement;
                        }
                        else
                        {
                            if (!intag)
                            {
                                istextnode = true;
                                tagElement.ElemName = "#text";
                                tagElement.ElementType = TextElementType.TextNode;
                                tagElement.Closed = true;
                            }
                        }
                    }
                    if (!inspec && cur == this.Evulator.RightTag)
                    {
                        if (!intag)
                        {
                            this.Evulator.IsParseMode = false;
                            throw new Exception("Syntax Error");
                        }
                        intag = false;
                    }
                }
                this.pos = i;
                if (!intag || istextnode)
                {
                    tagElement.Value = this.ParseInner();
                    if (!this.in_noparse && tagElement.ElementType == TextElementType.TextNode && string.IsNullOrEmpty(tagElement.Value))
                    {
                        return null;
                    }
                    intag = false;
                    if (this.in_noparse)
                    {
                        parent.AddElement(tagElement);
                        var elem = new TextElement
                        {
                            Parent = parent,
                            ElemName = this.Evulator.NoParseTag,
                            SlashUsed = true
                        };
                        this.in_noparse = false;
                        return elem;
                    }
                    return tagElement;
                }
                else
                {
                    this.ParseTagHeader(ref tagElement);
                    intag = false;
                    if (this.Evulator.NoParseEnabled && tagElement.ElemName == this.Evulator.NoParseTag)
                    {
                        this.in_noparse = true;
                    }
                    return tagElement;

                }
            }
            return tagElement;
        }
        private void ParseTagHeader(ref TextElement tagElement)
        {
            bool inquot = false;
            bool inspec = false;
            StringBuilder current = new StringBuilder();
            bool namefound = false;
            //bool inattrib = false;
            bool firstslashused = false;
            bool lastslashused = false;
            StringBuilder currentName = new StringBuilder();
#pragma warning disable CS0219 // Değişken atandı ancak değeri hiç kullanılmadı
            bool quoted = false;
#pragma warning restore CS0219 // Değişken atandı ancak değeri hiç kullanılmadı
            char quotchar = '\0';
            bool initial = false;
            for (int i = this.pos; i < this.TextLength; i++)
            {
                var cur = this.Text[i];


                if (inspec)
                {
                    inspec = false;
                    current.Append(cur);
                    continue;
                }
                var next = '\0';
                var next2 = '\0';
                if (i + 1 < this.TextLength)
                {
                    next = this.Text[i + 1];
                }
                if (i + 2 < this.TextLength)
                {
                    next2 = this.Text[i + 2];
                }
                if (tagElement.ElementType == TextElementType.CDATASection)
                {
                    if (cur == ']' && next == ']' && next2 == this.Evulator.RightTag)
                    {
                        tagElement.Value = current.ToString();
                        this.pos = i += 2;
                        return;
                    }
                    current.Append(cur);
                    continue;
                }
                if (this.Evulator.AllowXMLTag && cur == '?' && !namefound && current.Length == 0)
                {
                    tagElement.Closed = true;
                    tagElement.AutoClosed = true;
                    tagElement.ElementType = TextElementType.XMLTag;
                    continue;

                }
                if (this.Evulator.SupportExclamationTag && cur == '!' && !namefound && current.Length == 0)
                {
                    tagElement.Closed = true;
                    tagElement.AutoClosed = true;
                    if (i + 8 < this.TextLength)
                    {
                        var mtn = this.Text.Substring(i, 8);
                        if (this.Evulator.SupportCDATA && mtn == "![CDATA[")
                        {
                            tagElement.ElementType = TextElementType.CDATASection;
                            tagElement.ElemName = "#cdata";
                            namefound = true;
                            i += 7;
                            continue;
                        }
                    }
                }
                if (cur == '\\' && tagElement.ElementType != TextElementType.CommentNode)
                {
                    if (!namefound && tagElement.ElementType != TextElementType.Parameter)
                    {
                        this.Evulator.IsParseMode = false;
                        throw new Exception("Syntax Error");
                    }
                    inspec = true;
                    continue;
                }

                if (!initial && cur == '!' && next == '-' && next2 == '-')
                {
                    tagElement.ElementType = TextElementType.CommentNode;
                    tagElement.ElemName = "#summary";
                    tagElement.Closed = true;
                    i += 2;
                    continue;
                }
                if (tagElement.ElementType == TextElementType.CommentNode)
                {
                    if (cur == '-' && next == '-' && next2 == this.Evulator.RightTag)
                    {
                        tagElement.Value = current.ToString();
                        this.pos = i + 2;
                        return;
                    }
                    else
                    {
                        current.Append(cur);
                    }
                    continue;
                }
                initial = true;
                if (this.Evulator.DecodeAmpCode && tagElement.ElementType != TextElementType.CommentNode && cur == '&')
                {
                    current.Append(this.DecodeAmp(i + 1));
                    i = this.pos;
                    continue;
                }
                if (tagElement.ElementType == TextElementType.Parameter && this.Evulator.ParamNoAttrib)
                {
                    if (cur != this.Evulator.RightTag)
                    {
                        current.Append(cur);
                        continue;
                    }
                }

                if (firstslashused && namefound)
                {
                    if (cur != this.Evulator.RightTag)
                    {
                        if (cur == ' ' && next != '\t' && next != ' ')
                        {
                            this.Evulator.IsParseMode = false;
                            throw new Exception("Syntax Error");
                        }
                    }
                }
                if (cur == '"' || cur == '\'')
                {
                    if (!namefound || currentName.Length == 0)
                    {
                        this.Evulator.IsParseMode = false;
                        throw new Exception("Syntax Error");
                    }
                    if (inquot && cur == quotchar)
                    {
                        if (currentName.ToString() == "##set_TAG_ATTR##")
                        {
                            tagElement.TagAttrib = current.ToString();

                        }
                        else if (currentName.Length > 0)
                        {

                            tagElement.ElemAttr.SetAttribute(currentName.ToString(), current.ToString());
                        }
                        currentName.Clear();
                        current.Clear();
                        inquot = false;
                        quoted = true;
                        continue;
                    }
                    else if (!inquot)
                    {
                        quotchar = cur;
                        inquot = true;
                        continue;
                    }


                }
                if (!inquot)
                {
                    if (cur == this.Evulator.ParamChar && !namefound && !firstslashused)
                    {
                        tagElement.ElementType = TextElementType.Parameter;
                        tagElement.Closed = true;
                        continue;
                    }
                    if (cur == '/')
                    {
                        if (!namefound && current.Length > 0)
                        {
                            namefound = true;
                            tagElement.ElemName = current.ToString();
                            current.Clear();
                        }
                        if (namefound)
                        {
                            lastslashused = true;
                        }
                        else
                        {
                            firstslashused = true;
                        }
                        continue;
                    }
                    if (cur == '=')
                    {
                        if (namefound)
                        {
                            if (current.Length == 0)
                            {
                                this.Evulator.IsParseMode = false;
                                throw new Exception("Syntax Error");
                            }
                            currentName.Clear();
                            currentName.Append(current.ToString());
                            current.Clear();
                        }
                        else
                        {
                            namefound = true;
                            tagElement.ElemName = current.ToString();
                            current.Clear();
                            currentName.Clear();
                            currentName.Append("##set_TAG_ATTR##");
                        }
                        continue;
                    }
                    if (tagElement.ElementType == TextElementType.XMLTag)
                    {
                        if (cur == '?' && next == this.Evulator.RightTag)
                        {
                            cur = next;
                            i++;
                        }
                    }

                    if (cur == this.Evulator.LeftTag)
                    {
                        this.Evulator.IsParseMode = false;
                        throw new Exception("Syntax Error");
                    }
                    if (cur == this.Evulator.RightTag)
                    {
                        if (!namefound)
                        {
                            tagElement.ElemName = current.ToString();
                            current.Clear();
                        }
                        if (currentName.ToString() == "##set_TAG_ATTR##")
                        {
                            tagElement.TagAttrib = current.ToString();
                        }
                        else if (currentName.Length > 0)
                        {
                            tagElement.SetAttribute(currentName.ToString(), current.ToString());
                        }
                        else if (current.Length > 0)
                        {
                            tagElement.SetAttribute(current.ToString(), null);
                        }
                        tagElement.SlashUsed = firstslashused;
                        if (lastslashused)
                        {
                            tagElement.DirectClosed = true;
                            tagElement.Closed = true;
                        }
                        if (this.Evulator.AutoClosedTags.Contains(tagElement.ElemName))
                        {
                            tagElement.Closed = true;
                            tagElement.AutoClosed = true;
                        }
                        this.pos = i;
                        return;
                    }
                    if (cur == ' ')
                    {
                        if (next == ' ' || next == '\t' || next == this.Evulator.RightTag) continue;
                        if (!namefound && !PhpFuctions.empty(current))
                        {
                            namefound = true;
                            tagElement.ElemName = current.ToString();
                            current.Clear();

                        }
                        else if (namefound)
                        {
                            if (currentName.ToString() == "##set_TAG_ATTR##")
                            {
                                tagElement.TagAttrib = current.ToString();
                                quoted = false;
                                currentName.Clear();
                                current.Clear();
                            }
                            else if (!PhpFuctions.empty(currentName))
                            {
                                tagElement.SetAttribute(currentName.ToString(), current.ToString());
                                currentName.Clear();
                                current.Clear();
                                quoted = false;
                            }
                            else if (!PhpFuctions.empty(current))
                            {
                                tagElement.SetAttribute(current.ToString(), null);
                                current.Clear();
                                quoted = false;
                            }
                        }
                        continue;
                    }
                }
                current.Append(cur);
            }
            this.pos = this.TextLength;
        }
        private string ParseInner()
        {
            StringBuilder text = new StringBuilder();
            bool inspec = false;
            StringBuilder nparsetext = new StringBuilder();
            bool parfound = false;
            StringBuilder waitspces = new StringBuilder();

            for (int i = this.pos; i < this.TextLength; i++)
            {
                var cur = this.Text[i];
                var next = (i + 1 < this.TextLength) ? this.Text[i + 1] : '\0';
                if (inspec)
                {
                    inspec = false;
                    text.Append(cur);
                    continue;
                }
                if (cur == '\\')
                {
                    inspec = true;
                    continue;
                }
                //if (this.DecodeAmpCode && cur == '&')
                //{
                //    text.Append(this.DecodeAmp(i + 1));
                //    i = this.pos;
                //    continue;
                //}
                if (this.Evulator.NoParseEnabled && this.in_noparse)
                {
                    if (parfound)
                    {
                        if (cur == this.Evulator.LeftTag || cur == '\r' || cur == '\n' || cur == '\t' || cur == ' ')
                        {
                            text.Append(this.Evulator.LeftTag + nparsetext.ToString());
                            parfound = (cur == this.Evulator.LeftTag);
                            nparsetext.Clear();
                        }
                        else if (cur == this.Evulator.RightTag)
                        {
                            if (nparsetext.ToString() == '/' + this.Evulator.NoParseTag)
                            {
                                parfound = false;
                                this.pos = i;
                                if (this.Evulator.TrimStartEnd)
                                {
                                    return text.ToString().Trim();
                                }
                                return text.ToString();
                            }
                            else
                            {
                                text.Append(this.Evulator.LeftTag + nparsetext.ToString() + cur);
                                parfound = false;
                                nparsetext.Clear();
                            }
                            continue;
                        }

                    }
                    else
                    {
                        if (cur == this.Evulator.LeftTag)
                        {
                            parfound = true;
                            continue;
                        }
                    }
                }
                else
                {
                    if (!inspec && cur == this.Evulator.LeftTag || this.Evulator.DecodeAmpCode && cur == '&')
                    {
                        this.pos = i - 1;
                        if (this.Evulator.TrimStartEnd)
                        {
                            return text.ToString().Trim();
                        }
                        return text.ToString();
                    }
                }
                if (parfound)
                {
                    nparsetext.Append(cur);
                }
                else
                {
                    if (this.Evulator.TrimMultipleSpaces)
                    {
                        if (cur == ' ' && next == ' ') continue;
                    }
                    text.Append(cur);
                }
            }
            this.pos = this.TextLength;

            if (this.Evulator.TrimStartEnd)
            {
                return text.ToString().Trim();
            }
            return text.ToString();
        }








    }
}

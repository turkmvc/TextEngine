using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class IncludeEvulator : BaseEvulator
    {
        private TextEvulateResult Render_Parse(TextElement tag, object vars)
        {
            var loc = tag.GetAttribute("name");
            loc = this.EvulateText(loc)?.ToString();
            if (!this.ConditionSuccess(tag, "if") || !File.Exists(loc)) return null;
            string xpath = tag.GetAttribute("xpath");
            bool xpathold = false;
            if (string.IsNullOrEmpty(xpath))
            {
                xpath = tag.GetAttribute("xpath_old");
                xpathold = true;
            }

            var content = File.ReadAllText(loc);
            var result = new TextEvulateResult();

            result.Result = TextEvulateResultEnum.EVULATE_NOACTION;
            tag.Parent.SubElements.Remove(tag);        
            if(string.IsNullOrEmpty(xpath))
            {
                this.Evulator.Parse(tag.Parent, content);
            }
            else
            {
                var tempitem = new TextElement();
                tempitem.ElemName = "#document";
                this.Evulator.Parse(tempitem, content);
                TextElements elems = null;
                if (!xpathold)
                {
                    elems = tempitem.FindByXPath(xpath);
                }
                else
                {
                    elems = tempitem.FindByXPathOld(xpath);
                }
                for (int i = 0; i < elems.Count; i++)
                {
                    elems[i].Parent = tag.Parent;
                    tag.Parent.SubElements.Add(@elems[i]);
                }
            }
            return result;
        }
        private TextEvulateResult Render_Default(TextElement tag, object vars)
        {
            var loc = tag.GetAttribute("name");
            loc = this.EvulateText(loc)?.ToString();
            var parse = tag.GetAttribute("parse", "true");
            if (!File.Exists(loc) || !this.ConditionSuccess(tag, "if")) return null;
            var content = File.ReadAllText(loc);
            var result = new TextEvulateResult();

            if (parse == "false")
            {
                result.Result = TextEvulateResultEnum.EVULATE_TEXT;
                result.TextContent = content;
            }
            else
            {
                var tempelem = new TextElement
                {
                    ElemName = "#document",
                    BaseEvulator = this.Evulator
                };
                var tempelem2 = new TextElement
                {
                    ElemName = "#document",
                    BaseEvulator = this.Evulator
                };

                string xpath = tag.GetAttribute("xpath");
                bool xpathold = false;
                if (string.IsNullOrEmpty(xpath))
                {
                    xpath = tag.GetAttribute("xpath_old");
                    xpathold = true;
                }
      
                this.Evulator.Parse(tempelem2, content);
                if (string.IsNullOrEmpty(xpath))
                {
                    tempelem = tempelem2;
                }
                else
                {
                    TextElements elems = null;
                    if (!xpathold)
                    {
                        elems = tempelem2.FindByXPath(xpath);
                    }
                    else
                    {
                        elems = tempelem2.FindByXPathOld(xpath);
                    }
                    for (int i = 0; i < elems.Count; i++)
                    {
                        elems[i].Parent = tempelem;
                        tempelem.SubElements.Add(elems[i]);
                    }
                }
                var cresult = tempelem.EvulateValue(0, 0, vars);
                result.TextContent += cresult.TextContent;
                if (cresult.Result == TextEvulateResultEnum.EVULATE_RETURN)
                {
                    result.Result = TextEvulateResultEnum.EVULATE_RETURN;
                    return result;
                }
                result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            }
            return result;
        }
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            if (Evulator.IsParseMode)
            {
                return this.Render_Parse(tag, vars);
            }
            return this.Render_Default(tag, vars);

        }
    }
}

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
            if (!this.ConditionSuccess(tag, "if")) return null;
            if (!File.Exists(loc)) return null;
            var content = File.ReadAllText(loc);
            var result = new TextEvulateResult();
            this.Evulator.Parse(tag.Parent, content);
            result.Result = TextEvulateResultEnum.EVULATE_NOACTION;
            tag.Parent.SubElements.Remove(tag);        
            return result;
        }
        private TextEvulateResult Render_Default(TextElement tag, object vars)
        {
            var loc = tag.GetAttribute("name");
            var parse = tag.GetAttribute("parse", "true");
            if (!File.Exists(loc)) return null;
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
                this.Evulator.Parse(tempelem, content);
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

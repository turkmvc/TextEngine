using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class SwitchEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var result = new TextEvulateResult();
            var condition = tag.GetAttribute("c");
            var value = this.EvulateText(condition);
            TextElement @default = null;
            TextElement active = null;
            for (int i = 0; i < tag.SubElementsCount; i++)
            {
                var elem = tag.SubElements[i];
                if (elem.ElemName == "default")
                {
                    @default = elem;
                    continue;
                }
                else if (elem.ElemName != "case")
                {
                    continue;
                }
                if (this.EvulateCase(elem, value?.ToString()))
                {
                    active = elem;
                    break;
                }
            }
            if (active == null) active = @default;
            if (active == null) return result;
            var cresult = active.EvulateValue(0, 0, vars);
            result.TextContent += cresult.TextContent;
            if (cresult.Result == TextEvulateResultEnum.EVULATE_RETURN)
            {
                result.Result = TextEvulateResultEnum.EVULATE_RETURN;
                return result;
            }
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            return result;
        }
        protected bool EvulateCase(TextElement tag, string value)
        {
            var tagvalue = tag.GetAttribute("v");
            return tagvalue.Split('|').Contains(value);
        }
    }
}

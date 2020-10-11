using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class ParamEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var result = new TextEvulateResult();
            if (tag.ElementType != TextElementType.Parameter)
		    {
                result.Result = TextEvulateResultEnum.EVULATE_NOACTION;
                return result;
            }
            result.TextContent += this.EvulateText(tag.ElemName, vars);
            return result;
        }
    }
}

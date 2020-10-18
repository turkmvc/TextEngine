using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class ForeachEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var varname = tag.GetAttribute("var");
            var @in = tag.GetAttribute("in");
            if (string.IsNullOrEmpty(varname) || @in == null)
		    {
                return null;
            }
            var inlist = this.EvulateText(@in);
            if (inlist == null || !(inlist is IEnumerable list)) return null;
            var svar = new KeyValues<object>();
            this.Evulator.LocalVariables.Add(svar);
            var result = new TextEvulateResult();
            foreach (var item in list)
            {
                svar[varname] = item;
                var cresult = tag.EvulateValue(0, 0, vars);
                if (cresult == null) continue;
                result.TextContent += cresult.TextContent;
                if (cresult.Result == TextEvulateResultEnum.EVULATE_RETURN)
                {
                    result.Result = TextEvulateResultEnum.EVULATE_RETURN;
                    this.Evulator.LocalVariables.Remove(svar);
                    return result;
                }
                else if (cresult.Result == TextEvulateResultEnum.EVULATE_BREAK)
                {
                    break;
                }
            }
            this.Evulator.LocalVariables.Remove(svar);
            result.Result = TextEvulateResultEnum.EVULATE_TEXT;
            return result;
        }
    }
}

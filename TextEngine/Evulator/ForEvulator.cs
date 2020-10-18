using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class ForEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var varname = tag.GetAttribute("var");
            var start = tag.GetAttribute("start");
            var step = tag.GetAttribute("step");
            if (string.IsNullOrEmpty(start))
            {
                start = "0";
            }
            if (step == null || step == "0")
            {
                step = "1";
            }
            var to = tag.GetAttribute("to");
            if (string.IsNullOrEmpty(varname) && string.IsNullOrEmpty(step) && string.IsNullOrEmpty(to))
            {
                return null;
            }

            var startres = this.EvulateText(start);
            var stepres = this.EvulateText(step);
            int startnum = 0;
            int stepnum = 0;
            int tonum = 0;
            if (!TypeUtil.IsNumericType(startres))
            {
                stepnum = 1;
            }
            else
            {
                stepnum = (int) Convert.ChangeType(stepres, TypeCode.Int32);
            }
            if(TypeUtil.IsNumericType(startres))
            {
                startnum = (int)Convert.ChangeType(startres, TypeCode.Int32); ;
            }
            var tores = this.EvulateText(to);
            if (!TypeUtil.IsNumericType(tores))
		    {
                return null;
            }
            tonum = (int)Convert.ChangeType(tores, TypeCode.Int32);
		    var result = new TextEvulateResult();
            var svar = new KeyValues<object>();
            this.Evulator.LocalVariables.Add(svar);
            for (int i = startnum; i < tonum; i += stepnum)
		    {
                svar[varname] = i;
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

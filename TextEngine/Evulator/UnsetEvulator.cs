using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class UnsetEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            bool conditionok = this.ConditionSuccess(tag, "if");
            var result = new TextEvulateResult
            {
                Result = TextEvulateResultEnum.EVULATE_NOACTION
            };

            if (conditionok)
            {
                string defname = tag.GetAttribute("name");
                if (string.IsNullOrEmpty(defname)) return result;
                this.Evulator.DefineParameters.Delete(defname);
            }
            return result;
        }
    }
}

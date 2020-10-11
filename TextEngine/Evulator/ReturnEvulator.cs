using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class ReturnEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
		    var cr = this.ConditionSuccess(tag, "if");
            if (!cr) return null;
            var result = new TextEvulateResult
            {
                Result = TextEvulateResultEnum.EVULATE_RETURN
            };
            return result;
        }
    }
}

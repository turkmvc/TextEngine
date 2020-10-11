using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class GeneralEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var result = new TextEvulateResult();
            result.Result = TextEvulateResultEnum.EVULATE_DEPTHSCAN;
            return result;
        }
    }
}

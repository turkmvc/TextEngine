using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class BreakEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            var result = new TextEvulateResult
            {
                Result = TextEvulateResultEnum.EVULATE_BREAK
            };
            return result;
        }
    }
}

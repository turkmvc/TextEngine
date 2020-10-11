using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class NoPrintEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            tag.EvulateValue(0, 0, vars);
            return null;
        }
    }
}

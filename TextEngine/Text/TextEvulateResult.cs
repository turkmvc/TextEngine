using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public enum TextEvulateResultEnum
    {
        EVULATE_NOACTION = 0,
        EVULATE_TEXT = 1,
        EVULATE_CONTINUE = 2,
        EVULATE_RETURN = 3,
        EVULATE_DEPTHSCAN = 4,
        EVULATE_BREAK = 5
    }
    public class TextEvulateResult
    {
        public string TextContent { get; set; }
	    public TextEvulateResultEnum Result { get; set; }
	    public int Start { get; set; }
	    public int End { get; set; }
        public TextEvulateResult()
        {
            this.Result = TextEvulateResultEnum.EVULATE_TEXT;
        }
    }
}

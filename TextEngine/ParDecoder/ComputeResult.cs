using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.ParDecoder
{
    public class ComputeResult
    {
	    public NamedObjects Result { get; set; }
	    public ComputeResult()
        {
            this.Result = new NamedObjects();
        }
    }
}

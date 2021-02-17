using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Misc;
using TextEngine.ParDecoder;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public abstract class BaseEvulator
    {
        protected TextEvulator Evulator { get; set; }
        public BaseEvulator()
        {
          
        }
        public abstract TextEvulateResult Render(TextElement tag, object vars);
        protected object EvulateText(string text, object additionalparams = null)
        {
		    var pardecoder = new ParDecode(text);
		    pardecoder.Decode();
            var addpar = additionalparams as KeyValues<object>;
            if (addpar != null)
            {
                this.Evulator.LocalVariables.Add(addpar);
            }
		    var er =  pardecoder.Items.Compute(this.Evulator.GloblaParameters, null, this.Evulator.LocalVariables);
            if(addpar != null)
            {
                this.Evulator.LocalVariables.Remove(addpar);
            }
            return er.Result.First();
        }
        public void SetEvulator(TextEvulator evulator)
        {
            this.Evulator = evulator;
        }
        protected bool ConditionSuccess(TextElement tag, string attr = "c")
        {
		    var condition = (tag.NoAttrib) ? tag.Value : tag.GetAttribute(attr);
            if (condition == null) return true;
		    var res = this.EvulateText(condition);
            if(res is bool b)
            {
                return b;
            }
            return false;
        }
    }
}

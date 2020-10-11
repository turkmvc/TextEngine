using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Macros;
using TextEngine.Misc;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public  class CallMacroEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            if (string.IsNullOrEmpty(tag.ElemAttr.FirstAttribute?.Name)) return null;
            var name = tag.ElemAttr.FirstAttribute.Name;
		    var cr = this.ConditionSuccess(tag, "if");
            if (!cr) return null;
            if (string.IsNullOrEmpty(name)) return null;
		    var element = this.GetMacroElement(name);
            if (element != null)
		    {
                var newelement = new KeyValues<object>();
                for (int i = 0; i < element.ElemAttr.Count; i++)
                {
                    if (element.ElemAttr[i].Name == "name") continue;
                    newelement[element.ElemAttr[i].Name] = this.EvulateText(element.ElemAttr[i].Value, vars);
                }
                for (int i = 1; i < tag.ElemAttr.Count; i++)
                {
                    var key = tag.ElemAttr[i];
                    newelement[key.Name] = this.EvulateText(key.Value, vars);
                }
                var result = element.EvulateValue(0, 0, newelement);
                return result;
            }
            return null;
        }
        protected TextElement GetMacroElement(string name)
        {

            return this.Evulator.SavedMacrosList.GetMacro(name); ;
        }
    }
}

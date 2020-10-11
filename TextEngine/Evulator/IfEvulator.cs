using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Evulator
{
    public class IfEvulator : BaseEvulator
    {
        public override TextEvulateResult Render(TextElement tag, object vars)
        {
            if(this.Evulator.IsParseMode)
            {
                return Render_ParseMode(tag, vars);
            }
            return RenderDefault(tag, vars);
        }
        public  TextEvulateResult Render_ParseMode(TextElement tag, object vars)
        {
            var result = new TextEvulateResult();
            bool conditionok = this.ConditionSuccess(tag);
            bool sil = false;
            for (int i = 0; i < tag.SubElementsCount; i++)
            {
                var sub = tag.SubElements[i];
                if (!conditionok || sil)
                {
                    if(!sil)
                    {
                        if (sub.ElemName == "else")
                        {
                            conditionok = true;
                        }
                        else if (sub.ElemName == "elif")
                        {
                            conditionok = this.ConditionSuccess(sub);
                        }
                    }

                    tag.SubElements.RemoveAt(i);
                    i--;
                    continue;
                }
                else
                {
                    if(sub.ElemName == "else" || sub.ElemName == "elif")
                    {
                        sil = true;
                        i--;
                        continue;
                    }
                    //sub.EvulateValue(0, 0, vars);
                    sub.Index = tag.Parent.SubElements.Count;
                    sub.Parent = tag.Parent;
                    tag.Parent.SubElements.Add(sub);
                }
            }
            tag.Parent.SubElements.Remove(tag);
            result.Result = TextEvulateResultEnum.EVULATE_NOACTION;
            return result;
        }
        public  TextEvulateResult RenderDefault(TextElement tag, object vars)
        {
            var result = new TextEvulateResult();
            if (this.ConditionSuccess(tag))
            {
          
                var elseitem = tag.GetSubElement("elif", "else");
                if (elseitem != null)
                {
                    result.End = elseitem.Index;
                }
                result.Result = TextEvulateResultEnum.EVULATE_DEPTHSCAN;
            }
            else
            {
                var elseitem = tag.GetSubElement("elif", "else");
                while (elseitem != null)
                {
                    if (elseitem.ElemName == "else")
                    {
                        result.Start = elseitem.Index + 1;
                        result.Result = TextEvulateResultEnum.EVULATE_DEPTHSCAN;
                        return result;
                    }
                    else
                    {

                        if (this.ConditionSuccess(elseitem))
                        {
                            result.Start = elseitem.Index + 1;
                            var nextelse = elseitem.NextElementWN("elif", "else");
                            if (nextelse != null)
                            {
                                result.End = nextelse.Index;
                            }
                            result.Result = TextEvulateResultEnum.EVULATE_DEPTHSCAN;
                            return result;
                        }
                    }
                    elseitem = elseitem.NextElementWN("elif", "else");
                }
                if (elseitem == null)
                {
                    return result;
                }
            }
            return result;
        }
    }
}

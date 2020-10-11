using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextEngine.Text;

namespace TextEngine.Macros
{
    public class SavedMacros
    {
        private List<TextElement> macros = new List<TextElement>();
        private int GetMacroIndex(string name)
        {
            for (int i = 0; i < macros.Count; i++)
            {
                if (macros[i].GetAttribute("name") == name) return i;
            }
            return -1;
        }
        public TextElement GetMacro(string name)
        {
            var index = GetMacroIndex(name);
            if (index == -1) return null;
            return macros[index];
        }
        public void SetMacro(string name, TextElement tag)
        {
            var index = GetMacroIndex(name);
            if(index == -1)
            {
                macros.Add(tag);
            }
            else
            {
                macros[index] = tag;
            }
        }

    }
}

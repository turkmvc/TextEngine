using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Text
{
    public class TextElementAttribute
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string Value { get; set; }
        public override string ToString()
        {
            return $"{Name}=\"{Value}\"";
        }
    }
}

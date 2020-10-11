using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TextEngine.Text;

namespace TextEngine.XPathClasses
{
    public class XPathFunctions
    {
        public TextElement BaseItem { get; set; }
        public int TotalItems { get; set; }
        public int ItemPosition { get; set; }
        [XPathFunc(Name = "contains")]
        public bool Contains(string x, string y)
        {
            return x.Contains(y);
        }
        [XPathFunc(Name = "lower-case")]
        public string LowerCase(string x)
        {
            return x.ToLower();
        }
        [XPathFunc(Name = "upper-case")]
        public string UpperCase(string x)
        {
            return x.ToUpper();
        }
        [XPathFunc(Name = "text")]
        public string Text()
        {
            return BaseItem.InnerText();
        }
        [XPathFunc(Name = "starts-with")]
        public bool StartsWith(string x, string y)
        {
            return x.StartsWith(y);
        }
        [XPathFunc(Name = "ends-with")]

        public bool EndsWith(string x, string y)
        {
            return x.EndsWith(y);
        }
        [XPathFunc(Name = "position")]

        public int Position()
        {
            return this.BaseItem.Index;
        }
        [XPathFunc(Name = "last")]

        public int Last()
        {
            return TotalItems;
        }
        public MethodInfo GetMetohdByName(string callname)
        {
            var etype = this.GetType();
            var methods = etype.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                var functattrib = method.GetCustomAttribute(typeof(XPathFuncAttribute)) as XPathFuncAttribute;
                string mname = method.Name;
                if(functattrib != null)
                {
                    if (!string.IsNullOrEmpty(functattrib.Name)) mname = functattrib.Name;
                }
                if (mname == callname) return method;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TextEngine.Evulator;
using TextEngine.Macros;
using TextEngine.Misc;

namespace TextEngine.Text
{
    public class TextEvulator
    {
        public string Text { get; set; }
        private TextElement elements;

        public TextElement Elements
        {
            get { return elements; }
            set { elements = value; }
        }
        public bool ThrowExceptionIFPrevIsNull { get; set; }
        private int Depth { get; set; } = 0;
        public char LeftTag { get; set; } = '{';
        public char RightTag { get; set; } = '}';
        public string NoParseTag { get; set; } = "noparse";
        public bool NoParseEnabled { get; set; } = true;
        public char ParamChar { get; set; } = '%';
        public Dictionary<string, object> Aliasses { get; private set; }
        public List<string> AutoClosedTags { get; private set; }
        public object GloblaParameters { get; set; }
        public KeyValues<object> DefineParameters { get; set; }

        public KeyValueGroup LocalVariables { get; private set; }
        public bool ParamNoAttrib { get; set; }
        public bool DecodeAmpCode { get; set; }
        public bool AllowParseCondition { get; set; }
        public EvulatorTypes EvulatorTypes { get; private set; }
        public bool SupportExclamationTag { get; set; }
        public bool SupportCDATA { get; set; }
        public bool AllowXMLTag { get; set; }
        public bool TrimMultipleSpaces { get; set; }
        public bool TrimStartEnd { get; set; }
        public Dictionary<string, string> AmpMaps { get; private set; }
        public List<string> ConditionalTags { get; private set; }
        public List<string> NoAttributedTags { get; private set; }
        public SavedMacros SavedMacrosList { get; private set; }
        private bool isParseMode;
        public bool IsParseMode
        {
            get
            {
                return isParseMode;
            }
            set
            {
                isParseMode = value;
            }
        }
        public void ApplyXMLSettings()
        {
            this.SupportCDATA = true;
            this.SupportExclamationTag = true;
            this.LeftTag = '<';
            this.RightTag = '>';
            this.AllowXMLTag = true;
            this.TrimStartEnd = true;
            this.NoParseEnabled = false;
            this.DecodeAmpCode = true;
            this.TrimMultipleSpaces = true;

        }
        public TextEvulator(string text = null, bool isfile = false)
        {
            this.DefineParameters = new KeyValues<object>();
            this.LocalVariables = new KeyValueGroup();
            this.LocalVariables.Add(this.DefineParameters);
            this.ThrowExceptionIFPrevIsNull = true;
            var comparer = StringComparer.OrdinalIgnoreCase;
            this.SavedMacrosList = new SavedMacros();

            this.EvulatorTypes = new EvulatorTypes();
            this.AmpMaps = new Dictionary<string, string>();
            this.ConditionalTags = new List<string>();
            this.Aliasses = new Dictionary<string, object>(comparer);
            this.Elements = new TextElement()
            {
                ElemName = "#document",
                ElementType = TextElementType.Document
            };
            if (isfile)
            {
                this.Text = System.IO.File.ReadAllText(text);
            }
            else
            {
                this.Text = text;
            }

            this.InitNoAttributedTags();
            this.InitEvulator();
            this.InitAutoClosed();
            this.InitAmpMaps();
            this.InitConditionalTags();
        }
        public void OnTagClosed(TextElement element)
        {
            if (!this.AllowParseCondition || !this.IsParseMode || !this.ConditionalTags.Contains(element.ElemName)) return;
            element.Parent.EvulateValue(element.Index, element.Index + 1);
        }
        private void InitNoAttributedTags()
        {
            this.NoAttributedTags = new List<string>();
            this.NoAttributedTags.Add("if");
        }
        private void InitConditionalTags()
        {
            this.ConditionalTags.Add("if");
            this.ConditionalTags.Add("include");
            this.ConditionalTags.Add("set");
            this.ConditionalTags.Add("unset");
        }
        private void InitAutoClosed()
        {

            //Otomatik kapatılacak taglar.
            AutoClosedTags = new List<string>()
           {
              "elif", "else", "return", "break", "continue", "include", "cm", "set", "unset"
           };
        }
        private void InitEvulator()
        {
            this.EvulatorTypes.Param = typeof(ParamEvulator);
            this.EvulatorTypes.GeneralType = typeof(GeneralEvulator);
            this.EvulatorTypes["if"] = typeof(IfEvulator);
            this.EvulatorTypes["for"] = typeof(ForEvulator);
            this.EvulatorTypes["foreach"] = typeof(ForeachEvulator);
            this.EvulatorTypes["switch"] = typeof(SwitchEvulator);
            this.EvulatorTypes["return"] = typeof(ReturnEvulator);
            this.EvulatorTypes["break"] = typeof(BreakEvulator);
            this.EvulatorTypes["continue"] = typeof(ContinueEvulator);
            this.EvulatorTypes["cm"] = typeof(CallMacroEvulator);
            this.EvulatorTypes["macro"] = typeof(MacroEvulator);
            this.EvulatorTypes["noprint"] = typeof(NoPrintEvulator);
            this.EvulatorTypes["repeat"] = typeof(RepeatEvulator);
            this.EvulatorTypes["set"] = typeof(SetEvulator);
            this.EvulatorTypes["unset"] = typeof(UnsetEvulator);
            this.EvulatorTypes["include"] = typeof(IncludeEvulator);
        }

        private void InitAmpMaps()
        {
            this.AmpMaps["nbsp"] = " ";
            this.AmpMaps["amp"] = "&";
            this.AmpMaps["quot"] = "\"";
            this.AmpMaps["lt"] = "<";
            this.AmpMaps["gt"] = ">";
        }
        public void Parse()
        {
            var parser = new TextEvulatorParser(this);
            parser.Parse(this.Elements, this.Text);
        }
        public void Parse(TextElement baselement, string text)
        {
            var parser = new TextEvulatorParser(this);
            parser.Parse(baselement, text);
        }

    }
}

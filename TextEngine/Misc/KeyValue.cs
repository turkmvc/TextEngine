using System;
using System.Collections.Generic;
using System.Text;

namespace TextEngine.Misc
{

    public class KeyValue<TObject>
    {
        public string Name { get; set; }
        public TObject Value { get; set; }
    }
}

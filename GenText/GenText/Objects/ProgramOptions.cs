using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenText
{
    public class ProgramOptions
    {
        public int MaxShortDescriptionTextLength { get; set; }
        public bool BasicFieldsSameAsAdvanced { get; set; }
        public string SelectedTemplate { get; set; } = "";
        public string ItemTypesString { get; set; } = "";
        public string SelectedItemType { get; set; } = "";
        public string LastSaveLocation { get; set; } = "";
        public string DefaultTermsPathP1 { get; set; } = "";
        public string DefaultTermsPathP2 { get; set; } = "";
        public string Delimiter { get; set; } = "";
    }
}

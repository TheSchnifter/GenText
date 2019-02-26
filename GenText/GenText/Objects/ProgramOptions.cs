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

        private string _selectedTemplate = "";
        public string SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { _selectedTemplate = value; }
        }

        private string _itemTypesString = "";
        public string ItemTypesString
        {
            get { return _itemTypesString; }
            set { _itemTypesString = value; }
        }

        private string _selectedItemType = "";
        public string SelectedItemType
        {
            get { return _selectedItemType; }
            set { _selectedItemType = value; }
        }

        private string _lastSaveLocation = "";
        public string LastSaveLocation { get => _lastSaveLocation; set => _lastSaveLocation = value; }

        private string _defaultTermsPathP1 = "";
        public string DefaultTermsPathP1 { get => _defaultTermsPathP1; set => _defaultTermsPathP1 = value; }

        private string _defaultTermsPathP2 = "";
        public string DefaultTermsPathP2 { get => _defaultTermsPathP2; set => _defaultTermsPathP2 = value; }


    }
}

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

        private string _defaultItemOutPath = "";
        public string DefaultItemOutPath { get => _defaultItemOutPath; set => _defaultItemOutPath = value; }

        private string _defaultTermsPath = "";
        public string DefaultTermsPath { get => _defaultTermsPath; set => _defaultTermsPath = value; }


    }
}

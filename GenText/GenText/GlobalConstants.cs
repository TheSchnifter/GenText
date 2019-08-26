using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenText
{
    public static class GlobalConstants
    {
        public static string Version = "1.0.1";
        public static string OptionsFilePath = "options.txt";
        public static string DelimiterFilePath = "delimiter.txt";
        public static string TemplatesPath = "Templates";
        public static string DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static char Delimiter = AppService.GetDelimiter();
    }
}

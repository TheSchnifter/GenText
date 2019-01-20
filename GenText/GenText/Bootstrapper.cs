using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenText
{
    public static class Bootstrapper
    {
        public static void InitApplication()
        {
            //init options file
            GlobalFunctions.CreateFileIfDoesntExist(GlobalConstants.OptionsFilePath);

            //init templates directory
            if (!Directory.Exists(GlobalConstants.TemplatesPath))
            {
                Directory.CreateDirectory(GlobalConstants.TemplatesPath);
                GlobalFunctions.LogLine("Created templates directory");
            }
        }
    }
}

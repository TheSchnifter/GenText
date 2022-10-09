using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace GenText
{
    public static class AppService
    {
        public static void InitApplication()
        {
            //init options file
            FileIoService.CreateFileIfDoesntExist(GlobalConstants.OptionsFilePath);
            
            //init templates directory
            if (!Directory.Exists(GlobalConstants.TemplatesPath))
            {
                Directory.CreateDirectory(GlobalConstants.TemplatesPath);
                LogLine("Created templates directory");
            }
        }

        /// <summary>
        /// calls the main window to add line to log list box
        /// </summary>
        /// <param name="line"></param>
        public static void LogLine(string line)
        {
            ((MainWindow)System.Windows.Application.Current.Windows[0]).LogLine(line);
        }

        /// <summary>
        /// The item has been updated somewhere else in the app. Send it up to the main window
        /// </summary>
        /// <param name="item"></param>
        public static void RefreshItem(Object item, string path)
        {
            ((MainWindow)System.Windows.Application.Current.Windows[0]).LoadItem(item, path);
        }

        /// <summary>
        /// Options changed somewhere. Update the in memory options in MainWindow
        /// </summary>
        public static void RefreshMainWindowOptions()
        {
            ((MainWindow)System.Windows.Application.Current.Windows[0]).RefreshOptions();
        }

        public static ProgramOptions GetProgramOptions()
        {
            var opts = new ProgramOptions();
            return (ProgramOptions)FileIoService.LoadObjectFromFile(opts, GlobalConstants.OptionsFilePath, true);
        }

        public static List<string> LoadTemplateList()
        {
            return Directory.EnumerateFiles(GlobalConstants.TemplatesPath).ToList();
        }

        public static void SaveProgramOptions(ProgramOptions opts)
        {
            FileIoService.SaveObjectToFile(opts, GlobalConstants.OptionsFilePath);
        }

        public static List<string> GenerateFromTemplate(ProgramOptions opts, Object item)
        {
            var templateLines = FileIoService.GetStringCollectionFromFile(opts.SelectedTemplate);
            var itemProps = item.GetType().GetProperties();
            var newLines = new List<string>();

            foreach (string line in templateLines)
            {
                if (line.Contains("{"))
                {
                    var firstPos = line.IndexOf('{');
                    var lastPos = line.IndexOf('}');
                    var propToReplace = line.Substring(firstPos + 1, lastPos - firstPos - 1);
                    var bracketedProp = $"{{{propToReplace}}}";

                    var itemProp = itemProps.FirstOrDefault(x => x.Name.Equals(propToReplace));

                    if (itemProp != null)
                    {
                        newLines.Add(line.Replace(bracketedProp, itemProp.GetValue(item).ToString()).Trim());
                    }
                    else if (itemProp == null && propToReplace.ToUpper().Contains("TERMSP1"))
                    {
                        newLines.Add(line.Replace(bracketedProp, GetTermsP1(opts)));
                    }
                    else if (itemProp == null && propToReplace.ToUpper().Contains("TERMSP2"))
                    {
                        newLines.Add(line.Replace(bracketedProp, GetTermsP2(opts)));
                    }
                    else if (itemProp == null && propToReplace.ToUpper().Equals("TABLEROWS") && item.GetType() == typeof(MultiPropertyItem))
                    {
                        var realItem = (MultiPropertyItem)item;
                        var tableRows = realItem.ItemDetails.ToHtmlTableRows();
                        newLines.Add(line.Replace(bracketedProp, tableRows));
                    }
                    else
                    {
                        LogLine($"Item {item.GetType().ToString()} does not contain property {propToReplace}");
                        newLines.Add(@"<div style='display:none;'>Error replacing property " + propToReplace + "</div>");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    newLines.Add(line.Trim());
                }
            }

            return newLines;

        }

        public static string GetTermsP1(ProgramOptions opts)
        {
            var termLines = FileIoService.GetStringCollectionFromFile(opts.DefaultTermsPathP1);
            var sb = new StringBuilder();
            foreach (string line in termLines)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        public static string GetTermsP2(ProgramOptions opts)
        {
            var termLines = FileIoService.GetStringCollectionFromFile(opts.DefaultTermsPathP2);
            var sb = new StringBuilder();
            foreach (string line in termLines)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        public static string ShowSaveDialog(ProgramOptions opts, string defaultExt, string name)
        {
            var dialog = new SaveFileDialog()
            {
                DefaultExt = defaultExt,
                InitialDirectory = string.IsNullOrWhiteSpace(opts.LastSaveLocation) ? GlobalConstants.DefaultPath : opts.LastSaveLocation,
                FileName = name
            };
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                UpdateLastSaveLocation(dialog.FileName);
                return dialog.FileName;
            }
            else
            {
                return null;
            }

        }

        public static string ShowOpenFileDialog(ProgramOptions opts, string filter)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = filter,
                Multiselect = false,
                InitialDirectory = string.IsNullOrWhiteSpace(opts.LastSaveLocation) ? GlobalConstants.DefaultPath : opts.LastSaveLocation
            };

            var result = dialog.ShowDialog();

            return result.HasValue && result.Value ? dialog.FileName : null;
        }

        public static void UpdateLastSaveLocation(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (path.Last() != '\\')
                {
                    path = path.Substring(0, path.LastIndexOf('\\') + 1);
                }

                ProgramOptions opts = GetProgramOptions();
                opts.LastSaveLocation = path;
                SaveProgramOptions(opts);
            }
        }

        public static char GetDelimiter()
        {
            var fileLines = new List<string>();
            var fileExisted = FileIoService.CreateFileIfDoesntExist(GlobalConstants.DelimiterFilePath);

            if (fileExisted)
            {
                fileLines = FileIoService.GetStringCollectionFromFile(GlobalConstants.DelimiterFilePath);
            }

            //if the delimiter file does not exist, create it and add default delimiter
            if (fileLines.Count == 0)
            {
                FileIoService.SaveSingleLineToFile("|", GlobalConstants.DelimiterFilePath);
                LogLine("Delimiter file was created with default value |. To change, go to program directory and update character in delimiter.txt and restart application");
                return GetDelimiter();
            }
            else
                return fileLines.First().First();

        }

    }
}

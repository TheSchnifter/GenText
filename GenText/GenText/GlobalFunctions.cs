﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace GenText
{
    public static class GlobalFunctions
    {
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
        /// creates a new blank file if doesn't exist, else do nothing
        /// </summary>
        /// <param name="path"></param>
        public static void CreateFileIfDoesntExist(string path)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "");
                LogLine($"File {path} not found. Creating new file");
            }
        }

        /// <summary>
        /// overwrites contents of file empty for writing. creates file if doesn't exist
        /// </summary>
        /// <param name="path"></param>
        private static void EraseFile(string path)
        {
            try
            {
                System.IO.File.WriteAllText(path, "");
            }
            catch (Exception e)
            {
                LogLine(e.Message);
            }
        }

        public static Object LoadObjectFromFile(Object objType, string path, bool createIfNotFound)
        {
            if (createIfNotFound)
                CreateFileIfDoesntExist(path);

            var fileLineValues = new List<KeyValuePair<string, string>>();
            try
            {
                var reader = new StreamReader(path);
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    var splitLine = line.Split(',');
                    fileLineValues.Add(new KeyValuePair<string, string>(splitLine[0], splitLine[1]));
                }

                reader.Close();
            }
            catch (FileNotFoundException ex)
            {
                LogLine(ex.Message);
            }
            catch (Exception ex)
            {
                LogLine(ex.Message);
            }

            var obj = Activator.CreateInstance(objType.GetType());

            foreach (var prop in obj.GetType().GetProperties())
            {
                var option = fileLineValues.FirstOrDefault(x => x.Key == prop.Name);

                try
                {
                    if (option.Key != null)
                        prop.SetValue(obj, Convert.ChangeType(option.Value, prop.PropertyType));
                }
                catch (Exception e)
                {
                    LogLine(e.Message);
                    return Activator.CreateInstance(objType.GetType());
                }
            }

            return obj;
        }

        /// <summary>
        /// saves an object's properties to the specified path as key value pairs. overwrites existing file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public static void SaveObjectToFile(Object obj, string path)
        {
            var props = new List<KeyValuePair<string, string>>();

            foreach (var prop in obj.GetType().GetProperties())
            {
                props.Add(new KeyValuePair<string, string>(prop.Name, prop.GetValue(obj).ToString()));
            }

            EraseFile(path);

            try
            {
                var writer = new StreamWriter(path);

                foreach (KeyValuePair<string, string> prop in props)
                {
                    writer.WriteLine(prop.Key + "," + prop.Value);
                }

                writer.Close();
            }
            catch (Exception e)
            {
                LogLine(e.Message);
            }
        }

        public static void SaveSingleLineToFile(string html, string path)
        {
            EraseFile(path);
            try
            {
                var writer = new StreamWriter(path);
                writer.WriteLine(html);
                writer.Close();
            }
            catch (Exception e)
            {
                LogLine(e.Message);
            }
        }

        public static void SaveLineCollectionToFile(List<string> lines, string path)
        {
            EraseFile(path);
            try
            {
                var writer = new StreamWriter(path);

                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                LogLine(e.Message);
            }
        }

        public static ProgramOptions GetProgramOptions()
        {
            var opts = new ProgramOptions();
            return (ProgramOptions)LoadObjectFromFile(opts, GlobalConstants.OptionsFilePath, true);
        }

        public static void SaveProgramOptions(ProgramOptions opts)
        {
            SaveObjectToFile(opts, GlobalConstants.OptionsFilePath);
        }

        public static List<string> LoadTemplateList()
        {
            return Directory.EnumerateFiles(GlobalConstants.TemplatesPath).ToList();
        }

        public static List<string> GenerateFromTemplate(ProgramOptions opts, Object item)
        {
            var templateLines = GetTemplate(opts.SelectedTemplate);
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

                    if (itemProp == null)
                    {
                        LogLine($"Could not update {propToReplace}. Item does not contain property");
                    }
                    else
                    {
                        newLines.Add(line.Replace(bracketedProp, itemProp.GetValue(item).ToString()).Trim());
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    newLines.Add(line.Trim());
                }
            }

            return newLines;

        }

        public static List<string> GetTemplate(string path)
        {
            var lines = new List<string>();

            try
            {
                var reader = new StreamReader(path);
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                reader.Close();
            }
            catch (FileNotFoundException ex)
            {
                LogLine(ex.Message);
            }
            catch (Exception ex)
            {
                LogLine(ex.Message);
            }

            return lines;
        }

        public static string ShowSaveDialog(ProgramOptions opts, string defaultExt)
        {
            var dialog = new SaveFileDialog()
            {
                DefaultExt = defaultExt,
                InitialDirectory = string.IsNullOrWhiteSpace(opts.DefaultItemOutPath) ? GlobalConstants.DefaultPath : opts.DefaultItemOutPath
            };

            dialog.ShowDialog();

            return dialog.FileName;
        }

    }
}
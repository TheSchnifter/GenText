using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace GenText
{
    public static class FileIoService
    {

        /// <summary>
        /// creates a new blank file if doesn't exist, else do nothing
        /// </summary>
        /// <param name="path"></param>
        public static bool CreateFileIfDoesntExist(string path)
        {
            bool fileExisted = false;

            if (!File.Exists(path))
            {
                File.WriteAllText(path, "");
                AppService.LogLine($"File {path} not found. Creating new file");
            }
            else
            {
                fileExisted = true;
            }

            return fileExisted;
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
                AppService.LogLine(e.Message);
            }
        }

        public static Object LoadObjectFromFile(Object objType, string path, bool createIfNotFound)
        {
            if (createIfNotFound)
                CreateFileIfDoesntExist(path);

            var reader = new StreamReader(path);
            bool assumeJson = true;
            var obj = Activator.CreateInstance(objType.GetType());

            var fileLineValues = new List<KeyValuePair<string, string>>();
            try
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (assumeJson)
                    {
                        //assume it's json on the first line, but if it's not stop trying and parse it the old way
                        //this should support loading the old file type but then it will always save as json moving forward
                        try
                        {
                            obj = JsonConvert.DeserializeObject(line, objType.GetType());
                        }
                        catch (Exception ex)
                        {
                            //it's not json
                            obj = null;
                        }

                        if (obj != null)
                        {
                            reader.Close();
                            return obj;
                        }
                        else
                            assumeJson = false;
                    }

                    var splitLine = line.Split(GlobalConstants.Delimiter);
                    fileLineValues.Add(new KeyValuePair<string, string>(splitLine[0], splitLine[1]));
                }

                reader.Close();
            }
            catch (FileNotFoundException ex)
            {
                AppService.LogLine(ex.Message);
            }
            catch (Exception ex)
            {
                AppService.LogLine("Error loading object: " + ex.Message);
            }

            //re init since it was nulled out when assuming json
            obj = Activator.CreateInstance(objType.GetType());

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
                    AppService.LogLine($"Mapping error for property: {prop.Name}. Error: {e.Message}");
                    return Activator.CreateInstance(objType.GetType());
                }
            }

            return obj;
        }

        /// <summary>
        /// saves an object's properties to the specified path as json. overwrites existing file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public static void SaveObjectToFile(Object obj, string path)
        {
            EraseFile(path);

            try
            {
                var writer = new StreamWriter(path);

                var json = JsonConvert.SerializeObject(obj);

                writer.WriteLine(json);
                writer.Close();
            }
            catch (Exception e)
            {
                AppService.LogLine("Error saving: " + e.Message);
            }

            //var props = new List<KeyValuePair<string, string>>();

            //foreach (var prop in obj.GetType().GetProperties())
            //{
            //    props.Add(new KeyValuePair<string, string>(prop.Name, prop.GetValue(obj).ToString()));
            //}

            //try
            //{
            //    foreach (KeyValuePair<string, string> prop in props)
            //    {
            //        writer.WriteLine(prop.Key + GlobalConstants.Delimiter + prop.Value);
            //    }

            //    writer.Close();
            //}
            //catch (Exception e)
            //{
            //    AppService.LogLine("Error saving: " + e.Message);
            //}

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
                AppService.LogLine(e.Message);
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
                AppService.LogLine(e.Message);
            }
        }

        //TODO: split out data functions from non data functions
        public static List<string> GetStringCollectionFromFile(string path)
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
                AppService.LogLine(ex.Message);
            }
            catch (Exception ex)
            {
                AppService.LogLine(ex.Message);
            }

            return lines;
        }

    }
}

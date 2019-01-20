using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProgramOptions opts = null;
        bool itemLoaded = false;
        bool canGenerate = false;
        object currentItem = null;
        string generatedTemplate = null;
        string generatedTemplatePath = null;
        string generatedTemplateDirectory = null;

        public MainWindow()
        {
            InitializeComponent();
            StupidLogWindowFix();
            Bootstrapper.InitApplication();
            RefreshOptions();
            LoadTemplates();
            LoadItemTypes();
        }

        /// <summary>
        /// force a large width first column until i can figure out how to do it cleanly
        /// </summary>
        private void StupidLogWindowFix()
        {
            //TODO: fix log window first column
            var blankString = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                blankString.Append(" ");
            }
            blankString.Append("-");
            lstLog.Items.Add(blankString.ToString());
        }

        /// <summary>
        /// log a line to the listbox at the bottom of the page
        /// </summary>
        /// <param name="s"></param>
        public void LogLine(string s)
        {
            lstLog.Items.Add(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + s);
        }

        public void LoadTemplates()
        {
            var templateNames = GlobalFunctions.LoadTemplateList();

            cboTemplate.Items.Clear();
            foreach (string name in templateNames)
            {
                cboTemplate.Items.Add(name);
            }

            cboTemplate.SelectedItem = opts.SelectedTemplate;
        }

        public void LoadItemTypes()
        {
            cboItemType.Items.Clear();
            foreach (string item in opts.ItemTypes())
            {
                cboItemType.Items.Add(item);
            }

            cboItemType.SelectedItem = opts.SelectedItemType;
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            var OptionsWindow = new Options();
            OptionsWindow.Show();
        }

        private void CboTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            opts.SelectedTemplate = cboTemplate.SelectedItem.ToString();
            GlobalFunctions.SaveProgramOptions(opts);
            RefreshOptions();
        }

        public void RefreshOptions()
        {
            opts = GlobalFunctions.GetProgramOptions();
        }

        /// <summary>
        /// load or refresh loaded item in memory
        /// </summary>
        /// <param name="item"></param>
        /// <param name="path"></param>
        public void LoadItem(Object item, string path)
        {
            currentItem = item;
            itemLoaded = true;
            cboItemType.IsEnabled = false;
            lblActiveItem.Content = path;
            CheckCanGenerate();
            btnUnload.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnOpenItemDirectory.IsEnabled = true;
        }

        /// <summary>
        /// saves current item in its present state to its present path and unloads from memory
        /// </summary>
        private void UnloadItem()
        {
            if (itemLoaded)
            {
                GlobalFunctions.SaveObjectToFile(currentItem, lblActiveItem.Content.ToString());
                currentItem = null;
                itemLoaded = false;
                btnEdit.IsEnabled = false;
                btnOpenItemDirectory.IsEnabled = false;
                cboItemType.IsEnabled = true;
                lblActiveItem.Content = "None";
                btnUnload.IsEnabled = false;

                CheckCanGenerate();
                UnloadGeneratedOutput();

                LogLine("Closed item");
            }
        }

        private void UnloadGeneratedOutput()
        {
            generatedTemplate = null;
            generatedTemplatePath = null;
            generatedTemplateDirectory = null;
            btnCopy.IsEnabled = false;
            btnSave.IsEnabled = false;
            btnOpenBrowser.IsEnabled = false;
            btnOpenHtmlDir.IsEnabled = false;
        }

        private void CboItemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            opts.SelectedItemType = cboItemType.SelectedItem.ToString();
            GlobalFunctions.SaveProgramOptions(opts);
            RefreshOptions();
        }

        private void CheckCanGenerate()
        {
            canGenerate = itemLoaded && currentItem != null && !currentItem.IsNothing();

            btnGenerate.IsEnabled = canGenerate;
        }

        private Object GetCurrentItemType()
        {
            switch (cboItemType.SelectedItem.ToString())
            {
                case "Computer":
                    return new Computer();
                case "Part":
                    return new Part();
                default:
                    return new Item();
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            Window editWindow;
            switch (cboItemType.SelectedItem.ToString())
            {
                case "Computer":
                    editWindow = new EditComputer(new Computer(), opts, "");
                    break;
                case "Part":
                    editWindow = new EditPart(new Part(), opts, "");
                    break;
                default:
                    editWindow = new EditItem(new Item(), opts);
                    break;
            }

            editWindow.Show();
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            //clear anything previously generated
            UnloadGeneratedOutput();

            var lines = GlobalFunctions.GenerateFromTemplate(opts, currentItem);
            var outString = new StringBuilder();
            foreach (string line in lines)
            {
                outString.Append(line);
            }

            if (!string.IsNullOrWhiteSpace(outString.ToString()) && !outString.ToString().Contains("Error replacing property"))
            {
                generatedTemplate = outString.ToString();
                btnCopy.IsEnabled = true;
                btnSave.IsEnabled = true;
                LogLine($"Generated description for item using template \"{opts.SelectedTemplate}\"");
            }
            else
            {
                LogLine("One or more errors occurred generating description. Does the template selected match the loaded item type?");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (itemLoaded)
            {
                Window editWindow;
                switch (cboItemType.SelectedItem.ToString())
                {
                    case "Computer":
                        editWindow = new EditComputer((Computer)currentItem, opts, lblActiveItem.Content.ToString());
                        break;
                    case "Part":
                        editWindow = new EditPart((Part)currentItem, opts, lblActiveItem.Content.ToString());
                        break;
                    default:
                        editWindow = new EditItem((Item)currentItem, opts);
                        break;
                }

                editWindow.Show();
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var fileName = GlobalFunctions.ShowOpenFileDialog(opts, "txt files (*.txt)|*.txt");

            if (!string.IsNullOrWhiteSpace(fileName) && fileName.Split('.').Last().ToUpper().Equals("TXT"))
            {
                var itemObject = GetCurrentItemType();

                itemObject = GlobalFunctions.LoadObjectFromFile(itemObject, fileName, false);
                if (itemObject.IsNothing())
                {
                    LogLine("Item type mismatch. Select correct item type from drop down or add a new item via Options");
                }
                else
                {
                    LoadItem(itemObject, fileName);
                    LogLine($"Loaded \"{fileName}\" as {itemObject.GetType().ToString()}");
                }
            }
            else
            {
                GlobalFunctions.LogLine($"Items must be in a TXT file format");
            }
        }

        private void BtnUnload_Click(object sender, RoutedEventArgs e)
        {
            UnloadItem();
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(generatedTemplate))
            {
                Clipboard.SetText(generatedTemplate);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            generatedTemplatePath = GlobalFunctions.ShowSaveDialog(opts, "html");

            if (string.IsNullOrWhiteSpace(generatedTemplatePath))
            {
                LogLine("Must provide a path to save");
            }
            else
            {
                GlobalFunctions.SaveSingleLineToFile(generatedTemplate, generatedTemplatePath);

                var endOfDirectoryPath = generatedTemplatePath.LastIndexOf('\\');
                generatedTemplateDirectory = generatedTemplatePath.Substring(0, endOfDirectoryPath);
                LogLine($"Saved template HTML to \"{generatedTemplatePath}\"");
                btnOpenBrowser.IsEnabled = true;
                btnOpenHtmlDir.IsEnabled = true;
            }
        }

        private void BtnOpenHtmlDir_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", generatedTemplateDirectory);
        }

        private void BtnOpenItemDirectory_Click(object sender, RoutedEventArgs e)
        {
            var endOfDirectoryPath = lblActiveItem.Content.ToString().LastIndexOf('\\');
            System.Diagnostics.Process.Start("explorer.exe", lblActiveItem.Content.ToString().Substring(0, endOfDirectoryPath));
        }

        private void BtnOpenBrowser_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(generatedTemplatePath);
        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            lstLog.Items.Clear();
        }

        private void BtnSaveLog_Click(object sender, RoutedEventArgs e)
        {
            var path = GlobalFunctions.ShowSaveDialog(opts, "txt");

            var logs = new List<string>();
            foreach (var log in lstLog.Items)
            {
                logs.Add(log.ToString());
            }

            GlobalFunctions.SaveLineCollectionToFile(logs, path);
            LogLine($"Saved logs to \"{path}\"");

        }
    }
}

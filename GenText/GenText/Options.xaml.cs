using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace GenText
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        ProgramOptions opts;

        public Options()
        {
            InitializeComponent();
            opts = AppService.GetProgramOptions();
            PopulateExistingOptions();
            FixWithStaticItemTypes();
        }

        private void PopulateExistingOptions()
        {
            //TODO: add short description generation
            txtShortDescMaxSize.Text = opts.MaxShortDescriptionTextLength.ToString();
            chkBasicAdvancedEqual.IsChecked = opts.BasicFieldsSameAsAdvanced;
            txtTermsPath.Text = opts.DefaultTermsPathP1;
            txtTermsPath_Copy.Text = opts.DefaultTermsPathP2;
            txtDelimiter.Text = GlobalConstants.Delimiter.ToString();
            txtConvertNew.Text = GlobalConstants.Delimiter.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            opts.MaxShortDescriptionTextLength = Convert.ToInt32(txtShortDescMaxSize.Text);
            opts.BasicFieldsSameAsAdvanced = chkBasicAdvancedEqual.IsChecked.HasValue && chkBasicAdvancedEqual.IsChecked.Value;
            opts.DefaultTermsPathP1 = txtTermsPath.Text;
            opts.DefaultTermsPathP2 = txtTermsPath_Copy.Text;

            AppService.SaveProgramOptions(opts);
            AppService.LogLine("Saved options");
            AppService.RefreshMainWindowOptions();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            AppService.RefreshMainWindowOptions();
            this.Close();
        }

        /// <summary>
        /// copy a user specified html file to the template path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddTemplate_Click(object sender, RoutedEventArgs e)
        {
            var path = AppService.ShowOpenFileDialog(opts, "Html-Files(*.html)|*.html");

            if (!string.IsNullOrWhiteSpace(path))
            {
                if (path.Split('.').Last().ToUpper().Equals("HTML"))
                {
                    File.Copy(path, GlobalConstants.TemplatesPath + "\\" + path.Split('\\').Last(), true);
                    AppService.LogLine($"Added template file \"{path}\" to Templates");
                    AppService.RefreshMainWindowOptions();
                }
                else
                {
                    AppService.LogLine($"Did not add template file \"{path}\". File must be HTML type");
                }
            }
            else
            {
                MessageBox.Show("Must specify a file");
            }

        }

        private void FixWithStaticItemTypes()
        {
            //TODO:replace this with an add/remove item types functionality
            if (string.IsNullOrWhiteSpace(opts.ItemTypesString))
                opts.ItemTypesString = "Computer,Part,Item";
        }

        private void BtnSelectOutPath_Click(object sender, RoutedEventArgs e)
        {
            //TODO:implement select directory for out path
            MessageBox.Show("Not Implemented");
            //var path = AppService.ShowOpenFileDialog(opts, "txt files (*.txt)|*.txt");

            //if(!string.IsNullOrWhiteSpace(path))
            //{
            //    txtDefaultOutItemPath.Text = path;
            //}
        }

        private void BtnSelectTermsPath_Click(object sender, RoutedEventArgs e)
        {
            var path = AppService.ShowOpenFileDialog(opts, "txt files (*.txt)|*.txt");

            if (!string.IsNullOrWhiteSpace(path))
            {
                txtTermsPath.Text = path;
            }
        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtConvertOld.Text) || !string.IsNullOrWhiteSpace(txtConvertNew.Text))
            {
                var path = AppService.ShowOpenFileDialog(opts, "txt files (*.txt)|*.txt");

                if (!string.IsNullOrWhiteSpace(path))
                {
                    var lines = FileIoService.GetStringCollectionFromFile(path);
                    var newLines = new List<string>();

                    foreach (string line in lines)
                    {
                        newLines.Add(line.Replace(txtConvertOld.Text.Single(), txtConvertNew.Text.Single()));
                    }

                    FileIoService.SaveLineCollectionToFile(newLines, path);
                    AppService.LogLine("Converted file: " + path);
                }
                else
                {
                    MessageBox.Show("Path is required to convert file");
                }
            }
            else
            {
                MessageBox.Show("Must have a value in both delimiter boxes");
            }
        }
    }
}

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
            opts = GlobalFunctions.GetProgramOptions();
            PopulateExistingOptions();
            FixWithStaticItemTypes();
        }

        private void PopulateExistingOptions()
        {
            txtShortDescMaxSize.Text = opts.MaxShortDescriptionTextLength.ToString();
            chkBasicAdvancedEqual.IsChecked = opts.BasicFieldsSameAsAdvanced;
            txtDefaultOutItemPath.Text = opts.DefaultItemOutPath;
            txtTermsPath.Text = opts.DefaultTermsPath;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            opts.MaxShortDescriptionTextLength = Convert.ToInt32(txtShortDescMaxSize.Text);
            opts.BasicFieldsSameAsAdvanced = chkBasicAdvancedEqual.IsChecked.HasValue && chkBasicAdvancedEqual.IsChecked.Value;
            opts.DefaultItemOutPath = txtDefaultOutItemPath.Text;
            opts.DefaultTermsPath = txtTermsPath.Text;

            GlobalFunctions.SaveProgramOptions(opts);
            ((MainWindow)System.Windows.Application.Current.Windows[0]).RefreshOptions();
            GlobalFunctions.LogLine("Options have been updated");
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.Windows[0]).RefreshOptions();
            this.Close();
        }

        private void BtnAddTemplate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Html-Files(*.html)|*.html",
                Multiselect = false,
                InitialDirectory = string.IsNullOrWhiteSpace(opts.DefaultItemOutPath) ? GlobalConstants.DefaultPath : opts.DefaultItemOutPath
            };

            var result = dialog.ShowDialog();

            if(result.HasValue && result.Value)
            {
                if(dialog.FileName.Split('.').Last().ToUpper().Equals("HTML"))
                {
                    GlobalFunctions.LogLine($"Adding file {dialog.FileName}");
                    File.Copy(dialog.FileName, GlobalConstants.TemplatesPath + "\\" + dialog.FileName.Split('\\').Last(), true);
                    ((MainWindow)System.Windows.Application.Current.Windows[0]).RefreshOptions();
                }
                else
                {
                    GlobalFunctions.LogLine($"Did not add file {dialog.FileName}. Can only add HTML files");
                }
            }

        }

        private void FixWithStaticItemTypes()
        {
            //TODO:replace this with an add/remove item types functionality
            if (string.IsNullOrWhiteSpace(opts.ItemTypesString))
                opts.ItemTypesString = "Computer|Part";
        }

        private void BtnSelectOutPath_Click(object sender, RoutedEventArgs e)
        {
            //TODO:implement select directory for out path
            MessageBox.Show("Not Implemented");
            //var path = GlobalFunctions.ShowOpenFileDialog(opts, "txt files (*.txt)|*.txt");

            //if(!string.IsNullOrWhiteSpace(path))
            //{
            //    txtDefaultOutItemPath.Text = path;
            //}
        }

        private void BtnSelectTermsPath_Click(object sender, RoutedEventArgs e)
        {
            var path = GlobalFunctions.ShowOpenFileDialog(opts, "txt files (*.txt)|*.txt");

            if (!string.IsNullOrWhiteSpace(path))
            {
                txtTermsPath.Text = path;
            }
        }
    }
}

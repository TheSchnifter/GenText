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
            //TODO: add short description generation
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
            GlobalFunctions.LogLine("Saved options");
            ((MainWindow)System.Windows.Application.Current.Windows[0]).RefreshOptions();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.Windows[0]).RefreshOptions();
            this.Close();
        }

        /// <summary>
        /// copy a user specified html file to the template path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddTemplate_Click(object sender, RoutedEventArgs e)
        {
            var path = GlobalFunctions.ShowOpenFileDialog(opts, "Html-Files(*.html)|*.html");

            if(!string.IsNullOrWhiteSpace(path))
            {
                if(path.Split('.').Last().ToUpper().Equals("HTML"))
                {
                    File.Copy(path, GlobalConstants.TemplatesPath + "\\" + path.Split('\\').Last(), true);
                    GlobalFunctions.LogLine($"Added template file \"{path}\" to Templates");
                    ((MainWindow)System.Windows.Application.Current.Windows[0]).RefreshOptions();
                }
                else
                {
                    GlobalFunctions.LogLine($"Did not add template file \"{path}\". File must be HTML type");
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
                opts.ItemTypesString = "Computer|Part|Item";
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

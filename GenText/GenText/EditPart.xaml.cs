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
using System.Windows.Shapes;

namespace GenText
{
    /// <summary>
    /// Interaction logic for EditPart.xaml
    /// </summary>
    public partial class EditPart : Window
    {
        Part p;
        string path;
        ProgramOptions opts;

        public EditPart(Part item, ProgramOptions opts, string outPath)
        {
            InitializeComponent();
            p = item;
            path = outPath;
            this.opts = opts;

            txtItemLongDescP1.Text = p.ItemLongDescP1;
            txtItemLongDescP2.Text = p.ItemLongDescP2;
            txtItemTitle.Text = p.ItemTitle;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            p.ItemTitle = txtItemTitle.Text;
            p.ItemLongDescP1 = txtItemLongDescP1.Text;
            p.ItemLongDescP2 = txtItemLongDescP2.Text;

            if (string.IsNullOrWhiteSpace(path))
            {
                path = GlobalFunctions.ShowSaveDialog(opts, "txt");
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                GlobalFunctions.SaveObjectToFile(p, path);
                GlobalFunctions.RefreshItem(p, path);
                GlobalFunctions.LogLine($"Saved item to \"{path}\"");
                this.Close();
            }
            else
            {
                MessageBox.Show("Must specify a path");
            }
        }
    }
}

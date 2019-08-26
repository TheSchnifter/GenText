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
        Part part;
        string path;
        ProgramOptions opts;

        public EditPart(Part item, ProgramOptions opts, string outPath)
        {
            InitializeComponent();
            part = item;
            path = outPath;
            this.opts = opts;

            txtItemLongDescP1.Text = part.ItemLongDescP1;
            txtItemLongDescP2.Text = part.ItemLongDescP2;
            txtItemTitle.Text = part.ItemTitle;
            txtItemLotCode.Text = part.ItemLotCode;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            part.ItemTitle = txtItemTitle.Text;
            part.ItemLongDescP1 = txtItemLongDescP1.Text;
            part.ItemLongDescP2 = txtItemLongDescP2.Text;
            part.ItemLotCode = txtItemLotCode.Text;

            if (string.IsNullOrWhiteSpace(path))
            {
                path = AppService.ShowSaveDialog(opts, "txt", $"part-{part.ItemLotCode}");
            }
            else
            {
                FileIoService.SaveObjectToFile(part, path);
                AppService.RefreshItem(part, path);
                AppService.RefreshMainWindowOptions();
                AppService.LogLine($"Saved item to \"{path}\"");
                this.Close();
            }
        }
    }
}

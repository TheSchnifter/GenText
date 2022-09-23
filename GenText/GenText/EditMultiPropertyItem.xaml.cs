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
    public partial class EditMultiPropertyItem : Window
    {
        MultiPropertyItem multiPropertyItem;
        string path;
        ProgramOptions opts;

        public EditMultiPropertyItem(MultiPropertyItem item, ProgramOptions opts, string outPath)
        {
            InitializeComponent();
            multiPropertyItem = item;
            path = outPath;
            this.opts = opts;

            txtItemLongDescP1.Text = multiPropertyItem.ItemLongDescP1;
            
            //not used on this item type
            multiPropertyItem.ItemLongDescP2 = "";

            txtItemTitle.Text = multiPropertyItem.ItemTitle;

            foreach(KeyValuePair<string,string> prop in item.ItemDetails)
            {
                var detail = new ItemDetail(prop.Key, prop.Value);
                lstDetails.Items.Add(detail);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtDetailName.Text) && !string.IsNullOrWhiteSpace(txtDetailValue.Text))
            {
                var detail = new ItemDetail(txtDetailName.Text, txtDetailValue.Text);
                lstDetails.Items.Add(detail);
                txtDetailName.Text = "";
                txtDetailValue.Text = "";
                txtDetailName.Focus();
            }
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var index = lstDetails.SelectedIndex;

            if (index - 1 >= 0)
            {
                var item = lstDetails.Items.GetItemAt(index);
                lstDetails.Items.RemoveAt(index);
                lstDetails.Items.Insert(index - 1, item);
            }
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var index = lstDetails.SelectedIndex;

            if (index + 1 < lstDetails.Items.Count)
            {
                var item = lstDetails.Items.GetItemAt(index);
                lstDetails.Items.RemoveAt(index);
                lstDetails.Items.Insert(index + 1, item);
            }
        }

        private void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            lstDetails.Items.RemoveAt(lstDetails.SelectedIndex);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            multiPropertyItem.ItemTitle = txtItemTitle.Text;
            multiPropertyItem.ItemLongDescP1 = txtItemLongDescP1.Text;

            //replace whatever we had with whatever we have now
            multiPropertyItem.ItemDetails = new List<KeyValuePair<string, string>>();
            foreach( ItemDetail detail in lstDetails.Items )
            {
                multiPropertyItem.ItemDetails.Add(new KeyValuePair<string, string>(detail.Name, detail.Value));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                path = AppService.ShowSaveDialog(opts, "txt", "multiPropertyItem");
            }
            
            if (!string.IsNullOrWhiteSpace(path))
            {
                FileIoService.SaveObjectToFile(multiPropertyItem, path);
                AppService.RefreshItem(multiPropertyItem, path);
                AppService.RefreshMainWindowOptions();
                AppService.LogLine($"Saved item to \"{path}\"");
                this.Close();
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                BtnAddItem_Click(sender, e);
            }
        }

        private class ItemDetail
        {
            public ItemDetail(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}

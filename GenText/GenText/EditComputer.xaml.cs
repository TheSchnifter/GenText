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
using System.Windows.Shapes;

namespace GenText
{
    /// <summary>
    /// Interaction logic for EditComputer.xaml
    /// </summary>
    public partial class EditComputer : Window
    {
        Computer c;
        string path;
        ProgramOptions opts;

        public EditComputer(Computer item, ProgramOptions opts, string outPath)
        {
            InitializeComponent();
            c = item;
            path = outPath;
            this.opts = opts;

            txtBasicCPU.Text = c.BasicCPU;
            txtBasicRAM.Text = c.BasicRAM;
            txtBasicHDD.Text = c.BasicHDD;
            txtBasicOS.Text = c.BasicOS;

            if (opts.BasicFieldsSameAsAdvanced)
            {
                txtCPUType.Text = c.BasicCPU;
                txtRAMCapacity.Text = string.IsNullOrWhiteSpace(c.BasicRAM) ? "" : c.BasicRAM.Split(' ').FirstOrDefault();
                txtRAMType.Text = string.IsNullOrWhiteSpace(c.BasicRAM) ? "" : c.BasicRAM.Split(' ').LastOrDefault();
                txtHDDSize.Text = c.BasicHDD;
            }
            else
            {
                txtCPUType.Text = c.CPUType;
                txtRAMCapacity.Text = c.RAMCapacity;
                txtRAMType.Text = c.RAMType;
                txtHDDSize.Text = c.HDDSize;
            }

            txtCPUSpeed.Text = c.CPUSpeed;
            txtCPUCores.Text = c.CPUCores;
            txtCPUThreads.Text = c.CPUThreads;
            txtHDDQuantity.Text = c.HDDQuantity;
            txtHDDSpeed.Text = c.HDDSpeed;
            txtGFX.Text = c.GFX;
            txtScreenSize.Text = c.ScreenSize;
            txtScreenRes.Text = c.ScreenRes;
            txtScreenType.Text = c.ScreenType;
            txtBattery.Text = c.Battery;
            txtOSVersion.Text = c.OSVersion;
            txtWebcam.Text = c.Webcam;
            txtOtherDrives.Text = c.OtherDrives;
            txtNics.Text = c.Nics;
            txtItemTitle.Text = c.ItemTitle;
            txtItemLongDescP1.Text = c.ItemLongDescP1;
            txtItemLongDescP2.Text = c.ItemLongDescP2;
            txtHDDInterface.Text = c.HDDInterface;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //TODO: add basic and advanced combination on option flag
            c.BasicCPU = txtBasicCPU.Text;
            c.BasicRAM = txtBasicRAM.Text;
            c.BasicHDD = txtBasicHDD.Text;
            c.BasicOS = txtBasicOS.Text;
            c.CPUType = txtCPUType.Text;
            c.RAMCapacity = txtRAMCapacity.Text;
            c.RAMType = txtRAMType.Text;
            c.HDDSize = txtHDDSize.Text;
            c.CPUSpeed = txtCPUSpeed.Text;
            c.CPUCores = txtCPUCores.Text;
            c.CPUThreads = txtCPUThreads.Text;
            c.HDDQuantity = txtHDDQuantity.Text;
            c.HDDSpeed = txtHDDSpeed.Text;
            c.GFX = txtGFX.Text;
            c.ScreenSize = txtScreenSize.Text;
            c.ScreenRes = txtScreenRes.Text;
            c.ScreenType = txtScreenType.Text;
            c.Battery = txtBattery.Text;
            c.OSVersion = txtOSVersion.Text;
            c.Webcam = txtWebcam.Text;
            c.OtherDrives = txtOtherDrives.Text;
            c.Nics = txtNics.Text;
            c.ItemTitle = txtItemTitle.Text;
            c.ItemLongDescP1 = txtItemLongDescP1.Text;
            c.ItemLongDescP2 = txtItemLongDescP2.Text;
            c.HDDInterface = txtHDDInterface.Text;

            if (string.IsNullOrWhiteSpace(path))
            {
                path = GlobalFunctions.ShowSaveDialog(opts, "txt", "computer");
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                GlobalFunctions.SaveObjectToFile(c, path);
                GlobalFunctions.RefreshItem(c, path);
                GlobalFunctions.LogLine($"Saved item to \"{path}\"");
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TxtBasicCPU_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(opts.BasicFieldsSameAsAdvanced)
            {
                txtCPUType.Text = txtBasicCPU.Text;
            }
        }

        private void TxtBasicRAM_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(opts.BasicFieldsSameAsAdvanced)
            {
                txtRAMCapacity.Text = string.IsNullOrWhiteSpace(txtBasicRAM.Text) ? "" : txtBasicRAM.Text.Split(' ').FirstOrDefault();
                txtRAMType.Text = string.IsNullOrWhiteSpace(txtBasicRAM.Text) ? "" : txtBasicRAM.Text.Split(' ').LastOrDefault();
            }
        }

        private void TxtBasicHDD_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(opts.BasicFieldsSameAsAdvanced)
            {
                txtHDDSize.Text = txtBasicHDD.Text;
            }
        }

        private void TxtBasicOS_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(opts.BasicFieldsSameAsAdvanced)
            {
                txtOSVersion.Text = txtBasicOS.Text;
            }
        }
    }
}

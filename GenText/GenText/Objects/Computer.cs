using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenText
{
    public class Computer : Item
    {
        public string BasicCPU { get; set; }
        public string BasicRAM { get; set; }
        public string BasicHDD { get; set; }
        public string BasicOS { get; set; }
        public string CPUType { get; set; }
        public string CPUSpeed { get; set; }
        public string CPUCores { get; set; }
        public string CPUThreads { get; set; }
        public string RAMCapacity { get; set; }
        public string RAMType { get; set; }
        public string HDDQuantity { get; set; }
        public string HDDSize { get; set; }
        public string HDDInterface { get; set; }
        public string HDDSpeed { get; set; }
        public string GFX { get; set; }
        public string ScreenSize { get; set; }
        public string ScreenRes { get; set; }
        public string ScreenType { get; set; }
        public string Battery { get; set; }
        public string OSVersion { get; set; }
        public string Webcam { get; set; }
        public string OtherDrives { get; set; }
        public string Nics { get; set; }
    }
}

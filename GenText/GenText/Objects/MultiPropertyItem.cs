using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenText
{
    public class MultiPropertyItem : Item
    {
        public MultiPropertyItem()
        {
            ItemDetails = new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> ItemDetails { get; set; }
    }
}

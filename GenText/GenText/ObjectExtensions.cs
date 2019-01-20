using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenText
{
    public static class ObjectExtensions
    {
        public static List<string> ItemTypes(this ProgramOptions opts)
        {
            return opts.ItemTypesString.Split('|').ToList();
        }

        /// <summary>
        /// returns whether all strings in an object are nothing
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNothing(this Object obj)
        {
            foreach (var prop in obj.GetType().GetProperties().Where(x => x.PropertyType.Equals(typeof(string))))
            {
                if (prop.GetValue(obj) != null && !string.IsNullOrWhiteSpace(prop.GetValue(obj).ToString()))
                    return false;
            }

            return true;
        }
    }
}

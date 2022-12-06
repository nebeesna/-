using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab7
{
    internal class Action
    {
        public static void Start(Object o)
        {
            Data data = (Data)o;
            for (int i = data.start; i < data.end; i++)
            {
                data.writer.WriteLine(data.text[i].Replace(data.oldValue, data.newValue));
            }
        }
    }
}

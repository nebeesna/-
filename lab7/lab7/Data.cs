using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab7
{
    internal struct Data
    {
        public StreamWriter writer;
        public List<string> text;
        public string oldValue;
        public string newValue;
        public int start;
        public int end;
    }
}

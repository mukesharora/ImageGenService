using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReaderApi.Config
{
    public class ReaderConfig 
    {
        public string ReaderID { get; set; }

        public string HostName { get; set; }

        public int ReadTimeInMs { get; set; }
        
        public int[] AntennaPowers { get; set; }
    }
}

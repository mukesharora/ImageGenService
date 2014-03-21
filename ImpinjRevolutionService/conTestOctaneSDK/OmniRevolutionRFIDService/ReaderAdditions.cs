using System;
using System.Data.Objects.DataClasses;
using System.Linq;

namespace OmniRevolutionRFIDService
{
    public partial class Reader : EntityObject
    {
        public static readonly string SWITCH_READERMODE = "switch";
        public static readonly string LDC_READERMODE = "ldc";

        public string ReaderMode { get; set; }

        public int[] AntennaPowers { get; set; }        
    }
}
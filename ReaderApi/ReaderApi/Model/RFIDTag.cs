using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Impinj.OctaneSdk;

namespace ReaderApi.Model
{
    public class RFIDTag
    {
        /// <summary>
        /// Contents of the tag EPC memory bank.
        /// </summary>
        public string Epc
        {
            get;
            set;
        }

        /// <summary>
        /// The reader antenna port number for the antenna that last saw the tag;
        /// </summary>
        public ushort AntennaPortNumber
        {
            get;
            set;
        }

        public void CopyFrom(Tag tag)
        {            
            Epc = tag.Epc.ToHexString();         
            AntennaPortNumber = tag.AntennaPortNumber;
        }
    }
}

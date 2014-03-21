using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octane2ReaderBLL
{
    public interface ITagReportSink
    {
        void ReportTagRead(string readerID, string readerAddress, string EPC, int AntennaPortNumber = -1, string tagUSER = "");
    }
}

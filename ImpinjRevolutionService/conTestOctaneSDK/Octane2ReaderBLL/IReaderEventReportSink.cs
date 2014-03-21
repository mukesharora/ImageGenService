using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octane2ReaderBLL
{
    public interface IReaderEventReportSink
    {
        void ReportReaderEvent(string readerID, string eventDescription, string errorCode = "");
    }
}

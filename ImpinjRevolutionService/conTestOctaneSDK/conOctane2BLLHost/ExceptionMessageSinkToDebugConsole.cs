using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane2ReaderBLL;
using System.Diagnostics;

namespace conOctane2BLLHost
{
    public class ExceptionMessageSinkToDebugConsole : IReaderEventReportSink, ISystemExceptionSink
    {
        public void ReportReaderEvent(string readerID, string eventDescription, string errorCode = "")
        {
            DebugConsole.Instance.WriteLine(string.Format("Reader Event : {0} reports : {1}, errorCode : {2}", readerID, eventDescription, errorCode));
        }

        public void LogSystemException(string description)
        {
            DebugConsole.Instance.WriteLine(string.Format("Exception : {0}",description));
        }
    }
}

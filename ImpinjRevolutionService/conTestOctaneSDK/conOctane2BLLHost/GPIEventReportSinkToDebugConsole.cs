using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane2ReaderBLL;
using System.Diagnostics;

namespace conOctane2BLLHost
{
    public class GPIEventReportSinkToDebugConsole : IGPIReportSink
    {
        public void ReportGPIEvent(string readerID, int port, bool newState)
        {
            DebugConsole.Instance.WriteLine(string.Format("GPI: Rdr:{0} Port:{1} NewState:{2}",readerID,port,newState));
        }
    }
}

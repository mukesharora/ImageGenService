using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octane2ReaderBLL
{
    public interface IGPIReportSink
    {
        void ReportGPIEvent(string readerID, int port, bool newState);
    }
}

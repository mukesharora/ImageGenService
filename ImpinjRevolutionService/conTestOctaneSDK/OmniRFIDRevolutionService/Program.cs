using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Diagnostics;

namespace OmniRFIDRevolutionService
{
    static class Program
    {
        static EventLog g_EventLog;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            #if DEBUG
            RFIDService s = new RFIDService();
            s.StartService();
            s.StopService();
#else

            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new RFIDService() 
			    };

                g_EventLog = ((RFIDService)ServicesToRun[0]).EventLog;

                ServiceBase.Run(ServicesToRun);

            }
            catch (Exception ex)
            {
                if (null != g_EventLog)
                {
                    g_EventLog.WriteEntry(string.Format("Top Level Exception Occurred : {0}", ex.Message), EventLogEntryType.Error);
                }

            }
#endif
        }
    }
}

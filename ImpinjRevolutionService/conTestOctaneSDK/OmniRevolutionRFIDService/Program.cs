using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace OmniRevolutionRFIDService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG2
            RFIDService s = new RFIDService();
            s.StartService();
            s.StopService();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new RFIDService() 
			};
            ServiceBase.Run(ServicesToRun);
#endif
        }

      
    }
}

using System;
using System.Configuration;
using System.Linq;
using RFIDWebApiService;

namespace RFIDWebApiServiceConsole
{
    class Program
    {
        private static RFIDWebApiService.RFIDWebApiService service;
        
        static void Main(string[] args)
        {
            OnStart();
            Console.ReadLine();
            OnStop();
        }


        protected static void OnStart()
        {
         
            try
            {
                ////Establish IPC connection:
                //EstablishIPCConnection();

                ////Instantiate impinj interface:
                //Globals.ReaderInterface = new ReaderInterface(Globals.DBPath, Globals.TagReportSinkInterface,
                //                                Globals.GPIReportSinkInterface, Globals.ReaderEventSinkInterface,
                //                                Globals.SystemExceptionReportSinkInterface);
                //Globals.IPCProxy.ReaderCommandEvent += new ReaderCommandHandler(Globals.ReaderInterface.OnCommandReceived);
                //Globals.ReaderInterface.Start();

                string serviceHostName = "http://localhost:4000";
                try
                {
                    serviceHostName = ConfigurationManager.AppSettings["RFIDWebApiServiceHostName"];
                }
                catch (Exception)
                {
                    //Logger.Error(string.Format("could not obtain hostname for RFIDWebApiService. Please define RFIDWebApiServiceHostName in app.config. Using default of {0}", serviceHostName));
                }
                service = new RFIDWebApiService.RFIDWebApiService();
                service.Start(serviceHostName);                
            }
            catch (Exception ex)
            {
                
            }
        }

        protected static void OnStop()
        {            
            try
            {
                //Globals.ReaderInterface.Stop();
                //Globals.ReaderInterface = null; //will create a new instance when this service is restarted.

                service.Stop();
                
            }
            catch (Exception ex)
            {
            }
        }

    }
}

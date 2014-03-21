using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ATRemoteObjectsLib;

namespace conSimulatedIPCApp
{
    class Program
    {
        public static class Globals
        {
            public static ATRemoteAnnounceServer IPCServer { get; set; }
            public static ATRemoteAnnounceServerHost<ATRemoteAnnounceServerIPCTest> IPCHost { get; set; }
        }

        static void CreateGlobalObjects()
        {
            Globals.IPCServer = new ATRemoteAnnounceServerIPCTest();
            try
            {
                int ipcPort = int.Parse(ConfigurationManager.AppSettings["IPC_Port"]);
                string ipcName = ConfigurationManager.AppSettings["IPC_Name"];
                Globals.IPCHost = new ATRemoteAnnounceServerHost<ATRemoteAnnounceServerIPCTest>(ipcPort, ipcName);
            }
            catch(Exception)
            {                                            
                Globals.IPCHost = new ATRemoteAnnounceServerHost<ATRemoteAnnounceServerIPCTest>();
            }
        }

        static void Main(string[] args)
        {
            //Create all our globals:
            CreateGlobalObjects();

            //Start the IPC server object, so the 'other side' can connect:

            Globals.IPCHost.StartServer();

            if (Globals.IPCHost.IsServing == false)
            {
                Console.WriteLine("ERROR : IPC Host reports NOT SERVING");
            }

             //DateTime timeCheck = DateTime.Now;

            Console.WriteLine("Press Escape key to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(250);
                    Application.DoEvents();

                    //if (timeCheck + TimeSpan.FromMinutes(1) < DateTime.Now)
                    //{
                    //    timeCheck = DateTime.Now;
                       
                    //}
                }

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

           // Globals.IPCHost.StopServer(); //MJD : Call Commented out because the Host<> does not implement this method, and throws a "Not Implemented Exception"


        }
    }
}

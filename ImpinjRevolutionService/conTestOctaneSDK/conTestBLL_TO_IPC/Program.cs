using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane2ReaderBLL;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using ATRemoteObjectsLib;

namespace conTestBLL_TO_IPC
{
    public static class Globals
    {
        public static string DBPath { get; set; }
        public static ITagReportSink TagReportSinkInterface { get; set; }
        public static IGPIReportSink GPIReportSinkInterface { get; set; }
        public static IReaderEventReportSink ReaderEventSinkInterface { get; set; }
        public static ISystemExceptionSink SystemExceptionReportSinkInterface { get; set; }
        public static ATRemoteAnnounceServerProxy IPCProxy { get; set; }
    }

    public class Program
    {
        static void CreateGlobalObjects()
        {
            //g_TagReportSink = new ConsoleTagReportSink() as ITagReportSink;
            //g_ReaderEventReportSink = new ExceptionMessageSinkToDebugConsole() as IReaderEventReportSink;
            //g_SystemExceptionReportSink = g_ReaderEventReportSink as ISystemExceptionSink;
            //g_GPIReportSink = new GPIEventReportSinkToDebugConsole() as IGPIReportSink;
            //DebugConsole.Instance.Init(true, false);

             Globals.DBPath = @"C:\Workshop\February2013\ImpinjOctane2SDKPrototypes\conTestOctaneSDK\ImpinjReadersConfiguration.s3db";
             Globals.IPCProxy    = new ATRemoteAnnounceServerProxy();
             Globals.TagReportSinkInterface = new TagReportSink();
             Globals.GPIReportSinkInterface = new GPIReportSink();
             Globals.ReaderEventSinkInterface = new ReaderEventSink();
             Globals.SystemExceptionReportSinkInterface = new SystemExceptionSink();
        }


        static void Main(string[] args)
        {
            CreateGlobalObjects();

            //Connect to the IPC Channel:
            Globals.IPCProxy.Connect();


            //TODO : Should do a retry scheme here - retry 10 times with a 10 second pause between each try.
            if (!Globals.IPCProxy.IsConnected)
            {
                Globals.SystemExceptionReportSinkInterface.LogSystemException("IPC Proxy Unable to Connect");
                //TODO : May want to terminate here.
            }

            DBReaderInterface ri = new DBReaderInterface(Globals.DBPath, Globals.TagReportSinkInterface,
                                                Globals.GPIReportSinkInterface, Globals.ReaderEventSinkInterface, 
                                                Globals.SystemExceptionReportSinkInterface);

            ri.Start();

            DateTime timeCheck = DateTime.Now;

            Console.WriteLine("Press Escape key to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(250);
                    Application.DoEvents();

                    if (timeCheck + TimeSpan.FromMinutes(1) < DateTime.Now)
                    {
                        timeCheck = DateTime.Now;
                        ri.TryReconnectToDisconnectedReaders();
                    }
                }

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            ri.Stop();
        }
    }
}

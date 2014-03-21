using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane2ReaderBLL;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace conOctane2BLLHost
{
    class Program
    {
        const string DBPath = @"C:\Workshop\February2013\ImpinjOctane2SDKPrototypes\conTestOctaneSDK\ImpinjReadersConfiguration.s3db";
        static ITagReportSink g_TagReportSink = null;
        static IGPIReportSink g_GPIReportSink = null;
        static IReaderEventReportSink g_ReaderEventReportSink = null;
        static ISystemExceptionSink g_SystemExceptionReportSink = null;

        static void CreateGlobalObjects()
        {
            g_TagReportSink = new ConsoleTagReportSink() as ITagReportSink;
            g_ReaderEventReportSink = new ExceptionMessageSinkToDebugConsole() as IReaderEventReportSink;
            g_SystemExceptionReportSink = g_ReaderEventReportSink as ISystemExceptionSink;
            g_GPIReportSink = new GPIEventReportSinkToDebugConsole() as IGPIReportSink;
            DebugConsole.Instance.Init(true, false);
        }

        static void Main(string[] args)
        {
            CreateGlobalObjects();

            DBReaderInterface ri = new DBReaderInterface(DBPath, g_TagReportSink, g_GPIReportSink, g_ReaderEventReportSink, g_SystemExceptionReportSink);

            ri.Start();

            Console.WindowWidth = 160;

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

            
            ri.Stop();  //shuts down all readers.
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Octane2ReaderBLL;
using ATRemoteObjectsLib;
using System.Threading;
using OmniRFIDRevolutionService.ReportSink;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System.IO;

namespace OmniRFIDRevolutionService
{
    public partial class RFIDService : ServiceBase
    {
        private const string EVENT_LOG_SOURCE_NAME = "OmniRFIDRevEvtLogSource";
        private const string EVENT_LOG_NAME = "OmniRFIDRevolutionSvc";
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RFIDService));

        public RFIDService()
        {
            InitializeComponent();
            ConfigureLog4Net();

            #region initialize windows event log object
            if (!System.Diagnostics.EventLog.SourceExists(EVENT_LOG_SOURCE_NAME))
            {
                System.Diagnostics.EventLog.CreateEventSource(EVENT_LOG_SOURCE_NAME, EVENT_LOG_NAME);
            }

            eventLog1.Source = EVENT_LOG_SOURCE_NAME;
            eventLog1.Log = EVENT_LOG_NAME;
            #endregion

            //Instantiate the concrete implementations of the 'sink' interfaces , and the IPC proxy.
            CreateGlobalObjects();

            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            _timer.Interval = 15000;
            _timer.Start();
        }

#if DEBUG
        public void StartService()
        {
            OnStart(null);
            Console.WriteLine("My Service Started, press <enter> to terminate");
            Console.ReadLine();
        }

        public void StopService()
        {
            OnStop();
        }

#endif

        /// <summary>
        /// Timer1 Time Elapsed handler - use to attempt to reconnect any disconnected readers:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (null != Globals.ReaderInterface)
            {
                try
                {
                    Globals.ReaderInterface.TryReconnectToDisconnectedReaders();
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry(string.Format("Exception occurred reconnecting disconnected readers : {0}",
                        ex.Message), EventLogEntryType.Error);
                }
            }
        }

        protected void CreateGlobalObjects()
        {
            Globals.DBPath = @"ImpinjReadersConfiguration.s3db";
            try
            {
                int ipcProxyPort = int.Parse(ConfigurationManager.AppSettings["IPC_Port"]);
				int ipcServerPort = int.Parse(ConfigurationManager.AppSettings["Client_IPC_Port"]);
                string ipcName = ConfigurationManager.AppSettings["IPC_Name"];

                Globals.IPCProxy = new ATRemoteAnnounceServerProxy(ipcProxyPort, ipcServerPort, ipcName);
            }
            catch(Exception e)
            {
                Logger.Warn("Could not read in IPC port and IPC name going to use the defaults.",e);
                Globals.IPCProxy = new ATRemoteAnnounceServerProxy();
            }
            Globals.TagReportSinkInterface = new TagReportSink();
            Globals.GPIReportSinkInterface = new GPIReportSink();
            Globals.ReaderEventSinkInterface = new ReaderEventSink();
            Globals.SystemExceptionReportSinkInterface = new SystemExceptionSink();
        }

        /// <summary>
        /// Establish (or re-establish) an IPC connection.
        /// </summary>
        /// <returns></returns>
        protected bool EstablishIPCConnection()
        {
            try
            {
                int maxConnectRetries = 10;
                int retryDelayMS = 10000;
                int retryCount = 0;

                //Connect to the IPC Channel:
                try
                {
                    Globals.IPCProxy.Connect();
                }
                catch (Exception ex)
                {
                    ;//just eat it! - want to have the below while(!Globals.IPCProxy.Connected) code execute if unable to connect.
                }

                while (!Globals.IPCProxy.IsConnected)
                {

                    if (retryCount >= maxConnectRetries)
                    {
                        eventLog1.WriteEntry(string.Format("Unable to establish IPC connection after {0} retries", retryCount), EventLogEntryType.Error);
                        return false;
                    }


                    try
                    {
                        eventLog1.WriteEntry(string.Format("Unable to establish IPC Connection - retry {0}", retryCount++), EventLogEntryType.Error);
                        Thread.Sleep(retryDelayMS);
                        Globals.IPCProxy.Connect();
                    }
                    catch (Exception ex)
                    {
                        ;//again, just eat it, and try agin
                    }


                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
                eventLog1.WriteEntry("Service Failed To Start", EventLogEntryType.Error);
            }

            return true;
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Service Starting");

            try
            {                
                //Establish IPC connection:
                EstablishIPCConnection();

                //Instantiate reader interface:
                Globals.ReaderInterface = new DBReaderInterface(Globals.DBPath, Globals.TagReportSinkInterface,
                                                Globals.GPIReportSinkInterface, Globals.ReaderEventSinkInterface,
                                                Globals.SystemExceptionReportSinkInterface);

                Globals.ReaderInterface.Start();


                eventLog1.WriteEntry("Service Started");
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
                eventLog1.WriteEntry("Service Failed To Start", EventLogEntryType.Error);
            }
 
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Service Stopping");

            try
            {
                Globals.ReaderInterface.Stop();
                Globals.ReaderInterface = null; //will create a new instance when this service is restarted.

                eventLog1.WriteEntry("Service Stopped");
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
                eventLog1.WriteEntry("Service Failed To Stop", EventLogEntryType.Error);
            }
        }

        protected override void OnShutdown()
        {
            eventLog1.WriteEntry("Service Stopping due to shutdown");

            try
            {
                Globals.ReaderInterface.Stop();
                Globals.ReaderInterface = null; //will create a new instance when this service is restarted.

                eventLog1.WriteEntry("Service Stopped");
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
                eventLog1.WriteEntry("Service Failed To Stop", EventLogEntryType.Error);
            }

            base.OnShutdown();
        }

        private static void ConfigureLog4Net()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            FileInfo fi = new FileInfo(baseDir + "log4net.config");
            try
            {
                using (var fs = fi.OpenRead())
                {
                }
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine(string.Format("Did not load log4net.config file. It should be found in the install directory. {0}", e.Message));
                return;
            }
            XmlConfigurator.Configure(fi);

            ILoggerRepository repo = LogManager.GetRepository();
            foreach (log4net.Appender.IAppender appender in repo.GetAppenders())
            {
                if (appender.Name.CompareTo("RollingFileAppender") == 0 && appender is RollingFileAppender)
                {
                    var appndr = appender as RollingFileAppender;
                    string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    string logPath = appData + @"\Omni-id\RFIDService\Logs\RFIDService.txt";
                    appndr.File = logPath;
                    appndr.ActivateOptions();
                }
            }
        }
         
    }
}

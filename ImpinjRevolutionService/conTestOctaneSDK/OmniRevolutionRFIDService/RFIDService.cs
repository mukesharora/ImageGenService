using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using log4net;
using Octane2ReaderBLL;

namespace OmniRevolutionRFIDService
{
    public partial class RFIDService : ServiceBase
    {
        private const string EVENT_LOG_SOURCE_NAME = "OmniRFIDRevEvtLogSource";
        private const string EVENT_LOG_NAME = "OmniRFIDRevolutionSvc";
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private RFIDWebApiService.RFIDWebApiService service;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RFIDService));

#if DEBUG2
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

        public RFIDService()
        {
            InitializeComponent();

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
            _timer.Interval = 5000;
            _timer.Start();
        }

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
           // Globals.DBPath = @"C:\Workshop\February2013\ImpinjOctane2SDKPrototypes\conTestOctaneSDK\ImpinjReadersConfiguration.s3db";
            Globals.DBPath = Path.Combine(Directory.GetCurrentDirectory(), "ImpinjReadersConfiguration.s3db");
            Globals.TagReportSinkInterface = new TagReportSink();
            Globals.GPIReportSinkInterface = new GPIReportSink();
            Globals.ReaderEventSinkInterface = new ReaderEventSink();
            Globals.SystemExceptionReportSinkInterface = new SystemExceptionSink();

			var appSettings = System.Configuration.ConfigurationManager.AppSettings;

			if(appSettings.AllKeys.Contains("IPC_Port") &&
				appSettings.AllKeys.Contains("IPC_Name"))
			{
				int port = int.Parse(appSettings["IPC_Port"]);
				string name = appSettings["IPC_Name"];

				Globals.InitSignalRServer(port, name);
			}
			else
			{
				Globals.InitSignalRServer(9000, "RevolutionService");
			}
        }
       
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Service Starting");

            try
            {
                //Instantiate impinj interface:
                Globals.ReaderInterface = new SimpleReaderInterface(Globals.DBPath, Globals.TagReportSinkInterface,
                                                Globals.GPIReportSinkInterface, Globals.ReaderEventSinkInterface,
                                                Globals.SystemExceptionReportSinkInterface);
                Globals.ReaderInterface.Start();

                string serviceHostName = "localhost:4000";
                try
                {
                    serviceHostName = ConfigurationManager.AppSettings["RFIDWebApiServiceHostName"];
                }
                catch (Exception)
                {
                    Logger.Error(string.Format("could not obtain hostname for RFIDWebApiService. Please define RFIDWebApiServiceHostName in app.config. Using default of {0}", serviceHostName));
                }
                service = new RFIDWebApiService.RFIDWebApiService();
                service.Start(serviceHostName);

                eventLog1.WriteEntry("Service Started");
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
                eventLog1.WriteEntry("Service Failed To Start", EventLogEntryType.Error);
                this.Stop();
            }
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Service Stopping");

            try
            {
                Globals.ReaderInterface.Stop();
                Globals.ReaderInterface = null; //will create a new instance when this service is restarted.

				Globals.StopSignalRServer();

                service.Stop();

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

				Globals.StopSignalRServer();

                eventLog1.WriteEntry("Service Stopped");
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry(ex.Message, EventLogEntryType.Error);
                eventLog1.WriteEntry("Service Failed To Stop", EventLogEntryType.Error);
            }

            base.OnShutdown();
        }
    }
}

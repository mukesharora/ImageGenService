using System;
using System.IO;
using System.Linq;
using Impinj.OctaneSdk;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System.Threading;

namespace ReaderApi.Reader
{
    public class RFIDReader
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ImpinjReader));
        private ImpinjReader _reader;

        public RFIDReader()
        {
            _reader = new ImpinjReader();
        }
        
        public void ReportStrongestTagPerAntenna(string ip)
        {
            Logger.Info(string.Format("Connecting to reader with IP {0}", ip));
            try
            {

                _reader.Connect(ip);

                Settings settings = _reader.QueryDefaultSettings();

                // Tell the reader to include the antenna number,
                // timestamps and tag seen count in all tag reports. 
                settings.Report.IncludeAntennaPortNumber = true;
                settings.Report.IncludeFirstSeenTime = true;
                settings.Report.IncludeLastSeenTime = true;
                settings.Report.IncludeSeenCount = true;

                // Send a tag report every time the reader stops (period is over).
                settings.Report.Mode = ReportMode.BatchAfterStop;

                settings.Antennas[0].TxPowerInDbm = 15;
                settings.Antennas[1].TxPowerInDbm = 20;

                // Apply the newly modified settings.
                _reader.ApplySettings(settings);

                _reader.TagsReported += OnTagsReported;

                _reader.Start();

                Thread.Sleep(1000);

                _reader.Stop();
            }
            catch (OctaneSdkException e)
            {                
                Logger.Error("Octane SDK exception: {0}", e);
            }
            catch (Exception e)
            {
                // Handle other .NET errors.
                Logger.Error("Exception : {0}", e);
            }
        }

        private void OnTagsReported(ImpinjReader sender, TagReport report)
        {
            // This event handler is called asynchronously 
            // when tag reports are available.
            // Loop through each tag in the report 
            // and print the data.
            foreach (Tag tag in report)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}", 
                   tag.AntennaPortNumber, tag.Epc, tag.FirstSeenTime, 
                   tag.LastSeenTime, tag.TagSeenCount);
            }
        }

        public static void ConfigureLog4Net()
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
                MessageLog.Instance.Error(string.Format("Did not load log4net.config file. It should be found in the install directory. {0}", e.Message));
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
                    string logPath = appData + @"\Omni-id\ReaderApi\Logs\OmniReaderApi.txt";
                    appndr.File = logPath;
                    appndr.ActivateOptions();
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Impinj.OctaneSdk;
using ReaderApi.Config;
using ReaderApi.Error;
using ReaderApi.Model;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;

namespace ReaderApi.Reader
{
    public class RFIDReader
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RFIDReader));
        private ImpinjReader _reader;
        private ReaderConfig _readerConfig;

        public delegate void RFIDTagsReportedHandler(RFIDReader reader, List<RFIDTag> tags);
        public event RFIDTagsReportedHandler RFIDTagsReported;

        public RFIDReader(string readerId)
        {
            _readerConfig = ReaderListDeserializer.LoadReaderConfig(readerId);
            _reader = new ImpinjReader(_readerConfig.HostName, _readerConfig.ReaderID);
        }

        public void ReportStrongestTag(int enabledAntennaPort)
        {            
            ReportStrongestTagPerAntenna(--enabledAntennaPort);
        }

        public void ReportStrongestTagPerAntenna()
        {
            int enabledAntennaPort = -1;
            ReportStrongestTagPerAntenna(enabledAntennaPort);
        }

        private void ReportStrongestTagPerAntenna(int enabledAntennaPort)
        {
            // start config time
            // please note: from start to end config time it takes about 500 ms.
            // We can configure the reader ahead of time if desired to save time.
            // Then we'll need to maintain the connection to the reader with the keep alives.
            Logger.Info(string.Format("Connecting to reader with ID {0}", _readerConfig.ReaderID));
            try
            {
                _reader.ConnectTimeout = 2000;
                _reader.Connect();
                
                Settings settings = _reader.QueryDefaultSettings();

                // Tell the reader to include the antenna number,
                // timestamps and tag seen count in all tag reports. 
                settings.Report.IncludeAntennaPortNumber = true;
                settings.Report.IncludeFirstSeenTime = true;
                settings.Report.IncludeLastSeenTime = true;
                settings.Report.IncludeSeenCount = true;

                // Send a tag report every time the reader stops (period is over).
                settings.Report.Mode = ReportMode.BatchAfterStop;

                for (int antennaPort = 0; antennaPort < _readerConfig.AntennaPowers.Count(); antennaPort++)
                {
                    settings.Antennas[antennaPort].TxPowerInDbm = _readerConfig.AntennaPowers[antennaPort];                  
                }
                DisableUnusedAntennaPorts(enabledAntennaPort, settings, _readerConfig.AntennaPowers.Count());

                // Apply the newly modified settings.
                _reader.ApplySettings(settings);

                _reader.TagsReported += OnTagsReported;

                // end config time

                _reader.Start();
                
                Thread.Sleep(_readerConfig.ReadTimeInMs);

                _reader.Stop();                
                _reader.Disconnect();
            }
            catch (OctaneSdkException e)
            { 
                Logger.Error("Octane SDK exception: {0}", e);
                if (e.Message.Contains("Timeout"))
                {
                    throw new ConnectionException(e.Message);
                }
            }
            catch (Exception e)
            {
                // Handle other .NET errors.
                Logger.Error("Exception : {0}", e);
                throw e;
            }
        }
  
        private void DisableUnusedAntennaPorts(int enabledAntennaPort, Settings readerSettings, int numAntennas)
        {
            if (enabledAntennaPort >= 0)
            {
                for (int i = 0; i < numAntennas; i++)
                {
                    readerSettings.Antennas[i].IsEnabled = false;
                }
                readerSettings.Antennas[enabledAntennaPort].IsEnabled = true;
            }
        }

        private void OnTagsReported(ImpinjReader sender, TagReport report)
        { 
            // When call ReportStrongestTagPerAntenna() this event handler is called once.
            // This event handler is called after _reader.Stop() is called.
            // All tags read while the reader was on are reported.
            var tagGroups = from t in report.Tags
                            group t by t.AntennaPortNumber
                            into t1 select new { Antenna = t1.Key, Tags = t1 };
            List<RFIDTag> strongestTags = new List<RFIDTag>();            
            foreach (var tg in tagGroups)
            {
                Tag strongestTag = (from t in tg.Tags
                                    orderby t.TagSeenCount descending, (t.LastSeenTime.Utc - t.FirstSeenTime.Utc) descending
                                    select t).FirstOrDefault();
                //var taggy = from t in tg.Tags
                //            orderby t.TagSeenCount descending, (t.LastSeenTime.Utc - t.FirstSeenTime.Utc) descending
                //            select new { Epc = t.Epc, ReadingTime = t.LastSeenTime.Utc - t.FirstSeenTime.Utc, SeenCount = t.TagSeenCount };
                RFIDTag rfidTag = new RFIDTag();
                rfidTag.CopyFrom(strongestTag);
                strongestTags.Add(rfidTag);

                Logger.Info(string.Format("Strongest tag Epc {0}. Antenna Port {1}", strongestTag.Epc, strongestTag.AntennaPortNumber));
            }            
            OnRFIDTagsReported(strongestTags);
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

        private void OnRFIDTagsReported(List<RFIDTag> rfidTags)
        {
            if (RFIDTagsReported != null)
            {
                RFIDTagsReported(this, rfidTags);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impinj.OctaneSdk;
using Octane2ReaderConfigDAL;
using log4net;
using ATRemoteObjectsLib;

//configuration data access layer.
//Impinj reader SDK.
namespace Octane2ReaderBLL
{
	//TODO: This should probably be moved down to the OmniRFIDRevolutionService project
    public class DBReaderInterface : IReaderInterface
    {
        
        #region private class members
        
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DBReaderInterface));
        private string _dbFilePath = "";
        private ITagReportSink _tagReportSink = null;
        private IGPIReportSink _gpiReportSink = null;
        private IReaderEventReportSink _readerEventReportSink = null;
        private ISystemExceptionSink _readerExceptionSink = null;
        private static int opIdUser;
        static List<ImpinjReader> _readers = new List<ImpinjReader>();

        private bool _isStarted = false;

        #endregion

        #region top of file utility methods
        
        /// <summary>
        /// Create the entity framework connection string to configuration database
        /// </summary>
        /// <param name="dbPath"></param>
        /// <returns></returns>
        private string MakeEntityConnectionString(string dbPath)
        {
            EntityConnectionStringBuilder conn = new EntityConnectionStringBuilder();
            conn.ProviderConnectionString = @"data source=" + dbPath;
            conn.Metadata = "res://*/Octane2ReaderConfig.csdl|res://*/Octane2ReaderConfig.ssdl|res://*/Octane2ReaderConfig.msl";
            conn.Provider = "System.Data.SQLite";
        
            return conn.ToString();
        }

        #endregion
        
        #region Constructor
        
        public DBReaderInterface(string configDBFilePath,
            ITagReportSink tagSink, IGPIReportSink gpiSink, IReaderEventReportSink rdrEventSink, ISystemExceptionSink sysExceptionSink)
        {
            _dbFilePath = configDBFilePath;
            _tagReportSink = tagSink;
            _gpiReportSink = gpiSink;
            _readerEventReportSink = rdrEventSink;
            _readerExceptionSink = sysExceptionSink;
        }
        
        #endregion
        
        #region Start, Stop , Restart
        
        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            //1. Use the DAL to read in the list of reader objects including their configuration parameters to create.
            string connStr = this.MakeEntityConnectionString(_dbFilePath);
            
            //2. Create an Octane2 SDK reader object for each reader, configure each reader according to its
            //  configuration parameters, wire up event callbacks, and connect each reader object to its physical reader.

            using (ImpinjReadersConfigurationEntities context = new ImpinjReadersConfigurationEntities(connStr))
            {
                List<Reader> readerConfigs = context.Readers.ToList();
                
                foreach (Reader rdr in readerConfigs)
                {
                    ProcessReaderConfig(rdr);
                }
        
                context.SaveChanges();  //the ProcessreaderConfig method may have updated fields on the passed-in rdr object, 
                // such as status, etc.
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            // Stop all the readers and disconnect from them.
            foreach (ImpinjReader reader in _readers)
            {
                // TODO : Should we -= out all the event callback wire-ups? If not, will the reader objects still be GC'd?
                try
                {
                    // Stop reading.
                    reader.Stop();
                
                    // Disconnect from the reader.
                    reader.Disconnect();
                }
                catch (Exception ex)
                {
                    if (null != _readerExceptionSink)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("Exception stopping reader '{0}' Exception : {1}", reader.Address, ex.Message);
            
                        _readerExceptionSink.LogSystemException(sb.ToString());
                    }
                }
            }

            //Empty the list of readers: (can not do alter the _readers collection while in the above loop)
            _readers.Clear(); //TODO : Are all the reader instances going to be GC'd after this call?
        }
            
        public void Restart()
        {
            Stop();
            Start();
        }
            
        public void TryReconnectToDisconnectedReaders()
        {
            try
            {
                using (ImpinjReadersConfigurationEntities context = new ImpinjReadersConfigurationEntities(this.MakeEntityConnectionString(_dbFilePath)))
                {
                    List<Reader> readers = context.Readers.ToList();
                        
                    foreach (Reader r in readers)
                    {
                        if (r.CurrentStatus == "Disconnected")
                        {
                            foreach (ImpinjReader ir in _readers)
                            {
                                if (ir.Address == r.HostName)
                                {
                                    try
                                    {
                                        ir.Connect();
                                        Logger.Info(string.Format("Successfully reconnected to reader {0}", ir.Address));
                                        r.CurrentStatus = "Connected";
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(string.Format("Failed to reconnected to reader {0}", ir.Address));
                                    }
                                }
                            }
                        }
                    }
            
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        #endregion
            
        #region Private utility methods
            
        private void ProcessReaderConfig(Reader rdr)
        {
            ImpinjReader newReader = null;  //defined outside the try block in case want to use in an exception handling block.
            try
            {
                newReader = new ImpinjReader(rdr.HostName, rdr.ReaderID);
                _readers.Add(newReader);
                
                newReader.Connect();
                
                //Settings settings = newReader.QueryDefaultSettings();
                Settings settings = Settings.Load("defaultReaderConfig.xml");         
                 //Create a tag read operation for User memory.
                TagReadOp readUser = new TagReadOp();
                // Read from user memory
                readUser.MemoryBank = MemoryBank.User;
                // Read two (16-bit) words
                readUser.WordCount = 2;
                // Starting at word 0
                readUser.WordPointer = 0;

                opIdUser = readUser.Id;

                settings.Report.OptimizedReadOps.Add(readUser);

                //settings.SearchMode = SearchMode.DualTarget;
                
                //settings.Report.IncludeAntennaPortNumber = true;
                
                //// Send a tag report for every tag read.
                //settings.Report.Mode = ReportMode.Individual;
                
                //// Start reading tags when GPI #1 goes high.
                //settings.Gpis.GetGpi(1).IsEnabled = true;
                //settings.Gpis.GetGpi(1).DebounceInMs = 50;
                settings.AutoStart.Mode = AutoStartMode.GpiTrigger;
                settings.AutoStart.GpiPortNumber = 1;
                settings.AutoStart.GpiLevel = true;
                
                //// Start reading tags when GPI #2 goes high.
                //settings.Gpis.GetGpi(2).IsEnabled = true;
                //settings.Gpis.GetGpi(2).DebounceInMs = 50;
                
                //// Start reading tags when GPI #3 goes high.
                //settings.Gpis.GetGpi(3).IsEnabled = true;
                //settings.Gpis.GetGpi(3).DebounceInMs = 50;

                // Stop reading tags when GPI #3 goes low.
                settings.AutoStop.Mode = AutoStopMode.Duration;
                settings.AutoStop.DurationInMs = 10000;
                
                //settings.Gpis.GetGpi(4).IsEnabled = true;
                //settings.Gpis.GetGpi(4).DebounceInMs = 50;
                
                //// Enable keepalives.
                //settings.Keepalives.Enabled = true;                
                //settings.Keepalives.PeriodInMs = 5000;

                //settings.Keepalives.EnableLinkMonitorMode = true;
                //settings.Keepalives.LinkDownThreshold = 5;

                //settings.Save("defaultReaderConfig.xml");

                //NOTE : It may only make sense to wire up certain event handlers if the corresponding setting is 'on'
                
                newReader.ApplySettings(settings);
                
                newReader.ConnectionLost += OnConnectionLost;
                newReader.GpiChanged += OnGpiChanged;                
                newReader.AntennaChanged += OnAntennaChanged;
                newReader.KeepaliveReceived += OnKeepaliveReceived;                
                newReader.TagOpComplete += new ImpinjReader.TagOpCompleteHandler(OnTagOpComplete);
                
                //NOTE : sending the Reader.Start command is sent here may be dependent on the reader's mode:
                // UPDATE : Yes, it appears that if we are in GPI trigger mode, we do NOT call Start() - this will start an ordinary inventory operation.
                // newReader.Start();
            
                rdr.CurrentStatus = "Connected";
                rdr.LastPing = DateTime.Now;
                Logger.Info(string.Format("Successfully connected to reader {0}", rdr.HostName));                
            }
            catch (OctaneSdkException e)
            {
                string msg = string.Format("Reader '{0}' Exception : {1}", rdr.HostName, e.Message);
                if (null != _readerExceptionSink)
                {
                    _readerExceptionSink.LogSystemException(msg);
                }
                Logger.Error(msg, e);
            
                rdr.CurrentStatus = "Error";
                rdr.LastPing = DateTime.Now;
            }
            catch (Exception e)
            {
                string msg = string.Format("Reader '{0}' Exception : {1}", rdr.HostName, e.Message);
                if (null != _readerExceptionSink)
                {
                    _readerExceptionSink.LogSystemException(msg);
                }
                Logger.Error(msg, e);

                rdr.CurrentStatus = "Error";
                rdr.LastPing = DateTime.Now;
            }
        }
        
        #endregion
            
        #region Reader event handlers
                
        private void OnTagOpComplete(ImpinjReader reader, TagOpReport results)
        {
                try
                {
                    string userData, tidData, epcData;

                    userData = tidData = epcData = "";

                    // Loop through all the completed tag operations
                    foreach (TagOpResult result in results)
                    {
                        // Was this completed operation a tag read operation?
                        if (result is TagReadOpResult)
                        {
                            // Cast it to the correct type.
                            TagReadOpResult readResult = result as TagReadOpResult;

                            // Save the EPC
                            epcData = readResult.Tag.Epc.ToHexString();

                            // Are these the results for User memory or TID?
                            if (readResult.OpId == opIdUser)
                                userData = readResult.Data.ToHexString();

                            if (null != _tagReportSink)
                            {
                                Logger.Debug(string.Format("Tag read. Name {0}, Address {1}, EPC {2}, Antenna {3}, User Data {4}", reader.Name, reader.Address, result.Tag.Epc.ToString(), result.Tag.AntennaPortNumber, userData));
                                _tagReportSink.ReportTagRead(reader.Name, reader.Address, result.Tag.Epc.ToString(), result.Tag.AntennaPortNumber, userData);
                            }
                        }
                    }
                }
                catch (OctaneSdkException octaneEx)
                {
                    ;
                }
                catch (Exception ex)
                {
                    ;
                }
        }
            
        public void OnConnectionLost(ImpinjReader reader)
        {
            Logger.Warn(string.Format("Connection to reader {0} was lost", reader.Address));

            if (null != _readerEventReportSink)
            {
                _readerEventReportSink.ReportReaderEvent(reader.Name, "Connection Lost");
            }
                
            //Update the status of the reader in the EF database:
            try
            {
                using (ImpinjReadersConfigurationEntities context = new ImpinjReadersConfigurationEntities(this.MakeEntityConnectionString(_dbFilePath)))
                {
                    List<Reader> readers = context.Readers.ToList();
                            
                    foreach (Reader r in readers)
                    {
                        if (r.HostName == reader.Address)
                        {
                            r.CurrentStatus = "Disconnected";
                            r.LastPing = DateTime.Now;
                            break;
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (null != _readerExceptionSink)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Exception occurred updated connection status of Reader '{0}' : Msg: {1}", reader.Address, ex.Message);
                    _readerExceptionSink.LogSystemException(sb.ToString());
                }
            }
        }
                
        void OnKeepaliveReceived(ImpinjReader reader)
        {
            if (null != _readerEventReportSink)
            {
                Logger.Debug(string.Format("Keep alive received {0}", reader.Address));
                _readerEventReportSink.ReportReaderEvent(reader.Address, "Keepalive Received");
            }
        }
                
        void OnGpiChanged(ImpinjReader reader, GpiEvent e)
        {
            if (null != _gpiReportSink)
            {
                _gpiReportSink.ReportGPIEvent(reader.Name, e.PortNumber, e.State);
            }
        }
        
        void OnAntennaChanged(ImpinjReader reader, AntennaEvent e)
        {
            // throw new NotImplementedException();
            //TODO
        }

		public void OnCommandReceived(ReaderCommand command)
		{

		}
                
        #endregion


		public List<ImpinjReader> GetReaders()
		{
			//Get list of readers from db? Not sure if in-memory list is maintained
			throw new NotImplementedException();
		}

		public ImpinjReader GetReader(string deviceID)
		{
			//Get reader from db? Not sure if in-memory list is maintained
			throw new NotImplementedException();
		}

		public void SetGPO(ImpinjReader reader, int port, bool portState)
		{
			throw new NotImplementedException();
		}

		public void StartStopReader(ImpinjReader reader, bool start)
		{
			throw new NotImplementedException();
		}
	}
}
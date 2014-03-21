using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.IO;
using System.Linq;
using System.Text;
using ATRemoteObjectsLib;
using Impinj.OctaneSdk; //Impinj impinj SDK.
using Octane2ReaderBLL;

namespace OmniRevolutionRFIDService
{
    public class SimpleReaderInterface : IReaderInterface
    {
        #region private class members
        private string _dbFilePath = "";
        private ITagReportSink _tagReportSink = null;
        private IGPIReportSink _gpiReportSink = null;
        private IReaderEventReportSink _readerEventReportSink = null;
        private ISystemExceptionSink _readerExceptionSink = null;

        private static List<ImpinjReader> _readers = new List<ImpinjReader>();
        private static List<bool> _readersConnected = new List<bool>();
		private static Dictionary<ImpinjReader, bool> _readersMap = new Dictionary<ImpinjReader, bool>();

        private bool _isStarted = false;

        static int opIdUser, opIdTid;

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

        #region P/Invoke Code Declaration for GetModuleFileName
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity]
        private static extern uint GetModuleFileName(
        [System.Runtime.InteropServices.In]
        IntPtr hModule,
        [System.Runtime.InteropServices.Out]
        System.Text.StringBuilder lpFilename,
        [System.Runtime.InteropServices.In]
        [System.Runtime.InteropServices.MarshalAs(
        System.Runtime.InteropServices.UnmanagedType.U4)]
        int nSize
        );
        #endregion


        #region Constructor
        public SimpleReaderInterface(string configDBFilePath, 
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
            List<Reader> readerConfigs = LoadReaderConfigs();

            foreach(Reader rdr in readerConfigs)
            {
                bool connected = ProcessReaderConfig(rdr);
				//_readersConnected.Add(connected);
            }
        }

        private List<Reader> LoadReaderConfigs()
        {
            string appDirectory = "";

            System.Text.StringBuilder sb = new System.Text.StringBuilder(260);
            if (GetModuleFileName(System.IntPtr.Zero, sb, sb.Capacity) == 0)
            {
                throw new Exception("Reader API Unable to determine application directory path");
            }
            else
            {
                appDirectory = System.IO.Path.GetDirectoryName(sb.ToString());
            }

            ReaderListDeserializer rld = new ReaderListDeserializer();

            List<Reader> readerConfigs = rld.LoadReaderData(Path.Combine(appDirectory, "ReaderList.txt"));
            return readerConfigs;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            // Stop all the readers and disconnect from them.
            foreach (ImpinjReader reader in _readersMap.Keys)
            {
                // TODO : Should we -= out all the event callback wire-ups? If not, will the impinj objects still be GC'd?
                try
                {
                    // Stop reading.
                    reader.Stop();

                    // Disconnect from the impinj.
                    reader.Disconnect();
                }
                catch (Exception ex)
                {
                    if (null != _readerExceptionSink)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("Exception stopping impinj '{0}' Exception : {1}", reader.Address, ex.Message);

                        _readerExceptionSink.LogSystemException(sb.ToString());
                    }
                }
            }

            //Empty the list of readers: (can not do alter the _readers collection while in the above loop)
			//_readers.Clear(); //TODO : Are all the impinj instances going to be GC'd after this call?
			//_readersConnected.Clear();
			_readersMap.Clear();
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
				foreach(KeyValuePair<ImpinjReader, bool> pair in _readersMap.ToArray())
				{
					if(!pair.Value)
					{
						try
						{
							ImpinjReader reader = pair.Key;
							reader.ConnectTimeout = 2000;
							reader.Connect();

                            List<Reader> readerConfigs = LoadReaderConfigs();
                            Reader rdr = readerConfigs.Where(r => r.ReaderID == reader.Name).Single();

                            SendReaderConfig(reader, rdr);
							_readersMap[reader] = true;
						}
						catch(Exception)
						{ }
					}
				}
            }
            catch (Exception ex)
            {
                ;
            }
        }
        #endregion

		#region Public utility methods

		public List<ImpinjReader> GetReaders()
		{
			return _readersMap.Keys.ToList();
		}

		public ImpinjReader GetReader(string deviceID)
		{
			foreach(ImpinjReader reader in _readersMap.Keys)
			{
				if(reader.Name.Equals(deviceID))
				{
					return reader;
				}
			}

			return null;
		}

		public void SetGPO(ImpinjReader reader, int port, bool portState)
		{
			if(_readersMap[reader])
			{
				reader.SetGpo((ushort)port, portState);
			}
			else
			{
				throw new InvalidOperationException("Reader disconnected, cannot set GPO port state.");
			}
		}

		public void StartStopReader(ImpinjReader reader, bool start)
		{
			if(_readersMap[reader])
			{
				if(start)
				{
					if(!reader.QueryStatus().IsSingulating)
					{
						reader.Start();
					}
				}
				else
				{
					if(reader.QueryStatus().IsSingulating)
					{
						reader.Stop();
					}
				}
			}
			else
			{
				throw new InvalidOperationException("Reader disconnected, cannot start or stop the reader.");
			}
		}

		#endregion

		#region Private utility methods
		/// <summary>
        /// 
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns>true if the successfully connected to the impinj otherwise, false.</returns>
        private bool ProcessReaderConfig(Reader rdr)
        {
            ImpinjReader newReader = null;  //defined outside the try block in case want to use in an exception handling block.
            try
            {
                newReader = new ImpinjReader(rdr.HostName, rdr.ReaderID);
				//_readers.Add(newReader);
				_readersMap.Add(newReader, false);

                newReader.ConnectTimeout = 10000;   //give a generous 10 second timeout to connnect.
                newReader.Connect();

				SendReaderConfig(newReader, rdr);

                rdr.CurrentStatus = "Connected";
                rdr.LastPing = DateTime.Now;
				_readersMap[newReader] = true;

                return true;
            }
            catch (OctaneSdkException e)
            {
                if (null != _readerExceptionSink)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Reader '{0}' Exception : {1}", rdr.HostName , e.Message);

                    _readerExceptionSink.LogSystemException(sb.ToString());
                }

                rdr.CurrentStatus = "Error";
                rdr.LastPing = DateTime.Now;
            }
            catch (Exception e)
            {
                if (null != _readerExceptionSink)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Reader '{0}' Exception : {1}", e.Message);

                    _readerExceptionSink.LogSystemException(sb.ToString());
                }

                rdr.CurrentStatus = "Error";
                rdr.LastPing = DateTime.Now;
            }
            return false;
        }

        private void SendReaderConfig(ImpinjReader reader, Reader rdr)
		{
			Settings settings = reader.QueryDefaultSettings();

			// Create a tag read operation for User memory.
			TagReadOp readUser = new TagReadOp();
			// Read from user memory
			readUser.MemoryBank = MemoryBank.User;
			// Read two (16-bit) words
			readUser.WordCount = 2;
			// Starting at word 0
			readUser.WordPointer = 0;

			opIdUser = readUser.Id;

			settings.Report.OptimizedReadOps.Add(readUser);

			//MJD : New in response to request from Sandesh Bala from Bridgestone : only report tags ONCE per 
			// impinj reading session - the following two settings accomplish this (not tested yet)
			settings.SearchMode = SearchMode.DualTarget;

			//TODO : Here is where to check the properties of rdr, and use these values
			// to set corresponding settings on the newReader, such as KeepAlives, etc.
			settings.Report.IncludeAntennaPortNumber = true;

			// Send a tag report for every tag read.
			settings.Report.Mode = ReportMode.Individual;

			// Start reading tags when GPI #1 goes high.
			settings.Gpis.GetGpi(1).IsEnabled = true;
			settings.Gpis.GetGpi(1).DebounceInMs = 500;
			            
            if(string.Equals(rdr.ReaderMode, Reader.SWITCH_READERMODE, StringComparison.InvariantCultureIgnoreCase))
            {
                settings.AutoStop.Mode = AutoStopMode.Duration;
			    settings.AutoStop.DurationInMs = 10000;
                settings.AutoStart.Mode = AutoStartMode.GpiTrigger;
                settings.AutoStart.GpiPortNumber = 1;
                settings.AutoStart.GpiLevel = true;
            }
            else if (string.Equals(rdr.ReaderMode, Reader.LDC_READERMODE, StringComparison.InvariantCultureIgnoreCase))
            {
                settings.LowDutyCycle.IsEnabled = true;
                settings.LowDutyCycle.EmptyFieldTimeoutInMs = 1000;
                settings.LowDutyCycle.FieldPingIntervalInMs = 1000;
            }

            for (int i = 0; i < rdr.AntennaPowers.Count(); i++)
            {
                settings.Antennas.GetAntenna((ushort)(i + 1)).TxPowerInDbm = rdr.AntennaPowers[i];
            }

			// Start reading tags when GPI #2 goes high.
			settings.Gpis.GetGpi(2).IsEnabled = true;
			settings.Gpis.GetGpi(2).DebounceInMs = 50;

			// Start reading tags when GPI #3 goes high.
			settings.Gpis.GetGpi(3).IsEnabled = true;
			settings.Gpis.GetGpi(3).DebounceInMs = 50;

			settings.Gpis.GetGpi(4).IsEnabled = true;
			settings.Gpis.GetGpi(4).DebounceInMs = 50;

			// Enable keepalives.
			settings.Keepalives.Enabled = true;
			settings.Keepalives.PeriodInMs = 3000;
			settings.Keepalives.EnableLinkMonitorMode = true;
			settings.Keepalives.LinkDownThreshold = 5;

			reader.ApplySettings(settings);
			RegisterEventHandlers(reader);
		}

		void RegisterEventHandlers(ImpinjReader reader)
		{
			//Remove the event handlers first to ensure that we don't
			//subscribe multiple times to the same event
			reader.ConnectionLost -= OnConnectionLost;
			reader.ConnectionLost += OnConnectionLost;

			reader.GpiChanged -= OnGpiChanged;
			reader.GpiChanged += OnGpiChanged;

			reader.AntennaChanged -= OnAntennaChanged;
			reader.AntennaChanged += OnAntennaChanged;

			reader.KeepaliveReceived -= OnKeepaliveReceived;
			reader.KeepaliveReceived += OnKeepaliveReceived;

			reader.TagOpComplete -= new ImpinjReader.TagOpCompleteHandler(newReader_TagOpComplete);
			reader.TagOpComplete += new ImpinjReader.TagOpCompleteHandler(newReader_TagOpComplete);
		}

        #endregion

        #region Reader event handlers

		void newReader_TagOpComplete(ImpinjReader reader, TagOpReport results)
		{
			try
			{
				string userData, tidData, epcData;

				userData = tidData = epcData = "";

				// Loop through all the completed tag operations
				foreach(TagOpResult result in results)
				{
					// Was this completed operation a tag read operation?
					if(result is TagReadOpResult)
					{
						// Cast it to the correct type.
						TagReadOpResult readResult = result as TagReadOpResult;

						// Save the EPC
						epcData = readResult.Tag.Epc.ToHexString();

						// Are these the results for User memory or TID?
						if(readResult.OpId == opIdUser)
							userData = readResult.Data.ToHexString();

						if(null != _tagReportSink)
						{
							if(result.Tag.IsAntennaPortNumberPresent == false)
							{
								_tagReportSink.ReportTagRead(reader.Name, reader.Address, result.Tag.Epc.ToString(), -1, userData);
							}
							else
							{
								_tagReportSink.ReportTagRead(reader.Name, reader.Address, result.Tag.Epc.ToString(), result.Tag.AntennaPortNumber, userData);
							}
						}

					}
				}

			}
			catch(OctaneSdkException octaneEx)
			{
				;
			}
			catch(Exception ex)
			{
				;
			}
		}

        public void OnConnectionLost(ImpinjReader reader)
        {
			//int index = _readers.IndexOf(reader);
			//_readersConnected[index] = false;
			_readersMap[reader] = false;

            if (null != _readerEventReportSink)
            {
                _readerEventReportSink.ReportReaderEvent(reader.Name, "Connection Lost");
            }
        }

        void OnKeepaliveReceived(ImpinjReader reader)
        {
			_readersMap[reader] = true;

            if (null != _readerEventReportSink)
            {
                _readerEventReportSink.ReportReaderEvent(reader.Address, "Keepalive Received","KEEPALIVE");
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

        public void OnTagsReported(ImpinjReader sender, TagReport report)
        {
            foreach (Tag tag in report)
            {
                if (null != _tagReportSink)
                {
                    if (tag.IsAntennaPortNumberPresent == false)
                    {
                        _tagReportSink.ReportTagRead(sender.Name, sender.Address, tag.Epc.ToString());
                    }
                    else
                    {
                        _tagReportSink.ReportTagRead(sender.Name, sender.Address, tag.Epc.ToString(),tag.AntennaPortNumber);
                    }
                }
            }
        }

		public void OnCommandReceived(ReaderCommand command)
		{
			ImpinjReader reader = GetReader(command.RFIDDeviceID);

			if(reader == null)
			{
				return;//The impinj does not exist
			}

			switch(command.CommandType)
			{
				case ReaderCommand.ReaderCommandType.GPOChangeCommand:	
					SetGPO(reader, command.GPOPort, command.GPOPortState);

					break;
				case ReaderCommand.ReaderCommandType.ReaderChangeCommand:
					StartStopReader(reader, command.RFIDDeviceState);

					break;
				default:
					break;
			}
		}

        #endregion
	}
}

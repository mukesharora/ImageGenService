using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Middleware.client;
using Middleware.client.messages;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;
using Middleware.client.commands;
using ImageGenModels;
using BridgestoneSimulator.Properties;
using System.Collections.Specialized;

namespace BridgestoneSimulator
{

    /// <summary>
    /// This class uses two tables that are in the app.config file.
    /// RackTable - Maps EPC codes of Visual tags to the UID
    /// EPC,UID
    /// 
    /// StationTable
    /// StationName,ReaderGUID,Antenna
    /// 
    /// </summary>
    

    // Enum specifying if a rack is in the workstation or out of the workstation.
    public enum WorkstationStatusEnum { RACK_IN, RACK_OUT }

    public class MiddlewareHelper : INotifyPropertyChanged
    {
        #region Member variables

        /// <summary>
        /// Middleware
        /// </summary>
        private OmniMiddlewareClient _middlewareClient  = null;

        /// <summary>
        /// The event messages (appended to each other) for logging
        /// </summary>
        private string _eventMessages;

        /// <summary>
        /// Current rack workstation status (in/out)
        /// </summary>
        private WorkstationStatusEnum _workstationStatus = WorkstationStatusEnum.RACK_OUT;

        /// <summary>
        /// UID of rack in workstation.
        /// </summary>
        private string _workstationRack = null;

        /// <summary>
        /// Timer for when the rack is in transition (beam broken, reader on)
        /// </summary>
        private DispatcherTimer _transitionTimer = null;

        /// <summary>
        /// Duration in seconds to ignore RFID reads
        /// </summary>
        private int _TransitionDuration;

        /// <summary>
        /// Name of the Workstation. This app
        /// only recognizes one workstation at this time.
        /// It will use the 1st workstation from the StationTable
        /// in the App.config file.
        /// </summary>
        private string _workstationName;

        /// <summary>
        /// RackTable in App.config file.
        /// In form: EPC,UID
        /// Maps EPC codes of Visual tags to the UID
        /// </summary>
        StringCollection _rackTable = null;

        /// <summary>
        /// StationTable in App.config file
        /// In form: StationName,ReaderGUID,Antenna
        /// </summary>
        StringCollection _stationTable = null;

        #endregion

        #region ctor

        public MiddlewareHelper()
        {
            _transitionTimer = new DispatcherTimer();
            _transitionTimer.Stop();
            _transitionTimer.Tick += new EventHandler(_transitionTimer_Tick);
            
            TransitionDuration = 15;
            _transitionTimer.Interval = new TimeSpan(0, 0, TransitionDuration);
        }

        #endregion

        #region Public Methods

        public bool Initialize()
        {
            bool ok = false;

            try
            {
                // Get service addresses from App.Config
                //

                string calcManAddress = Settings.Default.CALCManAddress;
                if (string.IsNullOrEmpty(calcManAddress))
                {
                    // default value
                    calcManAddress = "localhost:3000";
                }

                string imageGenAddress = Settings.Default.ImageGenAddress;
                if (string.IsNullOrEmpty(imageGenAddress))
                {
                    // default value
                    imageGenAddress = "localhost:30525";
                }

                string rfidReaderAddress = Settings.Default.RFIDReaderAddress;
                if (string.IsNullOrEmpty(rfidReaderAddress))
                {
                    // default value
                    rfidReaderAddress = "localhost:4000";
                }

                // Create the MiddlewareClient
                //
                _middlewareClient = new OmniMiddlewareClient(calcManAddress, imageGenAddress, rfidReaderAddress);
                _middlewareClient.OmniIDMiddlewareEvent += new OmniMiddlewareClient.OmniAPIEventCallbackHandler(_middlewareClient_OmniIDMiddlewareEvent);

                // Read in Rack and Station tables.
                //
                ok = ReadTables();

                // Set the current workstation
                //
                if (ok)
                {
                    SetStationName();
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex, "MiddlewareHelper::Initialize()");
                AddEventMessage(string.Format("Unexpected exception in MiddlewareHelper::Initialize. {0}", ex.ToString()));
            }

            return ok;
        }

        /// <summary>
        /// Close the Middleware client. This ensures that the PrintQueues.txt file
        /// unclaims the message queue.
        /// </summary>
        public void Close()
        {
            try
            {
                if (_middlewareClient != null)
                {
                    _middlewareClient.Dispose();
                    _middlewareClient = null;
                }

                StopTransitionTimer();                
            }
            catch (Exception ex)
            {
                App.LogException(ex, "MiddlewareHelper::Close()");
                AddEventMessage(string.Format("Unexpected exception in MiddlewareHelper::Close. {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Resets the workstation status to out. Does not update Visual tag display.
        /// </summary>
        public void ResetWorkstation()
        {
            WorkstationStatus = WorkstationStatusEnum.RACK_OUT;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Duration that the rack is in transistion. It should
        /// be set longer then the Reader duration (in the OmniImpinj reader app)
        /// so that we don't don't look for RFID tag reads at this time.
        /// </summary>
        public int TransitionDuration
        {
            get { return _TransitionDuration; }
            set 
            {
                _TransitionDuration = value;
                NotifyPropertyChanged("TransitionDuration");
            }
        }

        /// <summary>
        /// Logging messages for the GUI
        /// </summary>
        public string EventMessages
        {
            get { return _eventMessages; }
            private set 
            { 
                _eventMessages = value;
                NotifyPropertyChanged("EventMessages");
            }
        }

        /// <summary>
        /// Rack IN / OUT status of the workstation
        /// </summary>
        public WorkstationStatusEnum WorkstationStatus
        {
            get { return _workstationStatus; }
            private set
            {
                _workstationStatus = value;
                NotifyPropertyChanged("WorkstationStatus");
            }
        }

        /// <summary>
        /// Current Workstation Rack (UID of rack in workstation)
        /// </summary>
        public string WorkstationRack
        {
            get { return _workstationRack; }
            private set
            {
                _workstationRack = value;
                NotifyPropertyChanged("WorkstationRack");
            }
        }

        /// <summary>
        /// Current workstation name (from StationsTable in App.Config)
        /// </summary>
        public string WorkstationName
        {
            get { return _workstationName; }
            set 
            {
                _workstationName = value;
                NotifyPropertyChanged("WorkstationName");
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handles for messages from Middleware.
        /// </summary>        
        void _middlewareClient_OmniIDMiddlewareEvent(OmniAPIMessage[] messages)
        {
            foreach (OmniAPIMessage message in messages)
            {
                try
                {
                    // This logs all events
                    //EventMessages = EventMessages + (string.Format("{0:D6}", _eventSequenceNumber) + "\t" +  message.ToString() + Environment.NewLine);
                    //_eventSequenceNumber++;

                    if ((message is OmniCommandErrorResultEvent) ||
                         (message is OmniSystemErrorEvent))
                    {
                        AddEventMessage(message.ToString() + Environment.NewLine);                        
                    }

                    if (message is OmniRFIDDetectionMessage)
                    {
                        // Check if rack is in transition.
                        //
                        if (InTransition)
                        {
                            string msg = "RFID read ignored (transition timer active)";
                            App.LogTrace(msg);
                            AddEventMessage(msg);
                        }
                        else
                        {
                            ProcessRfidRead(message as OmniRFIDDetectionMessage);
                        }
                    }

                    // No longer triggered by Visual tag announces.
                    //
                    //if (message is OmniVisualTagAnnounceEvent)
                    //{
                    //    VisualTagAnnounce(message as OmniVisualTagAnnounceEvent);
                    //}
                }
                catch (Exception ex)
                {
                    App.LogException(ex, "_middlewareClient_OmniIDMiddlewareEvent");
                    AddEventMessage(string.Format("Unexpected exception in _middlewareClient_OmniIDMiddlewareEvent. {0}", ex.ToString()));
                }
            }
        }

        /// <summary>
        /// Transition Timer elapsed event.
        /// </summary>
        void _transitionTimer_Tick(object sender, EventArgs e)
        {
            // Turn of the transition timer as we are no longer in transition.
            StopTransitionTimer();
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Is rack in transition?
        /// </summary>
        public bool InTransition 
        {
            get
            {
                return _transitionTimer.IsEnabled;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Read the "database" tables from the App.Config table
        /// </summary>
        /// <returns>True on success</returns>
        private bool ReadTables()
        {
            bool ok = false;

            try
            {
                _rackTable = Settings.Default.RackTable;
                _stationTable = Settings.Default.StationTable;
                ok = true;
            }
            catch (Exception ex)
            {
                App.LogException(ex, "ReadTables");
                AddEventMessage(string.Format("Unexpected exception in ReadTables. {0}", ex.ToString()));
            }

            return ok;
        }

        /// <summary>
        /// Sets the workstation name. Currently we just use the
        /// first workstation from the StationTable in App.config.
        /// </summary>
        private void SetStationName()
        {
            try
            {
                string row = _stationTable[0];
                string[] columns = row.Split(',');
                WorkstationName = columns[0];
            }
            catch (Exception ex)
            {
                App.LogException(ex, "SetStationName");
                AddEventMessage(string.Format("Unexpected exception in SetStationName. {0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Add an event message for logging
        /// </summary>        
        private void AddEventMessage(string message)
        {
            EventMessages = EventMessages + (DateTime.Now.ToShortTimeString() + "\t" + message.ToString() + Environment.NewLine);
        }

        /// <summary>
        /// Process an RFID read.
        /// This will result in a transition to "rack in" or a transition to "rack out" process.
        /// </summary>        
        private void ProcessRfidRead(OmniRFIDDetectionMessage rfidDetectionMsg)
        {
            // Find station of RFID read
            //
            string stationName = GetStationName(rfidDetectionMsg.RFIDDeviceID, rfidDetectionMsg.RFIDDeviceAntennaID);

            string msg = null;
            if (stationName == null)
            {
                // Station not found in StationsTable
                msg = string.Format("No workstation defined for RFID device ID / Antenna ID [{0} / {1}]", rfidDetectionMsg.RFIDDeviceID, rfidDetectionMsg.RFIDDeviceAntennaID);
                App.LogTrace(msg);
                AddEventMessage(msg);

            }
            else if (0 != string.Compare(stationName, WorkstationName, true))
            {
                // Station found in StationsTable, but is not the current workstation.
                msg = string.Format("Workstation {0} is not the current workstation", stationName);
                App.LogTrace(msg);
                AddEventMessage(msg);
            }
            else
            {
                // Get the UID of for the given EPC code.
                //
                string uid = GetUid(rfidDetectionMsg.RFIDTagEPC);
                if (uid != null)
                {
                    if (WorkstationStatus == WorkstationStatusEnum.RACK_OUT)
                    {
                        // Transitioning from RACK_OUT to RACK_IN
                        //
                        TransitionToRackIn(uid);
                    }
                    else
                    {
                        // Transitioning from RACK_IN to RACK_OUT
                        //
                        TransitionToRackOut(uid);
                    }
                }
                else
                {
                    // EPC not found in RackTable (uncommissioned).
                    msg = string.Format("Uncommission EPC (not in RackTable)", rfidDetectionMsg.RFIDTagEPC);
                    App.LogTrace(msg);
                    AddEventMessage(msg);
                }
            }
        }

        /// <summary>
        /// For a given EPC code, return the UID (lookup from RackTable)
        /// </summary>
        private string GetUid(string epc)
        {
            // RackTable format is:
            // EPC,UID

            string uid = null;
            try
            {
                foreach (string row in _rackTable)
                {
                    string[] columns = row.Split(',');
                    if ( 0 == string.Compare(columns[0], epc, true) )
                    {
                        uid = columns[1];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex, "GetUid");
                AddEventMessage(string.Format("Unexpected exception in GetUid. {0}", ex.ToString()));
            }

            return uid;
        }

        /// <summary>
        /// Look up the workstation name for the given RFID device id / antenna id
        /// </summary>
        private string GetStationName(string rfidDeviceId, int antennaId)
        {
            // Station format is:
            // StationName,ReaderGUID,Antenna

            string stationName = null;

            try
            {
                foreach (string row in _stationTable)
                {
                    string[] columns = row.Split(',');
                    if ( (0 == string.Compare(columns[1], rfidDeviceId, true)) &&
                         (0 == string.Compare(columns[2], antennaId.ToString(), true)))
                    {
                        stationName = columns[0];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex, "GetStationName");
                AddEventMessage(string.Format("Unexpected exception in GetStationName. {0}", ex.ToString()));
            }

            return stationName;
        }

        /// <summary>
        /// Transition rack from IN to OUT.
        /// </summary>        
        private void TransitionToRackOut(string uid)
        {
            string msg = string.Format("Transition to RACK_OUT. UID={0}", uid);
            App.LogTrace(msg);
            AddEventMessage(msg);

            if (WorkstationStatus != WorkstationStatusEnum.RACK_IN)
            {
                // Precondition. WorkstationStatus must be RACK_IN
                //
                msg = "Precondition violation. WorkstationStatus must be RACK_IN";
                App.LogError("TransitionToRackOut", msg);
                AddEventMessage(msg);
            }
            else if (uid != WorkstationRack)
            {
                // Precondition, the UID of the rack going out must match the UID of the rack in the workstation
                //
                msg = "Precondition violation. UID of rack in worksatation must equal UID of rack exiting workstation";
                App.LogError("TransitionToRackOut", msg);
                AddEventMessage(msg);
            }
            else
            {
                // Do transition from IN to OUT
                //
                WorkstationRack = "";
                WorkstationStatus = WorkstationStatusEnum.RACK_OUT;
                StartTransitionTimer();
                UpdateVisualTag(uid);
            }
        }

        /// <summary>
        /// Transition rack from OUT to IN.
        /// </summary>    
        private void TransitionToRackIn(string uid)
        {
            string msg = string.Format("Transition to RACK_IN. UID={0}", uid);
            App.LogTrace(msg);
            AddEventMessage(msg);

            if (WorkstationStatus != WorkstationStatusEnum.RACK_OUT)
            {
                // Precondition. WorkstationStatus must be RACK_OUT
                //
                App.LogError("TransitionToRackIn", "Precondition violation. WorkstationStatus must be RACK_OUT");
                AddEventMessage("Precondition violation. WorkstationStatus must be RACK_OUT");
            }
            else
            {
                // Do transition from OUT to IN
                //
                WorkstationRack = uid;
                WorkstationStatus = WorkstationStatusEnum.RACK_IN;
                StartTransitionTimer();
                UpdateVisualTag(uid);
            }
        }

#if VISUAL_TAG_ANNOUNCE
        private void VisualTagAnnounce(OmniVisualTagAnnounceEvent visualTagAnnounceEvent)
        {
            string msg = string.Format("Visual tag announce. UID={0}", visualTagAnnounceEvent.VisualTagUID);
            App.LogTrace(msg);
            AddEventMessage(msg);

            if (InTransition)
            {
                msg = "Announce ignored (transition timer active)";
                App.LogTrace(msg);
                AddEventMessage(msg);
            }
            else if (WorkstationStatus == WorkstationStatusEnum.RACK_OUT)
            {
                // Transitioning from RACK_OUT to RACK_IN
                //
                TransitionToRackIn(visualTagAnnounceEvent.VisualTagUID);
            }
            else
            {
                // Transitioning from RACK_IN to RACK_OUT
                //
                TransitionToRackOut(visualTagAnnounceEvent.VisualTagUID);
            }
        }



        /// <summary>
        /// Received a Visual Tag announce while in RACK_IN state, 
        /// Attempt to transition from RACK_OUT to RACK_OUT state.
        /// </summary>  
        private void TransitionToRackOut(string visualTagUID)
        {
            string msg = string.Format("Transition to RACK_OUT. UID={0}", visualTagUID);
            App.LogTrace(msg);
            AddEventMessage(msg);

            if (WorkstationStatus != WorkstationStatusEnum.RACK_IN)
            {
                // Precondition. WorkstationStatus must be RACK_IN
                //
                msg = "Precondition violation. WorkstationStatus must be RACK_IN";
                App.LogError("TransitionToRackOut", msg);
                AddEventMessage(msg);
            }
            else if (visualTagUID != WorkstationRack)
            {
                // Precondition, the UID of the rack going out must match the UID of the rack in the workstation
                //
                msg = "Precondition violation. UID of rack in worksatation must equal UID of rack exiting workstation";
                App.LogError("TransitionToRackOut", msg);
                AddEventMessage(msg);
            }
            else
            {
                WorkstationRack = "";
                WorkstationStatus = WorkstationStatusEnum.RACK_OUT;
                StartIgnoreAnnounceTimer();
                UpdateVisualTag(visualTagUID);
            }
        }


        /// <summary>
        /// Received a Visual Tag announce while in RACK_OUT state, 
        /// Attempt to transition from RACK_OUT to RACK_IN state.
        /// </summary>        
        private void TransitionToRackIn(string visualTagUID)
        {
            string msg = string.Format("Transition to RACK_IN. UID={0}", visualTagUID);            
            App.LogTrace(msg);
            AddEventMessage(msg);

            if (WorkstationStatus != WorkstationStatusEnum.RACK_OUT)
            {
                // Precondition. WorkstationStatus must be RACK_OUT
                //
                App.LogError("TransitionToRackIn", "Precondition violation. WorkstationStatus must be RACK_OUT");
                AddEventMessage("Precondition violation. WorkstationStatus must be RACK_OUT");
            }
            else
            {     
                WorkstationRack = visualTagUID;
                WorkstationStatus = WorkstationStatusEnum.RACK_IN;
                StartIgnoreAnnounceTimer();
                UpdateVisualTag(visualTagUID);
            }
        }

#endif
        /// <summary>
        /// Updates a visual tag by sending an image and changing the page.
        /// </summary>        
        private void UpdateVisualTag(string visualTagUID)
        {
            int pageNumber = (WorkstationStatus == WorkstationStatusEnum.RACK_IN) ? 1 : 2;

            // Send Iamge
            if (SendImage(visualTagUID, pageNumber))
            {
                // Change page
                SendPageChange(visualTagUID, pageNumber);
            }
        }

        /// <summary>
        /// Sends a page change command to the Visual Tag
        /// </summary>
        private bool SendPageChange(string visualTagUID, int pageNumber)
        {
            string msg = "Sending page change to visual tag";
            App.LogTrace(msg);
            AddEventMessage(msg);

            bool ok = false;
            try
            {
                string calcId = null; // not used

                _middlewareClient.PostOmniAPICommand(new OmniPageChangeCommand()
                {
                    PageNumber = pageNumber,
                    TransactionID = Guid.NewGuid(),
                    VisualTagUID = visualTagUID,
                    GatewayID = calcId
                });
                ok = true;
            }
            catch (Exception ex)
            {
                App.LogException(ex, "SendPageChange");
                AddEventMessage(string.Format("Unexpected exception in SendPageChange. {0}", ex.ToString()));
            }

            return ok;
        }

        /// <summary>
        /// Send an image to the Visual Tag
        /// </summary>
        private bool SendImage(string visualTagUID, int pageNumber)
        {
            string msg = "Sending image to visual tag";
            App.LogTrace(msg);
            AddEventMessage(msg);

            bool ok = false;
            try
            {                
                string calcId = null; // not used

                _middlewareClient.PostOmniAPICommand(new OmniImageUpdateCommand()
                {
                    TransactionID = Guid.NewGuid(),
                    VisualTagUID = visualTagUID,
                    PageNumber = pageNumber,
                    ImageTemplateID = 7,    // image template id is not currently used
                    CoralType = CoralTypes.P4,
                    ImageTemplateParameters = new Dictionary<string, string>()
			    {
			        {"field0", (WorkstationStatus == WorkstationStatusEnum.RACK_IN) ? "   In": "   Out"},
			        {"field1", DateTime.Now.ToShortTimeString()},
			        {"field2", visualTagUID},
			        {"field3", ""},
			        {"field4", ""},
			        {"field5", ""},                    
			    },
                    GatewayID = calcId
                });
                ok = true;
            }
            catch (Exception ex)
            {
                App.LogException(ex, "SendImage");
                AddEventMessage(string.Format("Unexpected exception in SendImage. {0}", ex.ToString()));
            }

            return ok;
        }

        /// <summary>
        /// Start the transition timer. While timer is active
        /// the business logic will ignore RFID reads.
        /// </summary>
        private void StartTransitionTimer()
        {
            if (_transitionTimer != null)
            {
                _transitionTimer.Interval = new TimeSpan(0, 0, TransitionDuration);

                _transitionTimer.Start();
                NotifyPropertyChanged("InTransition");

                string msg = "Transition timer started";
                App.LogTrace(msg);
                AddEventMessage(msg);
            }
        }

        /// <summary>
        /// Stops the transition timer.
        /// </summary>
        private void StopTransitionTimer()
        {
            if (_transitionTimer != null)
            {
                _transitionTimer.Stop();
                NotifyPropertyChanged("InTransition");

                string msg = "Transition timer stopped";
                App.LogTrace(msg);
                AddEventMessage(msg);
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

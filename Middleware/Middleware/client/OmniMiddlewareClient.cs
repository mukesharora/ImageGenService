using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using Middleware.client.commands;
using Middleware.client.messages;
using System.Configuration;
using OmniMQLib;
using OmniMiddlewareMessages;
using System.Reflection;
using System.Diagnostics;

//[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace Middleware.client
{ 
    /// <summary>
    /// Client interface to Omni-ID's services, providing capability to interact
    /// with Active Tags (via Gateways) and Impinj RFID Readers.
    /// </summary>
    public class OmniMiddlewareClient : IDisposable
    {
        private const string RFID_KEEP_ALIVE = "KEEPALIVE";
        private const string RFID_HOST_DEFAULT = "RevolutionService";

        public ushort BatteryThreshold;

        //Callback through which the Middleware communicates data to the client
        public delegate void OmniAPIEventCallbackHandler(OmniAPIMessage[] messages);

        public event OmniAPIEventCallbackHandler OmniIDMiddlewareEvent;

        private const string SYSTEM_METADATA_FILE = "SystemMetadata.xml";

        private readonly ILog logger = LogManager.GetLogger(typeof(OmniMiddlewareClient));
        //private SignalRClient<CALCManClientDef> calcManClient;
        //private SignalRClient<OmniRFID.RFIDClientDef> rfidReaderClient;
        private MiddlewareCommandModel commandModel;
		
        private Dictionary<int, OmniAPICommand> sentCalcCommands;

        /// <summary>
        /// Message Queue that receives messages from CalcMan and Readers.
        /// </summary>
        private OmniMessageQueueReceiver<List<OmniMiddlewareMessage>> _calcManReceiveMsgQueue = null;
        private QueueHostName _queueName;

        /// <summary>
        /// Resource name of the embedded log4net configuration file.
        /// </summary>
        private const string LOG_CONFIG_RESOURCE_NAME = "Middleware.log4net.config";

        private const string LOG_CONFIG_FILENAME = "log4net.config";

        /// <summary>
        /// Creates a new OmniMiddlewareClient which will interact with the
        /// CALCMan, ImageGen, and Revolution services. The host names of
        /// CALCMan, ImageGen, and OmniImpinjReader services will be
        /// read for the calling applications application configuration file (app.config).        
        /// The ClientGUID will also be read from the application configuration file. See
        /// other constructor for a description of the ClientGuid.
        /// </summary>   
        /// 
        /// <remarks>
        /// Example app.config appSettings are as follows:
        /// <para/>key="CALCManServiceHostName" value="local-host:3000"
        /// <para/>key="ImageGenServiceHostName" value="local-host:30525"
        /// <para/>key="RFIDServiceHostName" value="local-host:3300"
        /// <para/>key="MiddlewareClientGuid" value= "F33B072B647448d2BA48230903A2C565"
        /// </remarks>
        public OmniMiddlewareClient()
        {
            string calcAddress = ConfigurationManager.AppSettings["CALCManServiceHostName"] as string;
            string imageIp = ConfigurationManager.AppSettings["ImageGenServiceHostName"] as string;
            string rfidIp = ConfigurationManager.AppSettings["RFIDServiceHostName"] as string;
            string clientGuid = ConfigurationManager.AppSettings["MiddlewareClientGuid"] as string;

            Init(calcAddress, imageIp, rfidIp, clientGuid);
        }

        /// <summary>
        /// Creates a new OmniMiddlewareClient which will interact with the
        /// CALCMan, ImageGen, and OmniImpinjReader services at the given hostnames.
        /// These hostnames SHOULD NOT include "http://" and SHOULD include the
        /// port number on which the service is running (i.e. "local-host:3000")
        /// </summary>
        /// 
        /// <param name="calcIp">Hostname (IP and port) of the CALCMan service. 
        /// The IP address of the machine running the CALC Manager service and the 
        /// port is the "CALCManServiceHostName" value in the CALCManService.exe.config file</param>
        /// 
        /// <param name="imageIp">Hostname (IP and port) of the ImageGen service. 
        /// The IP address of the machine running the ImageGen service and the port 
        /// is the "ImageGenServiceHostName" value in the CALCManService.exe.config file</param>
        /// 
        /// <param name="rfidIp">Hostname (IP and port) of the OmniImpinj Reader application.
        /// The IP address of the machine running the OmniImpinj Reader application, and a default 
        /// port of 3300</param>      
        /// 
        /// <param name="clientGuid">A unique string that is used to identify the client instance. A client 
        /// should use the same guid each time, and will be used to create a message queue. GUID strings 
        /// like "F33B072B647448d2BA48230903A2C565" are recommended. This string should appear
        /// in the QueueNames.txt file as well as the CALCMan database for the Impinj reader. It is anticipated 
        /// that this parameter will be removed in the near future.</param>
        /// 
        /// <remarks>
        /// Example:
        /// var middleware = new OmniMiddlewareClient("localhost:3000", "localhost:30525", "localhost:3300", "F33B072B647448d2BA48230903A2C565");
        /// </remarks>         
        public OmniMiddlewareClient(string calcIp, string imageIp, string rfidIp, string clientGuid)
        {
            Init(calcIp, imageIp, rfidIp, clientGuid);
        }

        /**
        * Ensures that SignalR clients get disconnected from the servers when
        * this object goes out of scope.
        */
        ~OmniMiddlewareClient()
        {
            Dispose();
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (_calcManReceiveMsgQueue != null)
            {
                _calcManReceiveMsgQueue.StopProcessing();
                _calcManReceiveMsgQueue = null;
                // We are no longer claiming queues. Client supplies queue name.
                //QueueHostName.UnclaimQueue(_queueName.QueueName);
            }
        }

        /**
        * Asynchronously executes the given command and returns any requisite
        * responses to the OmniIDMiddlewareEvent.
        * Note that invalid/incomplete commands will not be executed and an
        * errorMsg response will be returned in response.
        * 
        * @param command	An instance of an OmniAPICommand subclass for the
        *					command that should be executed.
        */
        public void PostOmniAPICommand(OmniAPICommand command)
        {
            OmniCommandErrorResultEvent errorMessage;

            if (!command.IsValid(out errorMessage))
            {
                PostMessage(errorMessage);

                return;
            }

            switch(command.CommandType)
            {
                case OmniAPICommandType.OmniGPIStateReportCommand:
                case OmniAPICommandType.OmniImageUpdateCommand:
                case OmniAPICommandType.OmniPageChangeCommand:
                case OmniAPICommandType.OmniSystemMetadataInfoRequestCommand:
                case OmniAPICommandType.OmniVisualTagInfoRequestCommand:
                case OmniAPICommandType.OmniGPOStateChangeCommand:
                case OmniAPICommandType.OmniRFIDReaderCommand:
                case OmniAPICommandType.OmniImageUrlUpdateCommand:
                case OmniAPICommandType.OmniPageDeleteCommand:
                    List<OmniAPIMessage> responseMessages;

                    if (command.HasCALCResponse)
                    {
                        //Lock around Commands to the CALC so we can index their
                        //handles before their status messages come in
                        lock (sentCalcCommands)
                        {
                            if (command.Execute(commandModel, out responseMessages))
                            {
                                if (command.CALCResponseHandle != null)
                                {
                                    sentCalcCommands.Add(command.CALCResponseHandle.Value, command);
                                    logger.Info(String.Format("Command with TransactionID: {0} queued", command.TransactionID));
                                }
                            }
                        }
                    }
                    else
                    {
                        command.Execute(commandModel, out responseMessages);
                    }

                    PostMessages(responseMessages.ToArray());

                    break;
                case OmniAPICommandType.OmniGPOStateReportCommand:
                default:
                    errorMessage = OmniCommandErrorResultEvent.Unsupported_Command_Error;
                    errorMessage.TransactionID = command.TransactionID;

                    PostMessage(errorMessage);

                    return;
            }
        }

        #region Private helper functions

        /// <summary>
        /// Initialize the Middelware client
        /// </summary>
        private void Init(string calcIp, string imageIp, string rfidIp, string clientGuid)
        {
            try
            {
                ConfigureLogging();
                LogMiddlewareVersion();                
                logger.Info(string.Format("Client GUID: {0}", clientGuid));

                var clients = MiddlewareCommandModel.RestClients.CreateInstance(calcIp ?? "", imageIp ?? "", rfidIp ?? "");
                commandModel = new MiddlewareCommandModel(clients, SYSTEM_METADATA_FILE);

                sentCalcCommands = new Dictionary<int, OmniAPICommand>();

                ConfigureBatteryThreshold();

                ConfigureMessageQueue(clientGuid);
            }
            catch (Exception ex)
            {
                logger.Error("Unexpected exception in Init()", ex);
            }
        }

        /// <summary>
        /// Configure Log4Net from embedded configuration file, but use external
        /// configuration file if it exists. This will allow default logging outside
        /// of the box, and customized logging as well.
        /// </summary>
        private void ConfigureLogging()
        {
            if (!ConfigureLogOverride())
            {                
                try
                {
                    // Use embedded log4net configuration.
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Stream stream = assembly.GetManifestResourceStream(LOG_CONFIG_RESOURCE_NAME);
                    log4net.Config.XmlConfigurator.Configure(stream);                    
                }
                catch (Exception ex)
                {
                    // Not much we can do here.
                    Debug.WriteLine("Error configuring log4net with embedded configuration file:" + ex.ToString());
                }
            }
        }

        private bool ConfigureLogOverride()
        {
            bool useOverride = false;

            try
            {
                if (File.Exists(LOG_CONFIG_FILENAME))
                {
                    XmlConfigurator.Configure(new FileInfo(LOG_CONFIG_FILENAME));
                    useOverride = true;
                }
            }
            catch (Exception ex)
            {
                // Not much we can do here.
                Debug.WriteLine("Error configuring log4net with override configuration file:" + ex.ToString());
            }

            return useOverride;
        }

        /**
        * Configures the BatteryThreshold from the application configuration
        * file or uses a default value if the key is not present.
        */
        private void ConfigureBatteryThreshold()
        {
            var appSettings = ConfigurationManager.AppSettings;

            if (appSettings.AllKeys.Contains("BatteryThreshold"))
            {
                try
                {
                    BatteryThreshold = ushort.Parse(appSettings["BatteryThreshold"]);
                }
                catch (FormatException fe)
                {
                    logger.Error("Could not parse application configuration value for BatteryThreshold, using default value.", fe);
                    BatteryThreshold = 28;
                }
            }
            else
            {
                logger.Warn("No application configuration value found for BatteryThreshold, using default value.");
                BatteryThreshold = 28;
            }

            logger.Info(String.Format("BatteryThreshold set to {0}", BatteryThreshold));
        }
       
        /**
        * Wraps the message in a list and delegates to PostMessages below.
        */
        private void PostMessage(OmniAPIMessage message)
        {
            PostMessages(new OmniAPIMessage[] { message });
        }

        /**
        * Posts the given list of messages to the client via the
        * OmniIDMiddlewareEvent.
        */
        private void PostMessages(OmniAPIMessage[] messages)
        {
            if (OmniIDMiddlewareEvent != null)
            {
                OmniIDMiddlewareEvent(messages);
            }

            foreach (OmniAPIMessage msg in messages)
            {
                if (msg is OmniCommandErrorResultEvent)
                {
                    OmniCommandErrorResultEvent errorMsg = msg as OmniCommandErrorResultEvent;
                    logger.Error(String.Format("{0} - {1}", errorMsg.ErrorCode, errorMsg.Information));
                }
                else if (msg is OmniSystemErrorEvent)
                {
                    OmniSystemErrorEvent errorMsg = msg as OmniSystemErrorEvent;
                    logger.Error(String.Format("{0} - {1}", errorMsg.ErrorCode, errorMsg.Information));
                }
            }
        }

        /**
        * Configure the Calc Manager receive message queue.
        * 
        */
        private bool ConfigureMessageQueue(string clientGuid)
        {
            bool ok = false;
            try
            {
                // Get queue names from configuration (QueueNames.txt) file.
                //                
                _queueName = QueueHostName.ClaimQueue(clientGuid);

                if (_queueName == null)
                {
                    _queueName = new QueueHostName()
                    {
                        HostName = "localhost",
                        QueueName = clientGuid
                    };
                }


                if (_queueName != null)
                {
                    // Create and start the queue.
                    //
                    _calcManReceiveMsgQueue = new OmniMessageQueueReceiver<List<OmniMiddlewareMessage>>(_queueName.HostName, _queueName.QueueName);
                    _calcManReceiveMsgQueue.ProcessMessageCallback = new OmniMessageQueueReceiver<List<OmniMiddlewareMessage>>.ProcessDeserializedMessageDelegate(ProcessQueueMessage);
                    _calcManReceiveMsgQueue.StartProcessing();
                    ok = true;
                }
                else
                {
                    //logger.Error(string.Format("Error could not get a queue name."));

                }
            }
            catch (Exception ex)
            {
                logger.Error("Could not create ReceiveMsgQueue", ex);
            }

            return ok;
        }

        /*
        * Gets an application configuration setting for the app.config file.
        */ 
        private string GetConfigSetting(string settingName)
        {
            string settingValue = null;

            try
            {
                settingValue = ConfigurationManager.AppSettings[settingName];
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Unexpected error in GetConfigSetting. Setting = [{0}]", settingName), ex);  
            }

            return settingValue;
        }

        #endregion

        #region Event callback functions

        /*
        * Process the CalcMan message from the message queue.
        */
        private void ProcessQueueMessage(List<OmniMiddlewareMessage> msgs)
        {
            try
            {
                if (msgs != null && msgs.Count() > 0)
                {
                    switch (msgs.First().MsgType)
                    {
                        // ASSERT: all OmniMiddlewareMessage in msgs are of the same MsgType
                        case MsgTypes.ANNOUNCE:
                            {
                                ReceivedAnnounces(msgs);
                                break;
                            }
                        case MsgTypes.STATUS:
                            {
                                ReceivedStatusMessages(msgs);
                                break;
                            }
                        case MsgTypes.RFID:
                            {
                                ReceivedReaderMessages(msgs.First().RFIDEvents);
                                break;
                            }
                        default:
                            {
                                logger.Warn(string.Format("Unknown message type: {0}", msgs.First().MsgType.ToString()));
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Unknown exception in ProcessCalcManMessage()", ex);
            }
        }

        /**
        * Inspects all received announces for low battery levels and posts a
        * message to the client whenever one is found which is below the
        * BATTERY_THRESHOLD.
        */
        private void ReceivedAnnounces(List<OmniMiddlewareMessage> announces)
        {
            List<OmniAPIMessage> events = new List<OmniAPIMessage>();

            foreach (OmniMiddlewareMessage announce in announces)
            {
                if (announce.BatteryLevel < BatteryThreshold)
                {
                    OmniVisualTagHealthReportMessage healthReport =
                        new OmniVisualTagHealthReportMessage()
                        {
                            VisualTagUID = announce.Uid,
                            BatteryLevel = (int)announce.BatteryLevel
                        };

                    events.Add(healthReport);

                    logger.Warn(String.Format("Low battery detected: {0}", healthReport));
                }
                else
                {
                    OmniVisualTagAnnounceEvent ae = new OmniVisualTagAnnounceEvent() { VisualTagUID = announce.Uid };
                    events.Add(ae);
                }
            }

            PostMessages(events.ToArray());
        }

        /**
        * Correlates received command status messages with in-progress
        * commands (which were going to the CALC) and posts messages to the
        * client as needed.
        */
        private void ReceivedStatusMessages(List<OmniMiddlewareMessage> statusMsgs)
        {
            List<OmniAPIMessage> events = new List<OmniAPIMessage>();

            foreach (OmniMiddlewareMessage statusMsg in statusMsgs)
            {
                //Lock on the command mapping so that the transaction is stored
                //before any status messages are handled
                lock (sentCalcCommands)
                {
                    OmniAPICommand sentCommand = null;

                    //Lookup associated Command in sentCalcCommands map
                    if (sentCalcCommands.ContainsKey(statusMsg.Handle))
                    {
                        sentCommand = sentCalcCommands[statusMsg.Handle];
                    }
                    else
                    {
                        OmniSystemInformationEvent info =
                            new OmniSystemInformationEvent()
                            {
                                Information = String.Format("Got status message without transaction: {0}", statusMsg)
                            };

                        PostMessage(info);
                        logger.Warn(info.Information);

                        continue;
                    }

                    OmniCalcCommandResultEvent calcResult = null;

                    switch(sentCommand.CommandType)
                    {
                        case OmniAPICommandType.OmniImageUrlUpdateCommand:
                            OmniImageUrlUpdateCommand imageUrlUpdate = sentCommand as OmniImageUrlUpdateCommand;

                            calcResult = new OmniImageUpdateCommandResultEvent()
                            {
                                TransactionID = imageUrlUpdate.TransactionID,
                                VisualTagUID = imageUrlUpdate.VisualTagUID
                            };

                            break;
                        case OmniAPICommandType.OmniImageUpdateCommand:
                            OmniImageUpdateCommand imageUpdate = sentCommand as OmniImageUpdateCommand;

                            calcResult = new OmniImageUpdateCommandResultEvent()
                            {
                                TransactionID = imageUpdate.TransactionID,
                                VisualTagUID = imageUpdate.VisualTagUID
                            };

                            break;
                        case OmniAPICommandType.OmniPageChangeCommand:
                            OmniPageChangeCommand pageChange = sentCommand as OmniPageChangeCommand;

                            calcResult = new OmniPageChangeCommandResultEvent()
                            {
                                TransactionID = pageChange.TransactionID,
                                VisualTagUID = pageChange.VisualTagUID
                            };

                            break;
                        case OmniAPICommandType.OmniPageDeleteCommand:
                            OmniPageDeleteCommand pageDelete = sentCommand as OmniPageDeleteCommand;

                            calcResult = new OmniPageDeleteCommandResultEvent()
                            {
                                TransactionID = pageDelete.TransactionID,
                                VisualTagUID = pageDelete.VisualTagUID
                            };

                            break;
                        case OmniAPICommandType.OmniSystemMetadataInfoRequestCommand:
                            //We shouldn't ever get one of these...
                        case OmniAPICommandType.OmniVisualTagInfoRequestCommand:
                            //Don't think we should get one of these either
                        default:
                            OmniSystemInformationEvent info =
                                new OmniSystemInformationEvent()
                                {
                                    Information = String.Format("Got status message for unsupported Command Type: {0}, TransactionID: {1}",
                                        sentCommand.CommandType, sentCommand.TransactionID)
                                };

                            events.Add(info);
                            logger.Warn(info.Information);

                            sentCalcCommands.Remove(statusMsg.Handle);

                            break;
                    }

                    //Set status flags and add to the events list
                    if (calcResult != null)
                    {
                        if (CalcErrorCode.IsFailed(statusMsg.ErrorCode))
                        {
                            calcResult.SetCommandFailed();
                            logger.Error(String.Format("Command with TransactionID: {0} failed with error code: {1}",
                                calcResult.TransactionID, statusMsg.ErrorCode));
                        }
                        else if (CalcErrorCode.IsRetrying(statusMsg.ErrorCode))
                        {
                            calcResult.SetCommandRetrying();
                            logger.Info(String.Format("Command with TransactionID: {0} retrying", calcResult.TransactionID));
                        }
                        else if (CalcErrorCode.IsCompleted(statusMsg.ErrorCode))
                        {
                            calcResult.SetCommandSuccess();
                            logger.Info(String.Format("Command with TransactionID: {0} successful", calcResult.TransactionID));
                        }
                        else if (CalcErrorCode.IsSending(statusMsg.ErrorCode))
                        {
                            calcResult.SetCommandSending();
                            logger.Info(String.Format("Command with TransactionID: {0} sending", calcResult.TransactionID));
                        }

                        events.Add(calcResult);
                    }

                    //Remove the command if it has been completed or it failed
                    if (calcResult.CommandReceived || calcResult.CommandFailed)
                    {
                        sentCalcCommands.Remove(statusMsg.Handle);
                    }
                }
            }

            //Fire off all the messages
            PostMessages(events.ToArray());
        }
        
        private void ReceivedReaderMessages(List<RFIDReaderEvent> rfidMsgs)
        {
            List<OmniAPIMessage> events = new List<OmniAPIMessage>();

            foreach (RFIDReaderEvent message in rfidMsgs)
            {
                switch(message.EventType)
                {
                    case OmniMiddlewareMessages.RFIDReaderEventType.GPILevelChange:
                    {
                        events.Add(new OmniGPIEventMessage()
                        {
                            GPIPortNumber = message.GPIPortNumber,
                            PortState = message.GPIPortLevel == OmniMiddlewareMessages.SwitchLevel.Start ? GPIOPortState.High : GPIOPortState.Low,
                            RFIDDeviceID = message.ReaderGUID,
                            RFIDDeviceAddress = message.ReaderAddress
                        });

                        break;
                    }
                    case OmniMiddlewareMessages.RFIDReaderEventType.RFIDTagRead:
                    case OmniMiddlewareMessages.RFIDReaderEventType.AntennaTagRead:
                    {
                        events.Add(new OmniRFIDDetectionMessage()
                        {
                            RFIDDeviceAntennaID = message.AntennaNumber,
                            RFIDDeviceID = message.ReaderGUID,
                            RFIDDeviceAddress = message.ReaderAddress,
                            RFIDTagEPC = message.EPC,
                            RFIDTagUserMemory = message.USER,
                            TimeStamp = DateTime.Now
                        });

                        break;
                    }
                    case OmniMiddlewareMessages.RFIDReaderEventType.Exception:
                    {
                        OmniSystemErrorEvent errorMessage = OmniSystemErrorEvent.RFID_Reader_Error;
                        errorMessage.Information += message.Message;
                        events.Add(errorMessage);
                        break;
                    }
                    case OmniMiddlewareMessages.RFIDReaderEventType.Event:
                    {
                        events.Add(new OmniSystemInformationEvent()
                        {
                            RFIDDeviceID = message.ReaderGUID,
                            RFIDDeviceAddress = message.ReaderAddress,
                            Information = message.Message
                        });
                        break;
                    }
                    case OmniMiddlewareMessages.RFIDReaderEventType.GPOStateChange:
                    case OmniMiddlewareMessages.RFIDReaderEventType.GPOStateReport:
                    default:
                    {
                        OmniSystemErrorEvent errorMessage = OmniSystemErrorEvent.Unsupported_Message_Type;
                        errorMessage.Information = string.Format("Got reader message of type {0} with unsupported MessageType: {1}", message.GetType().ToString(), message.EventType);
                        events.Add(errorMessage);                        
                        break;
                    }                        
                }
            }

            PostMessages(events.ToArray());
        }

        /// <summary>
        /// Logs Middleware version.
        /// </summary>
        private void LogMiddlewareVersion()
        {
            try
            {
                Assembly assembly = Assembly.GetCallingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

                logger.Info(string.Format("Middleware client startup. File version: {0}", fvi.FileVersion));
                
            }
            catch (Exception ex)
            {
                logger.Warn(string.Format("Unexpected exception in LogMiddlewareVersion: ", ex.ToString()));                
            }
        }
        
        #endregion
    }
    
    /**
    * AggregateException class extension for convenience
    */
    static class AggregateExceptionExtensions
    {
        /**
        * Returns a new AggregateException with the given Exception added to
        * the InnerExceptions.
        */
        public static AggregateException AddException(this AggregateException ae,
            Exception newException)
        {
            List<Exception> exceptions = new List<Exception>(ae.InnerExceptions);
            exceptions.Insert(0, newException);
            
            return new AggregateException(exceptions);
        }
    }
    
    /**
    * Encapsulates the logic for parsing CALC status message error codes.
    */
    class CalcErrorCode
    {
        private const int COMPLETED = 0;
        private const int SENDING = 1;
        private const int FAILED = -1;
        private const int RETRYING = 2;
        
        /**
        * Returns true iff the given error code indicates that the command
        * completed successfully, false otherwise.
        */
        public static bool IsCompleted(int errorCode)
        {
            return errorCode == COMPLETED;
        }
        
        /**
        * Returns true iff the given error code indicates that the command was
        * received by the CALC, but has not yet been sent to the tag, false
        * otherwise.
        */
        public static bool IsSending(int errorCode)
        {
            return errorCode == SENDING;
        }
        
        /**
        * Returns true iff the given error code indicates that the CALC
        * encountered an error of some sort while handling the command, false
        * otherwise.
        */
        public static bool IsFailed(int errorCode)
        {
            return errorCode <= FAILED || errorCode > RETRYING;
        }
        
        /**
        * Returns true iff the given error code indicates that CALCMan is
        * retrying the command after an initial failure.
        */
        public static bool IsRetrying(int errorCode)
        {
            return errorCode == RETRYING;
        }
    }
}
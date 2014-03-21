
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using OmniWinIPC;
using OmniSysSchedSQLSDAL;
using OmniCfgSvcBLL;
using System.Windows;
using ClientConfigurator.Utility;
using NLog;

namespace ClientConfigurator.Models
{
    public class ClientConfigItem : ReactiveObject
    {
        #region Logging

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constants

        /// <summary>
        /// Default port to start at.
        /// </summary>
        public const int DEFAULT_IMPINJAPP_PORT = 3300;
        

        /// <summary>
        /// Configuration parameter names. These should be migrated to ClientConfigConstants as 
        /// needed.
        /// </summary>
        const string USES_CALCMAN_PARAMNAME = "Uses CALCMan";                           // Parameter name
        const string USES_IMAGEGEN_PARAMNAME = "Uses ImageGen";                         // Parameter name
        const string USES_IMPINJAPP_PARAMNAME = "Uses OmniImpinjApp";                   // Parameter name
        const string IMPINJ_PORT_PARAMNAME = "Impinj app port";                         // Parameter name        
        const string IMPINJ_CLIENT_HOSTNAME_PARAMNAME = "OmniImpinj client hostname";   // Parameter name        
        const string IMPINJ_HOSTNAME_PARAMNAME = "OmniImpinj hostname";                 // Parameter name        

        const string OUTPUT_WRITER_VALUE = "MessageQueue";   

        #endregion

        #region Constructor(s)

        public ClientConfigItem()
        {
            ClientGuid = Guid.NewGuid().ToString();

            // Initialize client specific CALCMan settings.
            //
            CalcManClientHosts = new ObservableCollection<string>();
            CalcManClientHosts.Add(App.LOCAL_HOST);
            CalcManClientHost = CalcManClientHosts.ElementAt(0);

            // Initialize client specific Impinj Reader app settings.
            //
            ImpinjAppClientHosts = new ObservableCollection<string>();
            ImpinjAppClientHosts.Add(App.LOCAL_HOST);
            ImpinjAppClientHost = ImpinjAppClientHosts.ElementAt(0);

            ImpinjHostNames = new ObservableCollection<string>();
            ImpinjHostNames.Add(App.LOCAL_HOST);
            ImpinjHostName = ImpinjHostNames.ElementAt(0);

            ImpinjAppReaderPort = DEFAULT_IMPINJAPP_PORT.ToString();

            // Setup action commands.
            //
            //CopyConfigString = new ReactiveCommand();
            //CopyConfigString.ObserveOnDispatcher().Subscribe(_ =>
            //{
            //    try
            //    {
            //        Clipboard.SetData(DataFormats.Text, ConfigString);
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Warn("unhandled in CopyConfigString action", ex);

            //    }
            //});

            //CopyAppConfigString = new ReactiveCommand();
            //CopyAppConfigString.ObserveOnDispatcher().Subscribe(_ => Clipboard.SetData(DataFormats.Text, AppConfigString));             

            // Initialize what services this client uses.
            //
            UsesCalcMan = false;
            UsesImageGen = false;
            UsesImpinjReaderApp = false;            
        }

        #endregion

        #region Public properties

        public ObservableCollection<string> CalcManClientHosts { get; set; }
        public ObservableCollection<string> ImpinjAppClientHosts { get; set; }
        public ObservableCollection<string> ImpinjHostNames { get; set; }        

        /// <summary>
        /// Action when configuration string (constructor) needs to be copied to the clipboard.
        /// </summary>
        //public IReactiveCommand CopyConfigString { get; protected set; }

        /// <summary>
        /// Action when app configuration string (app.settings) needs to be copied to the clipboard.
        /// </summary>
        //public IReactiveCommand CopyAppConfigString { get; protected set; }
        

        /// <summary>
        /// Name of the client host machine.
        /// Set to localhost if this client runs on the same machine as the CALCMan service.
        /// </summary>
        private string _calcManClientHost;
        public string CalcManClientHost
        {
            get { return _calcManClientHost; }
            set
            {
                this.RaiseAndSetIfChanged(ref _calcManClientHost, value);
                IsDirty = true;
            }
        }

        /// <summary>
        /// Name of the client host machine.
        /// Set to localhost if this client runs on the same machine as the Impinj reader app.
        /// </summary>
        private string _impinjAppClientHost;
        public string ImpinjAppClientHost
        {
            get { return _impinjAppClientHost; }
            set
            {
                this.RaiseAndSetIfChanged(ref _impinjAppClientHost, value);
                IsDirty = true;
            }
        }

        /// <summary>
        /// Name of machine this Omni Impinj reader app is running on. 
        /// Set to localhost if this is the same machine that the client is running on.
        /// </summary>
        private string _impinjHostName;
        public string ImpinjHostName
        {
            get { return _impinjHostName; }
            set
            {
                this.RaiseAndSetIfChanged(ref _impinjHostName, value);
                IsDirty = true;
            }
        }                

        /// <summary>
        /// Boolean if this client uses CALCMan.
        /// </summary>
        private bool _usesCalcMan;
        public bool UsesCalcMan
        {
            get { return _usesCalcMan; }
            set
            {
                this.RaiseAndSetIfChanged(ref _usesCalcMan, value);
                GenerateConfigString();
                IsDirty = true;
            }
        }

        /// <summary>
        /// Boolean if this client uses ImageGen.
        /// </summary>
        private bool _usesImageGen;
        public bool UsesImageGen
        {
            get { return _usesImageGen; }
            set
            {
                this.RaiseAndSetIfChanged(ref _usesImageGen, value);
                GenerateConfigString();
                IsDirty = true;
            }
        }

        /// <summary>
        /// Boolean if this client uses the Impinj reader app.
        /// </summary>
        private bool _usesImpinjReaderApp;
        public bool UsesImpinjReaderApp
        {
            get { return _usesImpinjReaderApp; }
            set
            {
                this.RaiseAndSetIfChanged(ref _usesImpinjReaderApp, value);
                GenerateConfigString();
                IsDirty = true;
            }
        }

        /// <summary>
        /// THis apps' client GUID. 
        /// </summary>
        private string _clientGuid;
        public string ClientGuid
        {
            get { return _clientGuid; }
            set
            {
                this.RaiseAndSetIfChanged(ref _clientGuid, value);
                GenerateConfigString();
                IsDirty = true;
            }
        }

        /// <summary>
        /// The Impinj reader apps IP port. This is auto-assigned but may be changed.
        /// Must be unique to other clients Impinj reader app ports.
        /// </summary>
        private string _impinjAppReaderPort;
        public string ImpinjAppReaderPort
        {
            get { return _impinjAppReaderPort; }
            set
            {
                int port = -1;

                if (Int32.TryParse(value, out port))
                {
                    this.RaiseAndSetIfChanged(ref _impinjAppReaderPort, value);
                    GenerateConfigString();
                    IsDirty = true;
                }
                else
                {
                    throw new ApplicationException("Must be a number.");
                }
            }
        }


        private string _appConfigString;
        public string AppConfigString
        {
            get { return _appConfigString; }
            set { this.RaiseAndSetIfChanged(ref _appConfigString, value); }
        }  
        
        /// <summary>
        /// The configuration string (constructor)
        /// </summary>
        private string _configString;
        public string ConfigString
        {
            get { return _configString; }
            set { this.RaiseAndSetIfChanged(ref _configString, value); }
        }  

        /// <summary>
        /// Boolean, if the client configuration needs to be saved.
        /// </summary>
        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { this.RaiseAndSetIfChanged(ref _isDirty, value); }
        }

        /// <summary>
        /// Prefix the the category that this item will use in the
        /// configuration database.
        /// </summary>
        public static string CategoryNamePrefix
        {
            get { return ClientConfigConstants.CLIENT_CATEGORY_NAME_PREFIX; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loads a client configuration object from the database.
        /// </summary>
        /// <param name="configDB">The configuration database.</param>
        /// <param name="category">The client configurations configuration category.</param>
        /// <returns>A ClientConfigItem object or null if there was a problem.</returns>
        public static ClientConfigItem Load(IConfigurationParameters configDB, string category)
        {
            ClientConfigItem item = null;

            try
            {
                item = new ClientConfigItem();

                // Load in settings from the database.
                //
                item.ClientGuid = configDB.Get(category, ClientConfigConstants.CLIENT_GUID_PARAMNAME);
                item.UsesCalcMan = bool.Parse(configDB.Get(category, USES_CALCMAN_PARAMNAME));
                item.UsesImageGen = bool.Parse(configDB.Get(category, USES_IMAGEGEN_PARAMNAME));
                item.UsesImpinjReaderApp = bool.Parse(configDB.Get(category, USES_IMPINJAPP_PARAMNAME));

                if (item.UsesCalcMan)
                {
                    item.CalcManClientHost = configDB.Get(category, ClientConfigConstants.CALCMAN_CLIENT_HOSTNAME_PARAMNAME);
                }

                if (item.UsesImpinjReaderApp)
                {
                    item.ImpinjAppClientHost = configDB.Get(category, IMPINJ_CLIENT_HOSTNAME_PARAMNAME);
                    item.ImpinjAppReaderPort = configDB.Get(category, IMPINJ_PORT_PARAMNAME);
                    item.ImpinjHostName = configDB.Get(category, IMPINJ_HOSTNAME_PARAMNAME);
                }                

                // Setup the host-name ComboBoxes.
                //
                if (item.ImpinjAppClientHost != App.LOCAL_HOST)
                {
                    item.ImpinjAppClientHosts.Add(item.ImpinjAppClientHost);
                    item.ImpinjAppClientHost = item.ImpinjAppClientHost;
                }

                if (item.ImpinjHostName != App.LOCAL_HOST)
                {
                    item.ImpinjHostNames.Add(item.ImpinjHostName);
                    item.ImpinjHostName = item.ImpinjHostName;
                }

                if (item.CalcManClientHost != App.LOCAL_HOST)
                {
                    item.CalcManClientHosts.Add(item.CalcManClientHost);
                    item.CalcManClientHost = item.CalcManClientHost;
                }

                item.IsDirty = false;
            }
            catch (Exception ex)
            {
                logger.WarnException("unhandled in Load", ex);
            }

            return item;
        }

        /// <summary>
        /// Saves the client configuration to the database.
        /// </summary>
        /// <param name="configs">The configuration database.</param>
        /// <param name="sysSchedEntities">Database entities for the application scheduler.</param>
        /// <returns>boolean of successful</returns>
        public bool Save(IConfigurationParameters configs, OmniSysSchedEntities sysSchedEntities)
        {
            bool ok = false;

            try
            {
                IsDirty = false;

                // Delete the existing setting. This is done if the user changes the
                // Uses_xyz properties. If they were for example using CALCMan, the CALCMan
                // client settings would be saved in the database. If they later decide not
                // to use CALCMan, we need to delete those settings. Deleting the existing
                // settings is the lazy-man way to accomplish this.
                //
                // If this ClientConfig item has never been saved to the database, the DeleteCategory
                // call will hang for a bit if the log service is not running.
                // Try to prevent this by adding a dummy parameter so the parameter table
                // for this category is not empty.
                //
                configs.Set(CategoryName, "placeholder", "delete record placeholder");
                configs.DeleteCategory(CategoryName);

                // Save settings to the configuration database.
                //
                configs.Set(CategoryName, ClientConfigConstants.CLIENT_GUID_PARAMNAME, ClientGuid);
                configs.Set(CategoryName, USES_CALCMAN_PARAMNAME, UsesCalcMan.ToString());
                configs.Set(CategoryName, USES_IMAGEGEN_PARAMNAME, UsesImageGen.ToString());
                configs.Set(CategoryName, USES_IMPINJAPP_PARAMNAME, UsesImpinjReaderApp.ToString());

                if (UsesCalcMan)
                {
                    configs.Set(CategoryName, ClientConfigConstants.CALCMAN_CLIENT_HOSTNAME_PARAMNAME, CalcManClientHost);
                }

                if (UsesImpinjReaderApp)
                {
                    configs.Set(CategoryName, IMPINJ_CLIENT_HOSTNAME_PARAMNAME, ImpinjAppClientHost);
                    configs.Set(CategoryName, IMPINJ_PORT_PARAMNAME, ImpinjAppReaderPort);
                    configs.Set(CategoryName, IMPINJ_HOSTNAME_PARAMNAME, ImpinjHostName);
                }


                //// There is no database relationship between the ScheduledApps and the Application tables.
                //// o ScheduledApps table is for just starting apps.
                //// o Application table is for configuration.
                ////
                //// For the Impinj reader, We need to schedule an app, and the OmniImpinj app will create configuration
                //// for it using the Application and parameter tables.

                //// ***************************************************************
                //// TODO - do we need to manage this relationship here in any way??
                //// ***************************************************************
                string scheduledAppName = "ImpinjReader_" + ClientGuid;

                // Look up YES/NO values
                //
                // NOTE: TODO This could fail. Do not assume these values are in the database.
                YesNo yesNo = sysSchedEntities.YesNoes.Where(x => x.Text.Equals("yes", StringComparison.InvariantCultureIgnoreCase)).First();
                int yesValue = yesNo.ID;

                yesNo = sysSchedEntities.YesNoes.Where(x => x.Text.Equals("no", StringComparison.InvariantCultureIgnoreCase)).First();
                int noValue = yesNo.ID;

                // Look up scheduled type
                //
                ScheduleType scheduleType = sysSchedEntities.ScheduleTypes.Where(x => x.Description.Equals("always running", StringComparison.InvariantCultureIgnoreCase)).First();
                int scheduleTypeValue = scheduleType.ID;

                // Schedule the app.
                //
                string commandLine = string.Format("/appGUID={0} /port={1} /clientMachine={2} /clientGuid={3} /OutputWriterComponentName={4}", ClientGuid, ImpinjAppReaderPort, ImpinjAppClientHost, ClientGuid, OUTPUT_WRITER_VALUE);
                string workingPath = null;
                // TODO, this should really be read in the installer data or registry to support 32-bit and installs to other drives.
                string exePath = @"C:\Program Files (x86)\Omni-id\OmniImpinjReader\OmniImpinjReader.exe";

                int appCount = sysSchedEntities.ScheduledApps.Where(x => x.AppGUID.Equals(ClientGuid, StringComparison.InvariantCultureIgnoreCase)).Count();

                int enabled = UsesImpinjReaderApp ? yesValue : noValue;
                if (appCount == 0)
                {
                    // Need new record.
                    //
                    ScheduledApps app = new ScheduledApps()
                    {
                        AppGUID = ClientGuid,
                        CommandLine = commandLine,
                        Enabled = enabled,
                        FullyQualifiedExePath = exePath,
                        HideGUI = noValue,
                        AppName = scheduledAppName,
                        FK_ID_ScheduleType = scheduleTypeValue,
                        WorkingDirectoryPath = workingPath,
                    };

                    sysSchedEntities.ScheduledApps.AddObject(app);
                    sysSchedEntities.SaveChanges();
                    IsDirty = false;
                }
                else if (appCount == 1)
                {
                    // Modify existing record.
                    //
                    ScheduledApps app = sysSchedEntities.ScheduledApps.Where(x => x.AppGUID.Equals(ClientGuid, StringComparison.InvariantCultureIgnoreCase)).First();

                    // Things that can be modified in the GUI
                    // GUI changes          -> Database (ScheduledApps)
                    // Enabled              -> Enabled
                    // port, client machine -> CommandLine

                    app.Enabled = UsesImpinjReaderApp ? yesValue : noValue;
                    app.CommandLine = commandLine;

                    sysSchedEntities.SaveChanges();
                    IsDirty = false;
                }
                else
                {
                    logger.Warn("Too many scheduled apps.");
                }

            }
            catch (Exception ex)
            {
                logger.WarnException("unhandled in Save", ex);
            }

            return ok;
        }

        /// <summary>
        /// Deletes a client configuration.
        /// </summary>
        public bool Delete(IConfigurationParameters configs, OmniSysSchedEntities sysSchedEntities)
        {
            bool ok = false;

            try
            {
                // Delete the category that defines this client configuration. 
                // NOTE: All category parameters will also be deleted.
                //
                if (configs.DeleteCategory(CategoryName))
                {
                    ok = true;

                    // Delete Impinj reader scheduled app.
                    var scheduledApps = sysSchedEntities.ScheduledApps.Where(x => x.AppGUID.Equals(ClientGuid, StringComparison.InvariantCultureIgnoreCase));
                    foreach (var app in scheduledApps)
                    {
                        sysSchedEntities.DeleteObject(app);
                    }
                    sysSchedEntities.SaveChanges();

                    // Omni-Impinj reader configuration will be kept in
                    // the database. This app did not write those values,
                    // so for now, they stay (ImpinjReader needs some helper
                    // methods to get rid of this data).
                }
            }
            catch (Exception ex)
            {
                logger.WarnException("unhandled in Delete", ex);
            }

            return ok;
        }

        /// <summary>
        /// Generates the constructor configuration string.
        /// </summary>
        public void GenerateConfigString(string calcManHostName, string calcManPort, string imageGenHostName, string imageGenPort)
        {
            CalcManHostName = calcManHostName;
            CalcManPort = calcManPort;

            ImageGenHostName = imageGenHostName;
            ImageGenPort = imageGenPort;

            GenerateConfigString();
        }

        #endregion

        #region Private properties

        /// <summary>
        /// This items category string. Used for saving the item to the database.
        /// </summary>
        private string CategoryName
        {
            get { return CategoryNamePrefix + ClientGuid; }
        }

        // From CalcManConfig class, for config string.
        private string CalcManHostName { get; set; }
        private string CalcManPort { get; set; }

        // from ImageGenConfig class, for config string.
        private string ImageGenHostName { get; set; }
        private string ImageGenPort { get; set; }


        #endregion

        #region Private methods

        /// <summary>
        /// Generates the configuration strings.
        /// </summary>
        public void GenerateConfigString()
        {
            const string NULL_STRING = "null";

            string fullCalcManHost = UsesCalcMan ? string.Format("\"{0}:{1}\"", CalcManHostName, CalcManPort) : NULL_STRING;
            string fullImageGenHost = UsesImageGen ? string.Format("\"{0}:{1}\"", ImageGenHostName, ImageGenPort) : NULL_STRING;
            string fullImpinjHost = UsesImpinjReaderApp ? string.Format("\"{0}:{1}\"", ImpinjHostName, ImpinjAppReaderPort) : NULL_STRING;
            string fullClientGuid = "\"" + ClientGuid + "\"";

            ConfigString = string.Format("var middleware = new OmniMiddlewareClient({0}, {1}, {2}, {3});",
                fullCalcManHost, fullImageGenHost, fullImpinjHost, fullClientGuid);

            fullCalcManHost = UsesCalcMan ? string.Format("\"{0}:{1}\"", CalcManHostName, CalcManPort) : "\"\"";
            fullImageGenHost = UsesImageGen ? string.Format("\"{0}:{1}\"", ImageGenHostName, ImageGenPort) : "\"\"";
            fullImpinjHost = UsesImpinjReaderApp ? string.Format("\"{0}:{1}\"", ImpinjHostName, ImpinjAppReaderPort) : "\"\"";

            AppConfigString = string.Format("<configuration>{0}" +
                "  <appSettings>{1}" +
                "    <add key=\"CALCManServiceHostName\" value={2} />{3}" +
                "    <add key=\"ImageGenServiceHostName\" value={4} />{5}" +
                "    <add key=\"RFIDServiceHostName\" value={6} />{7}" +
                "    <add key=\"MiddlewareClientGuid\" value={8} />{9}" +
                "    ...",
                Environment.NewLine, Environment.NewLine, fullCalcManHost, Environment.NewLine,
                fullImageGenHost, Environment.NewLine,
                fullImpinjHost, Environment.NewLine,
                fullClientGuid, Environment.NewLine);
                
        }

        public void SetImpinjAppClientHost(string impinjAppClientHost)
        {
            if (!ImpinjAppClientHosts.Contains(impinjAppClientHost))
            {
                ImpinjAppClientHosts.Add(impinjAppClientHost);
                ImpinjAppClientHost = impinjAppClientHost;
            }
        }

        public void SetImpinjHostName(string impinjHostName)
        {            
            if (!ImpinjHostNames.Contains(impinjHostName))
            {
                ImpinjHostNames.Add(impinjHostName);
                ImpinjHostName = impinjHostName;
            }
        }

        #endregion
    }
}

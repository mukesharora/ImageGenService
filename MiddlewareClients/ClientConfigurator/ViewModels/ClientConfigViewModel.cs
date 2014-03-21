using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientConfigurator.Models;
using OmniWinIPC;
using OmniCfgSvcBLL;
using System.Windows.Input;
using OmniSysSchedSQLSDAL;
using System.Windows;
using System.Xml.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using ClientConfigurator.Utility;
using NLog;
using System.Data.Objects;
using System.IO;
using System.Reflection;
using OmniCfgSvcSQLSDAL;
using System.Collections;
using System.Data.Objects.DataClasses;

namespace ClientConfigurator.ViewModels
{

    /// <summary>
    /// View Model for the ClientConfigWindow.
    /// </summary>
    public class ClientConfigViewModel : ReactiveObject
    {
        #region Logging

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constants

        #endregion

        #region private members

        /// <summary>
        /// Configuration database.
        /// </summary>
        private IConfigurationParameters _configDB = null;        

        /// <summary>
        /// Database access to the application scheduler tables.
        /// </summary>
        private OmniSysSchedEntities _sysSchedEntities = null;

        #endregion

        #region ReactiveCommands

        /// <summary>
        /// Action to create a new client configuration.
        /// </summary>
        public IReactiveCommand NewClientCommand { get; protected set; }

        /// <summary>
        /// Action to delete a client configuration.
        /// </summary>
        public IReactiveCommand DeleteCommand { get; protected set; }

        /// <summary>
        /// Action to save a client configuration (client specific settings).
        /// </summary>
        public IReactiveCommand SaveCommand { get; protected set; }

        /// <summary>
        /// Action to save the singleton CALCMan settings (not client specific settings).
        /// </summary>
        public IReactiveCommand SaveCalcManConfigCommand { get; protected set; }

        /// <summary>
        /// Action to save the singleton imageGen settings (not client specific settings).
        /// </summary>
        public IReactiveCommand SaveImageGenConfigCommand { get; protected set; }

        /// <summary>
        /// Action to save configuration.
        /// </summary>
        public IReactiveCommand SaveAllCommand { get; protected set; }

        /// <summary>
        /// Action when configuration string (constructor) needs to be copied to the clipboard.
        /// </summary>
        public IReactiveCommand CopyConfigString { get; protected set; }

        /// <summary>
        /// Action when app configuration string (app.settings) needs to be copied to the clipboard.
        /// </summary>
        public IReactiveCommand CopyAppConfigString { get; protected set; }

        /// <summary>
        /// Action to export configuration data to text file.
        /// </summary>
        public IReactiveCommand ExportCommand { get; protected set; }        

        #endregion

        #region Constructors

        public ClientConfigViewModel()
        {
            try
            {
                IsBusy = true;

                // Initialize Reactive commands
                ///
                NewClientCommand = new ReactiveCommand();
                NewClientCommand.Subscribe(_ => CreateNewClient());

                DeleteCommand = new ReactiveCommand();
                DeleteCommand.Subscribe(clientGuid => DeleteClient(clientGuid as string));

                SaveCommand = new ReactiveCommand();
                SaveCommand.Subscribe(clientGuid => SaveClient(clientGuid as string));

                SaveCalcManConfigCommand = new ReactiveCommand();
                SaveCalcManConfigCommand.Subscribe(_ => SaveCalcManConfig());

                SaveImageGenConfigCommand = new ReactiveCommand();
                SaveImageGenConfigCommand.Subscribe(_ => SaveImageGenConfig());

                SaveAllCommand = new ReactiveCommand();
                SaveAllCommand.Subscribe(_ => SaveAll());

                ExportCommand = new ReactiveCommand();
                ExportCommand.Subscribe(_ => ExportConfiguration());

                // Setup action commands.
                //
                CopyConfigString = new ReactiveCommand();
                CopyConfigString.ObserveOnDispatcher().Subscribe(p =>
                {
                    try
                    {
                        ClientConfigItem item = p as ClientConfigItem;
                        if (item != null)
                        {
                            Clipboard.SetData(DataFormats.Text, item.ConfigString);                            
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("unhandled in CopyConfigString action", ex);
                        //WarnUserCommand.Publish("Error accessing clipboard");
                        UserError.Throw("Error accessing clipboard");
                    }
                });

                CopyAppConfigString = new ReactiveCommand();                
                CopyAppConfigString.ObserveOnDispatcher().Subscribe(p =>
                {
                    try
                    {
                        ClientConfigItem item = p as ClientConfigItem;
                        if (item != null)
                        {
                            Clipboard.SetData(DataFormats.Text, item.AppConfigString);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("unhandled in CopyAppConfigString action", ex);
                        UserError.Throw("Error accessing clipboard");                        
                    }
                });                

                // Initialize database access.
                //
                _configDB = new OmniConfigParameters(ClientConfigConstants.AppName, ClientConfigConstants.AppGUID);
                _sysSchedEntities = OmniSysSchedDAL.GetContext();

                // Create model
                //
                ClientConfigList = new ObservableCollection<ClientConfigItem>();
                CalcManConfig = new Models.CalcManConfig();
                ImageGenConfig = new Models.ImageGenConfig();

                IsBusy = false;                
            }
            catch (Exception ex)
            {
                logger.ErrorException("unhandled in ClientConfigViewModel", ex);
            }
            
            // Test Exceptions
            //(new System.Threading.Thread(() =>
            //{
            //    throw new Exception("hello");
            //})).Start();
            //throw new Exception("hello");
        }

        #endregion

        #region Events

        /// <summary>
        /// Event handler for when a ClientConfigItem changes.
        /// Used to keep Impinj reader host names and client host names in sync.
        /// </summary>
        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "ImpinjAppClientHost") || (e.PropertyName == "ImpinjHostName"))
            {
                ClientConfigItem item = sender as ClientConfigItem;
                if (sender != null)
                {
                    string impinjAppClientHost = item.ImpinjAppClientHost;
                    string impinjHostName = item.ImpinjHostName;
                    if ((impinjAppClientHost != null) && (impinjHostName != null))
                    {
                        foreach (ClientConfigItem ccItem in ClientConfigList)
                        {
                            ccItem.SetImpinjAppClientHost(impinjAppClientHost);                            
                            ccItem.SetImpinjHostName(impinjHostName);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// List of client configurations.
        /// </summary>
        public ObservableCollection<ClientConfigItem> ClientConfigList { get; set; }
        
        /// <summary>
        /// IsDirty flag, used to check if we can cleanly exit the application.
        /// </summary>
        public bool IsDirty 
        {
            get
            {
                bool isDirty = false;
                if ( (ClientConfigList != null) && (CalcManConfig != null) && (ImageGenConfig != null))
                {
                    isDirty = ClientConfigList.Any(c => c.IsDirty) || CalcManConfig.IsDirty || ImageGenConfig.IsDirty;
                }
                return isDirty;
            }
        }

        /// <summary>
        /// IsBusy. Methods should set this when doing long operations to change the mouse pointer appropriately.
        /// </summary>
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { this.RaiseAndSetIfChanged(ref _isBusy, value); }
        }

        /// <summary>
        /// Model for singleton (non-client specific) portion of CALCMan configuration.
        /// </summary>
        private CalcManConfig _calcManConfig;
        public CalcManConfig CalcManConfig
        {
            get { return _calcManConfig; }
            set { this.RaiseAndSetIfChanged(ref _calcManConfig, value); }
        }

        /// <summary>
        /// Model for singleton (non-client specific) portion of ImageGen configuration.
        /// </summary>
        private ImageGenConfig _imageGenConfig;
        public ImageGenConfig ImageGenConfig
        {
            get { return _imageGenConfig; }
            set { this.RaiseAndSetIfChanged(ref _imageGenConfig, value); }
        }   

        #endregion

        #region Private methods

        /// <summary>
        /// Export configuration (for debugging purposes).
        /// </summary>
        private void ExportConfiguration()
        {
            try
            {
                // Get filename
                string path = Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);

                path += @"\Omni-id\ClientConfigurator\";
                Directory.CreateDirectory(path);
                path += "ClientConfiguratorExport_" + DateTime.Now.ToString("MMddyyyy_hhmmss") + ".txt";

                // Ignore these properties when exporting entities.
                List<string> ignoreProperties = new List<string>() 
                { 
                    "EntityKey", 
                    "EntityState",
                    "Reference"
                };

                using (StreamWriter exportFile = new StreamWriter(path))
                {
                    // Export schedule.
                    //                    
                    using (OmniSysSchedEntities schedulerEntities = OmniSysSchedDAL.GetContext())
                    {
                        ExportEntity("OmniSysSchedEntities", schedulerEntities.ScheduledApps, exportFile, ignoreProperties);
                    }

                    // Export Configuration tables
                    //                    
                    using (AppConfigParametersEntities appParameters = OmniCfgDAL.GetContext())
                    {
                        ExportEntity("Applications", appParameters.Applications, exportFile, ignoreProperties);
                        ExportEntity("Parameters", appParameters.Parameters, exportFile, ignoreProperties);
                        ExportEntity("ParameterCategories", appParameters.ParameterCategories, exportFile, ignoreProperties);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.ErrorException("Unhandled in ExportConfiguration", ex);
            }
        }

        /// <summary>
        /// Export a collection as a comma delimited list with a header. Right now we are getting some columns
        /// from the entity framework that we don't need, so we will have to ignore them for now.
        /// </summary>
        private void ExportEntity(string entityName, IEnumerable col, StreamWriter sw, List<string> ignoreProperties)
        {
            sw.WriteLine(entityName + ":");

            bool writeHeader = true;
            foreach (var item in col ?? Enumerable.Empty<string>())
            {
                string text = null;
                if (writeHeader)
                {
                    text = ObjectNamesToCsvData(item, ignoreProperties);
                    sw.WriteLine(text);
                    writeHeader = false;
                }

                text = ObjectToCsvData(item, ignoreProperties);
                sw.WriteLine(text);
            }            
        }

        /// <summary>
        /// Creates a comma delimited string of all the objects property names.
        /// </summary>
        private string ObjectNamesToCsvData(object obj, List<string> ignoreProperties)
        {
            if (obj == null)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties();

            bool needComma = false;
            for (int index = 0; index < pi.Length; index++)
            {              
                if ( (ignoreProperties.FirstOrDefault(p => pi[index].Name.Contains(p)) == null))
                {
                    if (needComma)
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        needComma = true;
                    }

                    sb.Append(pi[index].Name);                    
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a comma delimited string of all the objects property values names.
        /// </summary>
        private string ObjectToCsvData(object obj, List<string> ignoreProperties)
        {
            if (obj == null)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties();

            bool needComma = false;
            for (int index = 0; index < pi.Length; index++)
            {
                if (ignoreProperties.FirstOrDefault(p => pi[index].Name.Contains(p)) == null)
                {
                    if (needComma)
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        needComma = true;
                    }

                    sb.Append(pi[index].GetValue(obj, null));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Create a new client configuration.
        /// The Impinj reader will be given a new unique port and saved.
        /// </summary>
        private void CreateNewClient()
        {
            try
            {
                IsBusy = true;

                ClientConfigItem item = new ClientConfigItem();
                item.PropertyChanged += item_PropertyChanged;

                AssignUniquePort(item);
                item.GenerateConfigString(CalcManConfig.CalcManHostName, CalcManConfig.Port, ImageGenConfig.ImageGenHostName, ImageGenConfig.Port);

                ClientConfigList.Add(item);

                item.Save(_configDB, _sysSchedEntities);
                item.IsDirty = false;                

                IsBusy = false;
            }
            catch (Exception ex)
            {
                logger.ErrorException("Unhandled in CreateNewClient", ex);
            }

            // Test Exceptions
            //(new System.Threading.Thread(() =>
            //{
            //    throw new Exception("hello");
            //})).Start();
            //throw new Exception("hello");
        }

        /// <summary>
        /// Assigned a unique port to the Impinj reader.
        /// </summary>
        /// <param name="item"></param>
        private void AssignUniquePort(ClientConfigItem item)
        {
            int port = ClientConfigItem.DEFAULT_IMPINJAPP_PORT;

            item.ImpinjAppReaderPort = port.ToString();

            while ( ClientConfigList.Count(cc => cc.ImpinjAppReaderPort == item.ImpinjAppReaderPort) > 0)
            {
                port++;
                item.ImpinjAppReaderPort = port.ToString();
            }

        }

        /// <summary>
        /// Saves the ImageGen settings. These are the non-client specific 
        /// setting.
        /// </summary>
        private void SaveImageGenConfig()
        {
            IsBusy = true;
            try
            {
                bool ok = ImageGenConfig.Save(_configDB, ClientConfigConstants.CLIENT_CONFIG_CATEGORY);
                if (!ok)
                {
                    // TODO message to user
                }
            }
            catch (Exception ex)
            {
                logger.WarnException("unhandled in SaveImageGenConfig", ex);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Saves the entire configuration.
        /// </summary>
        private void SaveAll()
        {
            IsBusy = true;
            try
            {
                bool ok = ImageGenConfig.Save(_configDB, ClientConfigConstants.CLIENT_CONFIG_CATEGORY);
                ok = CalcManConfig.Save(_configDB, ClientConfigConstants.CLIENT_CONFIG_CATEGORY);

                ClientConfigList.ToList().ForEach(c => c.Save(_configDB, _sysSchedEntities));

                if (!ok)
                {
                    // TODO message to user
                }
            }
            catch (Exception ex)
            {
                logger.WarnException("unhandled in SaveImageGenConfig", ex);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Saves the CALCMan settings. These are the non-client specific 
        /// setting.
        /// </summary>
        private void SaveCalcManConfig()
        {
            IsBusy = true;
            try
            {
                bool ok = CalcManConfig.Save(_configDB, ClientConfigConstants.CLIENT_CONFIG_CATEGORY);
                if (!ok)
                {
                    // TODO message to user
                }
            }
            catch (Exception ex)
            {
                logger.WarnException("unhandled in SaveCalcManConfig", ex);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Saves a client configuration.
        /// </summary>
        /// <param name="clientGuid">GUID of the client configuration to save.</param>
        private void SaveClient(string clientGuid)
        {
            IsBusy = true;
            try
            {
                if (clientGuid != null)
                {
                    ClientConfigItem item = FindClient(clientGuid);
                    if (item != null)
                    {
                        item.Save(_configDB, _sysSchedEntities);
                    }
                }
                else
                {
                    // TODO log error
                }
            }
            catch (Exception ex)
            {
                logger.WarnException("unhandled in SaveClient", ex);
            }

            IsBusy = false;
        }

        /// <summary>
        /// Delete a client configuration.
        /// </summary>
        /// <param name="clientGuid">GUID of the client configuration to delete.</param>
        private void DeleteClient(string clientGuid)
        {
            // MessageBox should be initiated from View or some some
            // of user input service, for now it goes here.

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this client configuration? You will loose all Omni Impinj reader definitions.", "Delete record?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (clientGuid != null)
                    {
                        ClientConfigItem item = FindClient(clientGuid);
                        if (item != null)
                        {
                            IsBusy = true;

                            ClientConfigList.Remove(item);
                            item.Delete(_configDB, _sysSchedEntities);

                            IsBusy = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.WarnException("unhandled in DeleteClient", ex);
                }
            }
        }

        /// <summary>
        /// Loads the model
        /// </summary>
        public void LoadModel()
        {
            IsBusy = true;

            try
            {
                // Load CalcMan configuration
                //
                CalcManConfig.Load(_configDB, ClientConfigConstants.CLIENT_CONFIG_CATEGORY);

                // Setup handlers to detect when something in the model changes that effects the
                // ConfigString and AppConfigString. If might have been better to do this
                // with multi-binding.
                //
                var propertyChangedEvent1 = from evt in Observable.FromEventPattern<PropertyChangedEventArgs>(CalcManConfig, "PropertyChanged") select evt;
                var propertyChanged1 = from evt in propertyChangedEvent1
                                select evt;
                propertyChanged1.ObserveOnDispatcher().Subscribe(value =>
                {
                    ClientConfigList.ToList().ForEach(c => c.GenerateConfigString(CalcManConfig.CalcManHostName, CalcManConfig.Port, ImageGenConfig.ImageGenHostName, ImageGenConfig.Port));
                });

                var propertyChangedEvent2 = from evt in Observable.FromEventPattern<PropertyChangedEventArgs>(ImageGenConfig, "PropertyChanged") select evt;
                var propertyChanged2 = from evt in propertyChangedEvent2
                                       select evt;
                propertyChanged2.ObserveOnDispatcher().Subscribe(value =>
                {
                    ClientConfigList.ToList().ForEach(c => c.GenerateConfigString(CalcManConfig.CalcManHostName, CalcManConfig.Port, ImageGenConfig.ImageGenHostName, ImageGenConfig.Port));
                });
                

                // Load ImageGen configuration
                //
                ImageGenConfig.Load(_configDB, ClientConfigConstants.CLIENT_CONFIG_CATEGORY);

                // Get client categories
                //
                List<string> categories = _configDB.GetCategories();
                List<string> clientCategories = categories.FindAll(c => (c != null) && (c.StartsWith(ClientConfigItem.CategoryNamePrefix)));
                foreach (string clientCategory in clientCategories)
                {
                    ClientConfigItem item = ClientConfigItem.Load(_configDB, clientCategory);
                    item.PropertyChanged += item_PropertyChanged;
                    ClientConfigList.Add(item);
                }
            }
            catch (Exception ex)
            {
                logger.WarnException("unhandled in LoadModel", ex);
            }

            IsBusy = false;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns the ClientConfigItem for a given client GUID.
        /// </summary>
        private ClientConfigItem FindClient(string clientGuid)
        {
            return ClientConfigList.First(configItem => ((configItem != null) && (configItem.ClientGuid == clientGuid)));
        }

        #endregion        
    }


}

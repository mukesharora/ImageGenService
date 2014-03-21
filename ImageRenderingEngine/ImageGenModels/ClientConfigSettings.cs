using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniWinIPC;
using OmniCfgSvcBLL;
using ClientConfigurator.Utility;

namespace ImageGenModels
{
    /// <summary>
    /// Utility and model class for settings from the Middleware
    /// Client Configuration Utility.
    /// </summary>
    public class ClientConfigSettings
    {
        #region Private static variables

        private static ClientConfigSettings _instance = null;

        //private static readonly ILog Logger = LogManager.GetLogger(typeof(ClientConfigSettings));

        #endregion

        #region Constructor

        private ClientConfigSettings()
        {
            _instance = null;
            Port = null;
        }

        #endregion

        #region Singleton access property

        public static ClientConfigSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClientConfigSettings();
                    if (!_instance.Init())
                    {
                        _instance = null;
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Public properties

        public string Port { get; set; }

        public bool HasConfiguration
        {
            get
            {
                return (Port != null);
            }
        }

        #endregion

        #region Private methods

        private bool Init()
        {
            bool ok = false;

            try
            {
                IConfigurationParameters configDB = new OmniConfigParameters(ClientConfigConstants.AppName, ClientConfigConstants.AppGUID);

                Port = GetConfigValue(configDB, ClientConfigConstants.CLIENT_CONFIG_CATEGORY, ClientConfigConstants.IG_PORT_PARAM_NAME);
                ok = true;
            }
            catch (Exception)
            {
                //Logger.Warn("Unexpected exception in Init", ex);
            }

            return ok;
        }

        internal string GetConfigValue(IConfigurationParameters configDB, string category, string paramName)
        {
            string value = null;

            try
            {
                value = configDB.Get(category, paramName);
                if (value == "NOT FOUND")
                {
                    value = null;
                }
            }
            catch (Exception)
            {
                //Logger.Warn("Unexpected exception in GetConfigValue", ex);
            }

            return value;
        }

        #endregion
    }
}

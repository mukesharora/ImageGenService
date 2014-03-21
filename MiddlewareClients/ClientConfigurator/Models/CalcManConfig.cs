using ClientConfigurator.Utility;
using OmniCfgSvcBLL;
using OmniWinIPC;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientConfigurator.Models
{
    public class CalcManConfig : ReactiveObject
    {
        #region Constants

        private const int DEFAULT_PORT = 3000;        

        #endregion

        #region Constructor(s)

        public CalcManConfig()
        {
            Port = "Not set";

            CalcManHostNames = new ObservableCollection<string>();
            CalcManHostNames.Add(App.LOCAL_HOST);
            CalcManHostName = CalcManHostNames.ElementAt(0);
        }

        #endregion


        #region Public properties

        public ObservableCollection<string> CalcManHostNames { get; set; }

        /// <summary>
        /// Port that CALCMan is running on.
        /// 
        /// Middleware needs this to construct a new MiddlewareClient object.
        /// CalcMan needs this to communicate with the CALCs.
        /// </summary>
        private string _port;
        public string Port
        {
            get { return _port; }
            set
            {
                IsDirty = true;
                this.RaiseAndSetIfChanged(ref _port, value);
            }
        }

        /// <summary>
        /// Name of machine that CalcMan is running on. 
        /// Middleware needs this to construct a new MiddlewareClient object.
        /// Set to localhost if all clients are running on this machine.
        /// </summary>
        private string _calcManHostName;
        public string CalcManHostName
        {
            get { return _calcManHostName; }
            set
            {
                //if ((value != null) && value.StartsWith("a"))
                //{
                //    throw new ApplicationException("Customer name is mandatory.");
                //}
                //else
                //{
                this.RaiseAndSetIfChanged(ref _calcManHostName, value);                
                IsDirty = true;                
                //}
            }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { this.RaiseAndSetIfChanged(ref _isDirty, value); }
        }

        #endregion

        #region Public methods

        public bool Load(IConfigurationParameters clientConfigDb, string category)
        {
            bool ok = false;

            try
            {
                string calcManPort = clientConfigDb.Get(category, ClientConfigConstants.CM_PORT_PARAM_NAME);
                if (calcManPort == "NOT FOUND")
                {
                    Port = DEFAULT_PORT.ToString();
                    clientConfigDb.Set(category, ClientConfigConstants.CM_PORT_PARAM_NAME, Port);
                }
                else
                {
                    Port = calcManPort;                    
                }

                string hostname = clientConfigDb.Get(category, ClientConfigConstants.CM_HOSTNAME_PARAM_NAME);
                if (hostname == "NOT FOUND")
                {
                    clientConfigDb.Set(category, ClientConfigConstants.CM_HOSTNAME_PARAM_NAME, CalcManHostName);
                }
                else
                {
                    CalcManHostName = hostname;

                    if (CalcManHostName != App.LOCAL_HOST)
                    {
                        CalcManHostNames.Add(CalcManHostName);                        
                    }
                }

                ok = true;
                IsDirty = false;
            }
            catch (Exception ex)
            {
                // TODO log
            }

            return ok;
        }

        public bool Save(IConfigurationParameters clientConfigDb, string category)
        {
            bool ok = false;

            try
            {
                clientConfigDb.Set(category, ClientConfigConstants.CM_PORT_PARAM_NAME, Port);

                clientConfigDb.Set(category, ClientConfigConstants.CM_HOSTNAME_PARAM_NAME, CalcManHostName);

                IsDirty = false;
                ok = true;
            }
            catch (Exception ex)
            {
                // TODO log error
            }

            return ok;
        }

        #endregion
    }
}

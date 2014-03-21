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
    public class ImageGenConfig : ReactiveObject
    {

        #region Constants

        private const int DEFAULT_PORT = 30525;                
        private const string HOSTNAME_PARAM_NAME = "ImageGen Hostname";

        #endregion

        #region Constructor(s)

        public ImageGenConfig()
        {
            Port = "Not set";

            ImageGenHostNames = new ObservableCollection<string>();
            ImageGenHostNames.Add(App.LOCAL_HOST);
            ImageGenHostName = ImageGenHostNames.ElementAt(0);
        }

        #endregion

        #region Public properties

        public ObservableCollection<string> ImageGenHostNames { get; set; }

        /// <summary>
        /// Port that ImageGen is running on.
        /// 
        /// Middleware needs this to construct a new MiddlewareClient object.
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
        /// Name of machine that ImageGen is running on. 
        /// Middleware needs this to construct a new MiddlewareClient object.
        /// Set to localhost if all clients are running on this machine.
        /// </summary>
        private string _imageGenHostName;
        public string ImageGenHostName
        {
            get { return _imageGenHostName; }
            set
            {
                this.RaiseAndSetIfChanged(ref _imageGenHostName, value);
                IsDirty = true;
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
                string imageGenPort = clientConfigDb.Get(category, ClientConfigConstants.IG_PORT_PARAM_NAME);
                if (imageGenPort == "NOT FOUND")
                {
                    Port = DEFAULT_PORT.ToString();
                    clientConfigDb.Set(category, ClientConfigConstants.IG_PORT_PARAM_NAME, Port);
                }
                else
                {
                    Port = imageGenPort;                    
                }

                string hostname = clientConfigDb.Get(category, HOSTNAME_PARAM_NAME);
                if (hostname == "NOT FOUND")
                {
                    clientConfigDb.Set(category, HOSTNAME_PARAM_NAME, ImageGenHostName);
                }
                else
                {
                    ImageGenHostName = hostname;

                    if (ImageGenHostName != App.LOCAL_HOST)
                    {
                        ImageGenHostNames.Add(ImageGenHostName);                        
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
                clientConfigDb.Set(category, ClientConfigConstants.IG_PORT_PARAM_NAME, Port);
                clientConfigDb.Set(category, HOSTNAME_PARAM_NAME, ImageGenHostName);

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

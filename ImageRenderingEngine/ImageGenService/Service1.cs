using System;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using ImageGenWebApi;

namespace ImageGenService
{
    public partial class ImageGen : ServiceBase
    {
        private ImageGenWebApiService service;
        
        public ImageGen()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {            
            string host = ConfigurationManager.AppSettings["ImageGenServiceHostName"];
            service = new ImageGenWebApiService(host);
            service.Start(); 
        }

        protected override void OnStop()
        {
            service.Stop();
        }
    }
}

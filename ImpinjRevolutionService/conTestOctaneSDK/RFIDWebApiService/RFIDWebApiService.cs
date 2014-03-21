using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RFIDWebApiService
{
    public class RFIDWebApiService
    {
        private HttpSelfHostServer server;
        private HttpSelfHostConfiguration config;
        
        public void Start(string hostName)
        {
            Application_Start(hostName);
            server.OpenAsync().Wait();
        }

        public void Stop()
        {
            server.CloseAsync().Wait();
            server.Dispose();
        }

        protected void Application_Start(string hostName)
        {            
            config = new HttpSelfHostConfiguration(hostName);
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            WebApiConfig.Register(config);
            server = new HttpSelfHostServer(config);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);            
        }
    }
}
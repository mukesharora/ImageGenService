using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.SelfHost;
using System.Web.Http.Services;
using System.Web.Http;
using ImageGenWebApi.Formatters;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using ImageGenModels;
using System.Net.Http;

[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace ImageGenWebApi
{
    public class ImageGenWebApiService
    {
        private readonly HttpSelfHostServer server;
        private readonly HttpSelfHostConfiguration config;
        public static readonly string TEMPLATE_ROUTE_NAME = "TemplateApi";
        public static readonly string IMAGE_ROUTE_NAME = "ImageApi";
        public static readonly string CUSTOM_IMAGE_ROUTE_NAME = "CustomeImageApi";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ImageGenWebApiService));

        public ImageGenWebApiService(string baseAddress)
        {
            ClientConfigSettings settings = ClientConfigSettings.Instance;
            if (settings.HasConfiguration)
            {
                baseAddress = string.Format("http://localhost:{0}", settings.Port);
            }

            config = new HttpSelfHostConfiguration(baseAddress);
            //config.Filters.Add(new ValidateFilterAttribute());
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.Routes.MapHttpRoute(
                name: TEMPLATE_ROUTE_NAME,
                routeTemplate: "api/template/{coralType}",
                defaults: new { controller = "Template", coralType = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: IMAGE_ROUTE_NAME,
                routeTemplate: "api/image/{imageName}",
                defaults: new { controller = "Image", imageName = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
               name: CUSTOM_IMAGE_ROUTE_NAME,
               routeTemplate: "api/customimage/{action}/{labelTemplateID}/{assetID}",

               defaults: new { controller = "CustomImage", labelTemplateID = RouteParameter.Optional, assetID = RouteParameter.Optional });


            //config.Routes.MapHttpRoute(
            //    name: "CustomeImageApi2",
            //    routeTemplate: "api/customimage/test/{id}",
            //    defaults: new { controller = "CustomImage", id = RouteParameter.Optional });

            config.Formatters.Add(new PlainTextFormatter());

            server = new HttpSelfHostServer(config);
        }

        public void Start()
        {
            LogAppVersion();

            Logger.Info("Logging successfully initiated.");
            try
            {
                server.OpenAsync().Wait();
                Logger.Info("Service started.");
            }
            catch (AggregateException ex)
            {
                Logger.Error("Error starting service.", ex.InnerException);
            }
        }

        public void Stop()
        {
            server.CloseAsync().Wait();
            server.Dispose();
        }

        #region private methods

        /// <summary>
        /// Logs the application version.
        /// </summary>
        private void LogAppVersion()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

                Logger.Info(string.Format("ImageGenService startup. File version: {0}", fvi.FileVersion));
            }
            catch (Exception ex)
            {
                Logger.Info(string.Format("Unexpected exception in LogAppVersion: ", ex.ToString()));
            }
        }

        #endregion
    }
}

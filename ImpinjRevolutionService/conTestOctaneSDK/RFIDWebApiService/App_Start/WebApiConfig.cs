using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RFIDWebApiService
{
    public static class WebApiConfig
    {
        public static readonly string READER_ROUTE_NAME = "ReaderApi";
        
        public static void Register(HttpConfiguration config)
        {
			//config.Routes.MapHttpRoute(
			//    name: READER_ROUTE_NAME,
			//    routeTemplate: "api/Reader/{id}",
			//    defaults: new { id = RouteParameter.Optional, controller = "Reader" }
			//);

			config.Routes.MapHttpRoute(
				name: READER_ROUTE_NAME,
				routeTemplate: "api/Reader/{id}/{gpiPort}",
				defaults: new { id = RouteParameter.Optional, gpiPort = RouteParameter.Optional, controller = "Reader" }
			);
        }
    }
}

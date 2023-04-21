using GMAH.Web.Helpers.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace GMAH.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // add multipart/form-data formatter
            config.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter());
        }
    }
}

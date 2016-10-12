using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;

namespace kuujinbo.StackOverflow.iTextSharp.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public const string NAV_MENU = "NAV_MENU";
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Application[NAV_MENU] = File.ReadAllText(
                Server.MapPath(string.Format("~/content/{0}.html", NAV_MENU))
            );
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
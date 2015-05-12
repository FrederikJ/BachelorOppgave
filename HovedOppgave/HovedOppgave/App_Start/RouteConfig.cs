using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HovedOppgave
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "Send more idss",
                url: "{controller}/{action}/{id1}/{id2}",
                defaults: new { controller = "Reports", action = "CheckUser" },
                constraints: new { id1 = @"\d+", id2 = @"\d+" }
            );
        }
    }
}

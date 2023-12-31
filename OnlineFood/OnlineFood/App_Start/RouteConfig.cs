using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineFood
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
                name: "AdminCreate",
                url: "Admin/Create",
                defaults: new { controller = "Admin", action = "Create" }
            );

            routes.MapRoute(
                name: "AdminEdit",
                url: "Admin/Edit/{id}",
                defaults: new { controller = "Admin", action = "Edit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AdminDelete",
                url: "Admin/Delete/{id}",
                defaults: new { controller = "Admin", action = "Delete", id = UrlParameter.Optional }
            );

        }
    }
}

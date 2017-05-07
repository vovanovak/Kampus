using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kampus
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Main", 
                "main", 
                new { controller = "Main", action = "Index" });

            routes.MapRoute("SignIn",
                "signin", 
                new { controller = "Main", action = "SignIn" },
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
                new string[] { "Kampus.Controllers" });

            routes.MapRoute("Logout",
                "logout",
                new { controller = "User", action = "Logout" });

            routes.MapRoute("RegisterStep1", 
                "register/step-1", 
                new { controller = "Register", action = "Index" },
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
                new string[] { "Kampus.Controllers" });

            routes.MapRoute("RegisterStep2",
                "register/step-2",
                new { controller = "Register", action = "Step2" },
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
                new string[] { "Kampus.Controllers" });

            routes.MapRoute("RegisterStep3",
                "register/step-3",
                new { controller = "Register", action = "Step3" },
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
                new string[] { "Kampus.Controllers" });

            

            routes.MapRoute("RegisterStep4",
                "register/step-4",
                new { controller = "Register", action = "Step4" },
                new RouteValueDictionary(new { httpMethod = new HttpMethodConstraint("GET") }),
                new string[] { "Kampus.Controllers" });

            routes.MapRoute("Messages",
                "messages",  
                new { controller = "Message", action = "Index" });

            routes.MapRoute("Settings",
                "settings",
                new { controller = "Settings", action = "Index" });

            routes.MapRoute("Home",
                "home",
                new { controller = "User", action = "Index" });

            routes.MapRoute("Profile",
                "profiles/{id}",
                new { controller = "User", action = "Id" });

            routes.MapRoute("Friends",
                "my/friends",
                new { controller = "User", action = "Friends" });

            routes.MapRoute("FriendsById",
               "profiles/{id}/friends/",
               new { controller = "User", action = "FriendsById" });

            routes.MapRoute("Subscribers",
                "my/subscribers",
                new { controller = "User", action = "Subscribers" });

            routes.MapRoute("MyTasks", 
                "my/tasks",
                new { controller = "Task", action = "ViewHomeTasks" });

            routes.MapRoute("AllTasks", 
                "tasks",
                new { controller = "Task", action = "Index" });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Main", action = "Index", id = UrlParameter.Optional },
                new string[] { "Kampus.Controllers" }
            );
        }
    }
}
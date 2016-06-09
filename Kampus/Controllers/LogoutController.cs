using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kampus.Controllers
{
    public class LogoutController : Controller
    {
        //
        // GET: /Logout/

        public ActionResult Index()
        {
            Session.Clear();
            Response.Cookies.Clear();
            return RedirectToAction("Index", "SignIn");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Kampus.DAL;
using Kampus.DAL.Concrete;
using Kampus.Models;

namespace Kampus.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/

        //private UserRepositoryBase db = Kampus.Container.Autofac.Container.Resolve<UserRepositoryBase>();

        public ActionResult Index()
        {
            return View();
        }
    }
}

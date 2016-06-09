using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Kampus.DAL;
using Kampus.DAL.Abstract;
using Kampus.DAL.Concrete;

namespace Kampus.Controllers
{
    public class SignInController : Controller
    {
        //
        // GET: /SignIn/

        private readonly IUserRepository _dbUser =
            Kampus.Container.Autofac.Container.Resolve<IUserRepository>();


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string SignIn(string username, string password)
        {
            SignInResult res = _dbUser.SignIn(username, password);

            if (res == SignInResult.Successful)
            {
                ViewBag.CurrentUser = _dbUser.GetByUsername(username);
                Session.Add("CurrentUser", ViewBag.CurrentUser);
                Session.Add("CurrentUserId", ViewBag.CurrentUser.Id);
            }

            return res.ToString();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Kampus.DAL.Abstract;
using Kampus.DAL.Concrete;
using Kampus.Models;

namespace Kampus.Controllers
{
    public class SettingsController : Controller
    {
        //
        // GET: /Settings/
        private readonly IUserRepository _dbUser =
            Kampus.Container.Autofac.Container.Resolve<IUserRepository>();
        private readonly IUniversityRepository _dbUniversity =
           Kampus.Container.Autofac.Container.Resolve<IUniversityRepository>();
        private readonly ICityRepository _dbCity =
            Kampus.Container.Autofac.Container.Resolve<ICityRepository>();

        public ActionResult Index()
        {
            InitViewBag();

            return View();
        }

        public void InitViewBag()
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));
            List<CityModel> cities = _dbCity.GetAll();
            List<UniversityModel> universities = _dbUniversity.GetAll();
            List<UniversityFacultyModel> faculties = universities.ElementAt(0).Faculties;

            ViewBag.CurrentUser = user;
            ViewBag.Cities = cities;
            ViewBag.Universities = universities;
            ViewBag.Faculties = faculties;
        }

        #region Change Avatar

        public string GetEncodedHash(string path)
        {
            const string salt = "adhasdhasdhas";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(path + salt));
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            return base64digest;
        }

        [HttpPost]
        public ActionResult ChangeAvatar(HttpPostedFileBase file)
        {
            InitViewBag();

            int userid = Convert.ToInt32(Session["CurrentUserId"]);


            if (file != null)
            {

                string filename = GetEncodedHash(DateTime.Now.Ticks.ToString());
                filename = filename.Replace("\\", "a");
                filename = filename.Replace("/", "a");
                filename = filename.Replace("+", "b");

                string ext = file.FileName.Substring(file.FileName.LastIndexOf("."));

                file.SaveAs(HttpContext.Server.MapPath("~/Images/")
                            + filename + ext);

                string path = "/Images/" + filename + ext;

                _dbUser.SetAvatar(userid, path);

                ViewBag.CurrentUser.Avatar = path;
            }

            return View("Index");
        }

        #endregion

        #region Change Password

        [HttpPost]
        public ActionResult ChangePassword(string oldpassword, string newpassword, string newpasswordconfirm)
        {
            InitViewBag();

            int userid = Convert.ToInt32(Session["CurrentUserId"]);

            _dbUser.ChangePassword(userid, oldpassword, newpassword, newpasswordconfirm);

            return View("Index");
        }

        #endregion

        #region Change Status

        [HttpPost]
        public ActionResult ChangeStatus(string status)
        {
            InitViewBag();

            int userid = Convert.ToInt32(Session["CurrentUserId"]);

            _dbUser.ChangeStatus(userid, status);

            return View("Index");
        }


        #endregion

        #region Change City

        [HttpPost]
        public ActionResult ChangeCity(string city)
        {
            InitViewBag();

            int userid = Convert.ToInt32(Session["CurrentUserId"]);

            _dbUser.ChangeCity(userid, city);

            return View("Index");
        }

        #endregion

        #region Change Student Info

        [HttpPost]
        public ActionResult ChangeStudentInfo(string university, string faculty, int course)
        {
            InitViewBag();

            int userid = Convert.ToInt32(Session["CurrentUserId"]);

            _dbUser.ChangeStudentInfo(userid, university, faculty, course);

            return View("Index");
        }

        #endregion

        #region Password Recovery

        public ActionResult PasswordRecovery()
        {
            return View();
        }

        public void RecoverPassword(string username, string email)
        {
            string path = Url.Action("PassRecover", "Settings", null, Request.Url.Scheme) + "?str=";
            _dbUser.RecoverPassword(username, email, path);
        }

        public ActionResult PassRecover(string str)
        {
            string username = _dbUser.ContainsRecoveryWithSuchHash(str);
            
            if (username != null)
            {
                Session.Add("RecoveryUsername", username);
                return View();
            }
            return View("Error");
        }

        [HttpPost]
        public int TotalRecover(string password, string password1)
        {
            if (password == password1)
            {
                string username = Session["RecoveryUsername"] as string;
                _dbUser.SetNewPassword(username, password);
                return 1;
            }
            return 0;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Kampus.DAL.Abstract;
using Kampus.Models;
using Kampus.DAL;
using Kampus.DAL.Security;
using Kampus.Api.Controllers;

namespace Kampus.Controllers
{
    public class SettingsController : Controller
    {
        //
        // GET: /Settings/
        private IUnitOfWork _unitOfWork;
        public SettingsController()
        {
            _unitOfWork = UnitOfWorkResolver.UnitOfWork;
        }

        public ActionResult Index()
        {
            InitViewBag();

            return View();
        }

        public void InitViewBag()
        {
            UserModel user =.HttpContext.Session[ HttpContext.Session[.CurrentUser] as UserModel;
            List<CityModel> cities = _unitOfWork.Cities.GetAll();
            List<UniversityModel> universities = _unitOfWork.Universities.GetAll();
            List<UniversityFacultyModel> faculties = universities.ElementAt(0).Faculties;

            ViewBag.CurrentUser = user;
            ViewBag.Cities = cities;
            ViewBag.Universities = universities;
            ViewBag.Faculties = faculties;
        }

        #region Change Avatar
        
        [HttpPost]
        public ActionResult ChangeAvatar(HttpPostedFileBase file)
        {
            InitViewBag();

            int userid = Convert.ToInt32(Session["CurrentUserId"]);


            if (file != null)
            {
                string path = FileController.SaveImage(HttpContext, file);
                _unitOfWork.Users.SetAvatar(userid, path);
                ViewBag.CurrentUser.Avatar = path;
            }

            return View("Index");
        }

        #endregion

        #region Change Password

        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string newPasswordConfirm)
        {
            InitViewBag();

            int userId = Convert.ToInt32(Session["CurrentUserId"]);
            _unitOfWork.Users.ChangePassword(userId, oldPassword, newPassword, newPasswordConfirm);
            return View("Index");
        }

        #endregion

        #region Change Status

        [HttpPost]
        public ActionResult ChangeStatus(string status)
        {
            InitViewBag();

            int userId = Convert.ToInt32(Session["CurrentUserId"]);
            _unitOfWork.Users.ChangeStatus(userId, status);

            return View("Index");
        }


        #endregion

        #region Change City

        [HttpPost]
        public ActionResult ChangeCity(string city)
        {
            InitViewBag();

            int userId = Convert.ToInt32(Session["CurrentUserId"]);

            _unitOfWork.Users.ChangeCity(userId, city);

            return View("Index");
        }

        #endregion

        #region Change Student Info

        [HttpPost]
        public ActionResult ChangeStudentInfo(string university, string faculty, int course)
        {
            InitViewBag();

            int userId = Convert.ToInt32(Session["CurrentUserId"]);
            _unitOfWork.Users.ChangeStudentInfo(userId, university, faculty, course);

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
            _unitOfWork.Users.RecoverPassword(username, email, path);
        }

        public ActionResult PassRecover(string str)
        {
            string username = _unitOfWork.Users.ContainsRecoveryWithSuchHash(str);
            
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
                string username =.HttpContext.Session["RecoveryUsername"] as string;
                _unitOfWork.Users.SetNewPassword(username, password);
                return 1;
            }
            return 0;
        }

        #endregion
    }
}

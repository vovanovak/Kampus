using Kampus.Models;
using Microsoft.AspNetCore.Mvc;
using Kampus.Api.Constants;
using Microsoft.AspNetCore.Http;
using Kampus.Api.Extensions;
using Kampus.Api.Services;
using Kampus.Application.Services;
using Kampus.Application.Services.Users;

namespace Kampus.Controllers
{
    public class SettingsController : Controller
    {
        //
        // GET: /Settings/
        private readonly IUserService _userService;
        private readonly IUserProfileRecoveryService _userProfileRecoveryService;
        private readonly ICityService _cityService;
        private readonly IUniversityService _universityService;
        private readonly IFileService _fileService;

        public SettingsController(
            IUserService userService,
            IUserProfileRecoveryService userProfileRecoveryService,
            ICityService cityService,
            IUniversityService universityService,
            IFileService fileService)
        {
            _userService = userService;
            _userProfileRecoveryService = userProfileRecoveryService;
            _cityService = cityService;
            _universityService = universityService;
            _fileService = fileService;
        }

        public ActionResult Index()
        {
            InitViewBag();

            return View();
        }

        public void InitViewBag()
        {
            ViewBag.CurrentUser = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            ViewBag.Cities = _cityService.GetCities();
            ViewBag.Universities = _universityService.GetUniversities();
            ViewBag.Faculties = ViewBag.Universities.ElementAt(0).Faculties;
        }

        #region Change Avatar
        
        [HttpPost]
        public ActionResult ChangeAvatar(IFormFile file)
        {
            InitViewBag();

            int userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);

            if (file != null)
            {
                string path = _fileService.SaveImage(HttpContext, file);
                _userService.SetAvatar(userId, path);
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

            var userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            _userService.ChangePassword(userId, oldPassword, newPassword, newPasswordConfirm);
            return View("Index");
        }

        #endregion

        #region Change Status

        [HttpPost]
        public ActionResult ChangeStatus(string status)
        {
            InitViewBag();

            int userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            _userService.ChangeStatus(userId, status);

            return View("Index");
        }


        #endregion

        #region Change City

        [HttpPost]
        public ActionResult ChangeCity(string city)
        {
            InitViewBag();

            int userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);

            _userService.ChangeCity(userId, city);

            return View("Index");
        }

        #endregion

        #region Change Student Info

        [HttpPost]
        public ActionResult ChangeStudentInfo(string university, string faculty, int course)
        {
            InitViewBag();

            int userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            _userService.ChangeStudentInfo(userId, university, faculty, course);

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
            string path = Url.Action("PassRecover", "Settings", null, Request.Scheme) + "?str=";
            _userProfileRecoveryService.RecoverPassword(username, email, path);
        }

        public ActionResult PassRecover(string str)
        {
            string username = _userProfileRecoveryService.ContainsRecoveryWithSuchHash(str);
            
            if (username != null)
            {
                HttpContext.Session.Add("RecoveryUsername", username);
                return View();
            }
            return View("Error");
        }

        [HttpPost]
        public int TotalRecover(string password, string password1)
        {
            if (password == password1)
            {
                var username = HttpContext.Session.Get<string>("RecoveryUsername");
                _userProfileRecoveryService.SetNewPassword(username, password);
                return 1;
            }
            return 0;
        }

        #endregion
    }
}

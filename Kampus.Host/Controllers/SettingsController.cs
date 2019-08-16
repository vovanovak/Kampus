using System.Threading.Tasks;
using Kampus.Application.Services;
using Kampus.Application.Services.Users;
using Kampus.Host.Constants;
using Kampus.Host.Extensions;
using Kampus.Host.Services;
using Kampus.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kampus.Host.Controllers
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

        public async Task<IActionResult> Index()
        {
            await InitViewBag();

            return View();
        }

        public async Task InitViewBag()
        {
            ViewBag.CurrentUser = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            ViewBag.Cities = await _cityService.GetCities();
            ViewBag.Universities = await _universityService.GetUniversities();
            ViewBag.Faculties = ViewBag.Universities.ElementAt(0).Faculties;
        }

        #region Change Avatar

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(IFormFile file)
        {
            await InitViewBag();

            var userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);

            if (file != null)
            {
                var path = await _fileService.SaveImage(HttpContext, file);
                _userService.SetAvatar(userId, path);
                ViewBag.CurrentUser.Avatar = path;
            }

            return View("Index");
        }

        #endregion

        #region Change Password

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string newPasswordConfirm)
        {
            await InitViewBag();

            var userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            _userService.ChangePassword(userId, oldPassword, newPassword, newPasswordConfirm);
            return View("Index");
        }

        #endregion

        #region Change Status

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string status)
        {
            await InitViewBag();

            int userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            _userService.ChangeStatus(userId, status);

            return View("Index");
        }


        #endregion

        #region Change City

        [HttpPost]
        public async Task<IActionResult> ChangeCity(string city)
        {
            await InitViewBag();

            int userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);

            _userService.ChangeCity(userId, city);

            return View("Index");
        }

        #endregion

        #region Change Student Info

        [HttpPost]
        public async Task<IActionResult> ChangeStudentInfo(string university, string faculty, int course)
        {
            await InitViewBag();

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

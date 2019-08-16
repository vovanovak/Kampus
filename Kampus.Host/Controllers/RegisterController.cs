using System.Collections.Generic;
using System.Threading.Tasks;
using Kampus.Application.Services;
using Kampus.Application.Services.Users;
using Kampus.Host.Extensions;
using Kampus.Host.Services;
using Kampus.Models;
using Kampus.Persistence.Entities.UniversityRelated;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kampus.Host.Controllers
{
    public class RegisterController : Controller
    {
        //
        // GET: /Register/

        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly ICityService _cityService;
        private readonly IUniversityService _universityService;

        private static UserModel _userModel;

        public RegisterController(
            IUserService userService,
            IFileService fileService,
            ICityService cityService,
            IUniversityService universityService)
        {
            _userService = userService;
            _fileService = fileService;
            _cityService = cityService;
            _universityService = universityService;
        }

        public ActionResult Index()
        {
            return View("Step1", _userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step1(UserModel u)
        {
            _userModel = new UserModel();

            if (ModelState.IsValidField("FullName") &&
                ModelState.IsValidField("Email") &&
                ModelState.IsValidField("Password"))
            {
                _userModel.Email = u.Email;
                _userModel.Password = u.Password;
                _userModel.FullName = u.FullName;

                await FillViewBag();

                return RedirectToAction("Step2");
            }

            return View("Step1", _userModel);
        }

        public async Task<IActionResult> Step2()
        {
            await FillViewBag();

            return View(_userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step2(UserModel u)
        {
            if (ModelState.IsValidField("DateOfBirth") &&
                ModelState.IsValidField("UniversityName") &&
                ModelState.IsValidField("Faculty") &&
                ModelState.IsValidField("City") &&
                ModelState.IsValidField("UniversityCourse"))
            {
                _userModel.DateOfBirth = u.DateOfBirth;
                _userModel.UniversityName = u.UniversityName;
                _userModel.UniversityFaculty = u.UniversityFaculty;
                _userModel.UniversityCourse = u.UniversityCourse;
                _userModel.City = u.City;

                return RedirectToAction("Step3");
            }

            await FillViewBag();

            return View("Step2", _userModel);
        }

        public async Task<IActionResult> Step3()
        {
            return View(_userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step3(IFormFile file, string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                if (await _userService.ContainsUserWithSuchUsername(username))
                    return View("Step3", _userModel);

                _userModel.Username = username;

                if (file != null)
                {
                    _userModel.Avatar = await _fileService.SaveImage(HttpContext, file);
                }

                return RedirectToAction("Step4");
            }

            return View("Step3", _userModel);
        }

        public ActionResult Step4()
        {
            return View(_userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step4(UserModel u)
        {
            _userService.RegisterUser(_userModel);
            return RedirectToAction("SignIn", "Main");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserModel u)
        {
            if (ModelState.IsValid)
            {
                _userService.RegisterUser(u);
                return View("Success");
            }

            await FillViewBag();

            return View(u);
        }

        [HttpGet]
        public async Task<string> ContainsUserWithSuchUsername(string username)
        {
            return (await _userService.ContainsUserWithSuchUsername(username)) ? "contains" : "no";
        }

        [HttpGet]
        public bool ContainsUserWithSuchEmail(string email)
        {
            return _userService.ContainsUserWithSuchEmail(email);
        }

        [HttpGet]
        public async Task<IReadOnlyList<Faculty>> GetUniversityFaculties(string name)
        {
            return await _universityService.GetUniversityFaculties(name);
        }

        public async Task FillViewBag()
        {
            ViewBag.Cities = await _cityService.GetCities();
            ViewBag.Universities = await _universityService.GetUniversities();
            ViewBag.Faculties = (ViewBag.Universities[0] as UniversityModel).Faculties;
        }
    }
}

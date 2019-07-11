using System.Collections.Generic;
using System.Linq;
using Kampus.Models;
using Microsoft.AspNetCore.Mvc;
using Kampus.Application.Services.Users;
using Kampus.Api.Services;
using Kampus.Application.Services;
using Microsoft.AspNetCore.Http;
using Kampus.Api.Extensions;

namespace Kampus.Controllers
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
        public ActionResult Step1(UserModel u)
        {
            _userModel = new UserModel();

            if (ModelState.IsValidField("FullName") &&
                ModelState.IsValidField("Email") &&
                ModelState.IsValidField("Password"))
            {
                _userModel.Email = u.Email;
                _userModel.Password = u.Password;
                _userModel.FullName = u.FullName;
                
                FillViewBag();

                return RedirectToAction("Step2");
            }
            else
            {
                return View("Step1", _userModel);
            }
        }

        public ActionResult Step2()
        {
            FillViewBag();

            return View(_userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step2(UserModel u)
        {
            if (ModelState.IsValidField("DateOfBirth") &&
                ModelState.IsValidField("UniversityName") &&
                ModelState.IsValidField("UniversityFaculty") &&
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
            else
            {
                FillViewBag();

                return View("Step2", _userModel);
            }
        }

        public ActionResult Step3()
        {
            return View(_userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step3(IFormFile file, string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                if (_userService.ContainsUserWithSuchUsername(username))
                    return View("Step3", _userModel);

                _userModel.Username = username;

                if (file != null)
                {
                    _userModel.Avatar = _fileService.SaveImage(HttpContext, file);
                }

                return RedirectToAction("Step4");
            }
            else
            {
                return View("Step3", _userModel);
            }
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
            return RedirectToAction("Index", "SignIn");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserModel u)
        {
            if (ModelState.IsValid)
            {
                _userService.RegisterUser(u);
                return View("Success");
            }
            else
            {
                FillViewBag();

                return View(u);
            }
        }

        [HttpGet]
        public string ContainsUserWithSuchUsername(string username)
        {
            return _userService.ContainsUserWithSuchUsername(username) ? "contains" : "no";
        }

        [HttpGet]
        public bool ContainsUserWithSuchEmail(string email)
        {
            return _userService.ContainsUserWithSuchEmail(email);
        }

        [HttpGet]
        public string GetUniversityFaculties(string name)
        {
            return _universityService.GetUniversityFaculties(name);
        }

        public void FillViewBag()
        {
            List<CityModel> cities = _cityService.GetCities();
            ViewBag.Cities = cities;

            List<UniversityModel> universities = _universityService.GetUniversities();
            ViewBag.Universities = universities;

            List<UniversityFacultyModel> faculties = universities.ElementAt(0).Faculties;
            ViewBag.Faculties = faculties;
        }
    }
}

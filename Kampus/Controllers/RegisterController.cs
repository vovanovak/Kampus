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
    public class RegisterController : Controller
    {
        //
        // GET: /Register/

        private readonly IUserRepository _dbUser = 
            Kampus.Container.Autofac.Container.Resolve<IUserRepository>();
        private readonly IUniversityRepository _dbUniversity =
            Kampus.Container.Autofac.Container.Resolve<IUniversityRepository>();
        private readonly ICityRepository _dbCity = 
            Kampus.Container.Autofac.Container.Resolve<ICityRepository>();

        private static UserModel _userModel;

        public ActionResult Index()
        {
            FillViewBag();

            return View("Step1", _userModel);
        }

        public ActionResult Step1()
        {
            FillViewBag();

            return View(_userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step1(UserModel u)
        {
           
            _userModel = new UserModel();
           

            FillViewBag();

            if (ModelState.IsValidField("FullName") &&
                ModelState.IsValidField("Email") &&
                ModelState.IsValidField("Password"))
            {
                _userModel.Email = u.Email;
                _userModel.Password = u.Password;
                _userModel.FullName = u.FullName;

                return View("Step2", _userModel);
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
            FillViewBag();

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

                return View("Step3", _userModel);
            }
            else
            {
                return View("Step2", _userModel);
            }
        }

        public ActionResult Step3()
        {
            FillViewBag();

            return View(_userModel);
        }

        public string GetEncodedHash(string path)
        {
               const string salt = "adhasdhasdhas";
               MD5 md5 = new MD5CryptoServiceProvider();
               byte [] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(path + salt));
               string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
               return base64digest.Substring(0, base64digest.Length-2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step3(HttpPostedFileBase file, string username)
        {
            FillViewBag();

            if (!string.IsNullOrEmpty(username))
            {
                if (_dbUser.ContainsUserWithSuchUsername(username))
                    return View("Step3", _userModel);

                _userModel.Username = username;

                if (file != null)
                {
                    
                    string filename = GetEncodedHash(DateTime.Now.Ticks.ToString());
                    filename = filename.Replace("\\", "a");
                    filename = filename.Replace("/", "a");
                    filename = filename.Replace("+", "b");

                    string ext = file.FileName.Substring(file.FileName.LastIndexOf("."));

                    file.SaveAs(HttpContext.Server.MapPath("~/Images/")
                                                          + filename + ext);
                    _userModel.Avatar = "/Images/" + filename + ext;
                }

                return View("Step4", _userModel);
            }
            else
            {
                return View("Step3", _userModel);
            }
        }

        public ActionResult Step4()
        {
            FillViewBag();

            return View(_userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Step4(UserModel u)
        {
           
            _dbUser.RegisterUser(_userModel);
            return RedirectToAction("Index", "SignIn");
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserModel u)
        {
            if (ModelState.IsValid)
            {
                _dbUser.RegisterUser(u);
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
            return (_dbUser.ContainsUserWithSuchUsername(username)) ? "contains" : "no";
        }

        [HttpGet]
        public string GetUniversityFaculties(string name)
        {
            string res = _dbUniversity.GetUniversityFaculties(name);
            return res;
        }

        public void FillViewBag()
        {
            List<CityModel> cities = _dbCity.GetCities();
            ViewBag.Cities = cities;

            List<UniversityModel> universities = _dbUniversity.GetUniversities();
            ViewBag.Universities = universities;

            List<UniversityFacultyModel> faculties = universities.ElementAt(0).Faculties;
            ViewBag.Faculties = faculties;
        }
    }
}

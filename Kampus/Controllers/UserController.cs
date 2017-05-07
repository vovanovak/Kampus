using Kampus.DAL;
using Kampus.DAL.Abstract;
using Kampus.DAL.Enums;
using Kampus.DAL.Exceptions;
using Kampus.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kampus.Controllers
{
    public class UserController : Controller
    {
        private IUnitOfWork _unitOfWork;

        private SearchTaskModel _searchTask = new SearchTaskModel();
        private UserSearchModel _searchUser = new UserSearchModel();

        public UserController()
        {
            _unitOfWork = UnitOfWorkResolver.UnitOfWork;
        }

        public ActionResult Index()
        {
            var user = Session["CurrentUser"] as UserModel;
            Session.Add("UserProfile", user);

            user.Posts = _unitOfWork.WallPosts.GetAllPosts(user.Id);
            user.Friends = _unitOfWork.Users.GetUserFriends(user.Id);
            user.Subscribers = _unitOfWork.Users.GetUserSubscribers(user.Id);

            ViewBag.CurrentUser = user;
            ViewBag.UserProfile = user;

            return View();
        }

        public ActionResult Id(int id)
        {
            var user = Session["CurrentUser"] as UserModel;
            ViewBag.CurrentUser = user;

            try
            {
                UserModel userProfile = _unitOfWork.Users.GetEntityById(id);
                userProfile.Posts = _unitOfWork.WallPosts.GetAllPosts(userProfile.Id);
                userProfile.Friends = _unitOfWork.Users.GetUserFriends(userProfile.Id);
                userProfile.Subscribers = _unitOfWork.Users.GetUserSubscribers(userProfile.Id);

                ViewBag.UserProfile = userProfile;
                
                Session.Add("UserProfile", userProfile);
            }
            catch (Exception e)
            {
                return View("Error");
            }

            return View("Index");
        }

        #region Global User Search

        public ActionResult All()
        {
            UserModel user = Session["CurrentUser"] as UserModel;
            List<UserModel> users = _unitOfWork.Users.GetAll().ToList();

            ViewBag.CurrentUser = user;
            ViewBag.Users = users;

            List<CityModel> cities = _unitOfWork.Cities.GetCities();
            ViewBag.Cities = cities;

            List<UniversityModel> universities = _unitOfWork.Universities.GetUniversities();
            ViewBag.Universities = universities;

            List<UniversityFacultyModel> faculties = universities.ElementAt(0).Faculties;
            ViewBag.Faculties = faculties;

            ViewBag.UserSearch = _searchUser;

            return View();
        }

        public ActionResult SearchUsers(string request, string university, string faculty, string city,
            int? course, int? minage, int? maxage, int? minrating, int? maxrating)
        {
            List<UserModel> users = _unitOfWork.Users.SearchUsers(request, university, faculty, city,
                course, minage, maxage, minrating, maxrating);

            _searchUser = _unitOfWork.Users.UpdateUserSearch(request, university, faculty, city,
               course, minage, maxage, minrating, maxrating);

            ViewBag.UserSearch = _searchUser;

            UserModel user = Session["CurrentUser"] as UserModel;

            ViewBag.CurrentUser = user;
            ViewBag.Users = users;

            List<CityModel> cities = _unitOfWork.Cities.GetCities();
            ViewBag.Cities = cities;

            List<UniversityModel> universities = _unitOfWork.Universities.GetUniversities();
            ViewBag.Universities = universities;

            List<UniversityFacultyModel> faculties = universities.ElementAt(0).Faculties;
            ViewBag.Faculties = faculties;

            return View("All");
        }

        #endregion

        #region Friends

        public ActionResult Friends()
        {
            var receiver = Session["UserProfile"] as UserModel;
            var sender = Session["CurrentUser"] as UserModel;

            receiver.Friends = _unitOfWork.Users.GetUserFriends(receiver.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = receiver;

            return View("Friends");
        }

        [HttpPost]
        public ActionResult RemoveFriend(int friendid)
        {
            UserModel user = Session["CurrentUser"] as UserModel;

            _unitOfWork.Users.RemoveFriend(user.Id, friendid);
            user.Friends = _unitOfWork.Users.GetUserFriends(user.Id);

            ViewBag.CurrentUser = user;

            return View("Friends");
        }

        public ActionResult FriendsById(int id)
        {
            var receiver = _unitOfWork.Users.GetEntityById(id);
            var sender = Session["CurrentUser"] as UserModel;

            receiver.Friends = _unitOfWork.Users.GetUserFriends(receiver.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = receiver;

            return View("Friends");
        }

        #endregion

        #region Subscribers

        public ActionResult Subscribers()
        {
            var receiver = Session["UserProfile"] as UserModel;
            var sender = Session["CurrentUser"] as UserModel;

            receiver.Subscribers = _unitOfWork.Users.GetUserSubscribers(receiver.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = receiver;

            return View("Subscribers");
        }

        [HttpPost]
        public string AddSubscriber()
        {
            var receiver = Session["UserProfile"] as UserModel;
            var sender = Session["CurrentUser"] as UserModel;
            
            bool res = false;

            try
            {
                _unitOfWork.Users.AddSubscriber(receiver, sender);
                res = true;
            }
            catch (SameUserException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SubscribeOnFriendException e)
            {
                Console.WriteLine(e.Message);
            }
            
            return JsonConvert.ToString(res);
            
        }

        [HttpPost]
        public ActionResult AddAsAFriend(int userId)
        {
            var currentUser = Session["CurrentUser"] as UserModel;

            try
            {
                _unitOfWork.Users.AddFriend(currentUser.Id, userId);
            }
            catch (SameUserException e)
            {
                Console.WriteLine(e.Message);
            }

            currentUser.Subscribers = _unitOfWork.Users.GetUserSubscribers(currentUser.Id);

            ViewBag.CurrentUser = currentUser;
            return View("Subscribers");
        }

        #endregion

        #region Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Response.Cookies.Clear();
            return RedirectToAction("Index", "SignIn");
        }
        #endregion
    }
}

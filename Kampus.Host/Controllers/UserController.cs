﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kampus.Application.Exceptions;
using Kampus.Application.Services;
using Kampus.Application.Services.Users;
using Kampus.Host.Constants;
using Kampus.Host.Extensions;
using Kampus.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kampus.Host.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserConnectionsService _userConnectionsService;
        private readonly IUserSearchService _userSearchService;
        private readonly IUserProfileRecoveryService _userProfileRecoveryService;
        private readonly IWallPostService _wallPostService;
        private readonly ICityService _cityService;
        private readonly IUniversityService _universityService;

        private SearchTaskModel _searchTask = new SearchTaskModel();
        private UserSearchModel _searchUser = new UserSearchModel();

        public UserController(
            IUserService userService,
            IUserConnectionsService userConnectionsService,
            IUserSearchService userSearchService,
            IUserProfileRecoveryService userProfileRecoveryService,
            IWallPostService wallPostService,
            ICityService cityService,
            IUniversityService universityService)
        {
            _userService = userService;
            _userConnectionsService = userConnectionsService;
            _userSearchService = userSearchService;
            _userProfileRecoveryService = userProfileRecoveryService;
            _wallPostService = wallPostService;
            _cityService = cityService;
            _universityService = universityService;
        }

        public async Task<IActionResult> Index()
        {
            var user = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            HttpContext.Session.Add(SessionKeyConstants.UserProfile, user);

            user.Posts = await _wallPostService.GetAllPosts(user.Id);
            user.Friends = _userConnectionsService.GetUserFriends(user.Id);
            user.Subscribers = _userConnectionsService.GetUserSubscribers(user.Id);

            ViewBag.CurrentUser = user;
            ViewBag.UserProfile = user;

            return View();
        }

        public async Task<IActionResult> Id(int id)
        {
            var user = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            ViewBag.CurrentUser = user;

            try
            {
                UserModel userProfile = _userService.GetById(id);
                userProfile.Posts = await _wallPostService.GetAllPosts(userProfile.Id);
                userProfile.Friends = _userConnectionsService.GetUserFriends(userProfile.Id);
                userProfile.Subscribers = _userConnectionsService.GetUserSubscribers(userProfile.Id);

                ViewBag.UserProfile = userProfile;

                HttpContext.Session.Add(SessionKeyConstants.UserProfile, userProfile);
            }
            catch (Exception)
            {
                return View("Error");
            }

            return View("Index");
        }

        #region Global User Search

        public async Task<IActionResult> All()
        {
            var user = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            var users = _userService.GetAll().ToList();

            ViewBag.CurrentUser = user;
            ViewBag.Users = users;
            ViewBag.Cities = await _cityService.GetCities();
            ViewBag.Universities = await _universityService.GetUniversities();
            ViewBag.Faculties = ViewBag.Universities.ElementAt(0).Faculties;

            ViewBag.UserSearch = _searchUser;

            return View();
        }

        public async Task<IActionResult> SearchUsers(string request, string university, string faculty, string city,
            int? course, int? minage, int? maxage, int? minrating, int? maxrating)
        {
            List<UserModel> users = _userSearchService.SearchUsers(request, university, faculty, city,
                course, minage, maxage, minrating, maxrating);

            _searchUser = _userSearchService.UpdateUserSearch(request, university, faculty, city,
               course, minage, maxage, minrating, maxrating);

            ViewBag.UserSearch = _searchUser;

            UserModel user = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            ViewBag.CurrentUser = user;
            ViewBag.Users = users;
            ViewBag.Cities = await _cityService.GetCities();
            ViewBag.Universities = await _universityService.GetUniversities();
            ViewBag.Faculties = ViewBag.Universities.ElementAt(0).Faculties;

            return View("All");
        }

        #endregion

        #region Friends

        public ActionResult Friends()
        {
            var receiver = HttpContext.Session.Get<UserModel>(SessionKeyConstants.UserProfile);
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            receiver.Friends = _userConnectionsService.GetUserFriends(receiver.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = receiver;

            return View("Friends");
        }

        [HttpPost]
        public ActionResult RemoveFriend(int friendid)
        {
            UserModel user = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            _userConnectionsService.RemoveFriend(user.Id, friendid);
            user.Friends = _userConnectionsService.GetUserFriends(user.Id);

            ViewBag.CurrentUser = user;

            return View("Friends");
        }

        public ActionResult FriendsById(int id)
        {
            var receiver = _userService.GetById(id);
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            receiver.Friends = _userConnectionsService.GetUserFriends(receiver.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = receiver;

            return View("Friends");
        }

        #endregion

        #region Subscribers

        public ActionResult Subscribers()
        {
            var receiver = HttpContext.Session.Get<UserModel>(SessionKeyConstants.UserProfile);
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            receiver.Subscribers = _userConnectionsService.GetUserSubscribers(receiver.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = receiver;

            return View("Subscribers");
        }

        [HttpPost]
        public string AddSubscriber()
        {
            var receiver = HttpContext.Session.Get<UserModel>(SessionKeyConstants.UserProfile);
            var sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            bool res = false;

            try
            {
                _userConnectionsService.AddSubscriber(receiver, sender);
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
            var currentUser = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            try
            {
                _userConnectionsService.AddFriend(currentUser.Id, userId);
            }
            catch (SameUserException e)
            {
                Console.WriteLine(e.Message);
            }

            currentUser.Subscribers = _userConnectionsService.GetUserSubscribers(currentUser.Id);

            ViewBag.CurrentUser = currentUser;
            return View("Subscribers");
        }

        #endregion

        #region Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn", "Main");
        }
        #endregion
    }
}

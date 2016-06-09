using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Kampus.DAL;
using Kampus.DAL.Abstract;
using Kampus.DAL.Concrete;
using Kampus.Models;
using System.IO;
using Newtonsoft.Json;

namespace Kampus.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/

        private readonly IUserRepository _dbUser =
             Kampus.Container.Autofac.Container.Resolve<IUserRepository>();
        private readonly IWallPostRepository _dbWallPost =
            Kampus.Container.Autofac.Container.Resolve<IWallPostRepository>();
        private readonly ITaskRepository _dbTask =
            Kampus.Container.Autofac.Container.Resolve<ITaskRepository>();
        private readonly IMessageRepository _dbMessage =
            Kampus.Container.Autofac.Container.Resolve<IMessageRepository>();
        private readonly INotificationRepository _dbNotification =
            Kampus.Container.Autofac.Container.Resolve<INotificationRepository>();
        private readonly IUniversityRepository _dbUniversity =
           Kampus.Container.Autofac.Container.Resolve<IUniversityRepository>();
        private readonly ICityRepository _dbCity =
            Kampus.Container.Autofac.Container.Resolve<ICityRepository>();

        private SearchTaskModel _searchTask = new SearchTaskModel();
        private UserSearchModel _searchUser = new UserSearchModel();
        
        private static List<FileModel> _attachmentsWallpost;
        private static List<FileModel> _attachmentsTask;


        public ActionResult Id(int id)
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            ViewBag.CurrentUser = user;

            try
            {
                UserModel userPr = (UserModel)_dbUser.GetEntityById(id);
                userPr.Posts = _dbWallPost.GetAllPosts(userPr.Id);
                userPr.Friends = _dbUser.GetUserFriends(userPr.Id);
                userPr.Subscribers = _dbUser.GetUserSubscribers(userPr.Id);
                ViewBag.UserProfile = userPr;


                Session.Add("UserProfileId", userPr.Id);
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
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));
            List<UserModel> users = _dbUser.GetAll().ToList();

            ViewBag.CurrentUser = user;
            ViewBag.Users = users;

            List<CityModel> cities = _dbCity.GetCities();
            ViewBag.Cities = cities;

            List<UniversityModel> universities = _dbUniversity.GetUniversities();
            ViewBag.Universities = universities;

            List<UniversityFacultyModel> faculties = universities.ElementAt(0).Faculties;
            ViewBag.Faculties = faculties;

            ViewBag.UserSearch = _searchUser;

            return View();
        }

        public ActionResult SearchUsers(string request, string university, string faculty, string city,
            int? course, int? minage, int? maxage, int? minrating, int? maxrating)
        {
            List<UserModel> users = _dbUser.SearchUsers(request, university, faculty, city,
                course, minage, maxage, minrating, maxrating);

            _searchUser = _dbUser.UpdateUserSearch(request, university, faculty, city,
               course, minage, maxage, minrating, maxrating);

            ViewBag.UserSearch = _searchUser;

            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            ViewBag.CurrentUser = user;
            ViewBag.Users = users;

            List<CityModel> cities = _dbCity.GetCities();
            ViewBag.Cities = cities;

            List<UniversityModel> universities = _dbUniversity.GetUniversities();
            ViewBag.Universities = universities;

            List<UniversityFacultyModel> faculties = universities.ElementAt(0).Faculties;
            ViewBag.Faculties = faculties;

            return View("All");
        }

        #endregion

        #region WallPost

        [HttpPost]
        public void UploadFileWallpost()
        {
            if (_attachmentsWallpost == null)
                _attachmentsWallpost = new List<FileModel>();

            _attachmentsWallpost.AddRange(UploadFileToServer().ToArray());
        }

        public List<FileModel> UploadFileToServer()
        {
            List<FileModel> files = new List<FileModel>();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                var fileName = Convert.ToString(DateTime.Now.Ticks) + file.FileName.Substring(file.FileName.LastIndexOf("."));

                var path = Path.Combine(Server.MapPath("~/Files/"), fileName);
                file.SaveAs(path);

                files.Add(new FileModel() { RealFileName = fileName, FileName = file.FileName, IsImage = (fileName.Substring(fileName.LastIndexOf('.')) == ".jpg") });
            }

            return files;
        }

        [HttpPost]
        public string WriteWallPost(string text)
        {
            int userid = Convert.ToInt32(Session["UserProfileId"]);
            int senderid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            UserModel sender = (UserModel)_dbUser.GetEntityById(senderid);

            
            WallPostModel last = _dbWallPost.WriteWallPost(user.Id, sender.Id, text, _attachmentsWallpost);

            last.Id = _dbWallPost.GetAll().Last().Id;

            if (_attachmentsWallpost == null)
                _attachmentsWallpost = new List<FileModel>();
            _attachmentsWallpost.Clear();

            return JsonConvert.SerializeObject(new
            {
                Id = last.Id,
                Content = last.Content,
                Likes = last.Likes.Count,
                Sender = last.Sender.Username,
                Username = last.User.Username,
                Comments = last.Comments,
                Files = last.Attachments.Where(f => !f.IsImage).ToList(),
                Images = last.Attachments.Where(f => f.IsImage).ToList()
            });
        }


        [HttpPost]
        public int LikeWallPost(int postid)
        {
            int userid = Convert.ToInt32(Session["UserProfileId"]);
            int senderid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            UserModel sender = (UserModel)_dbUser.GetEntityById(senderid);

            int res = _dbWallPost.LikeWallPost(sender.Id, postid);
            
            return res;
        }

        [HttpPost]
        public string WritePostComment(string text, int postid)
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["UserProfileId"]));
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            
            _dbWallPost.WritePostComment(sender.Id, postid, text);
            user.Posts = _dbWallPost.GetAllPosts(user.Id);

            user.Posts = _dbWallPost.GetAllPosts(sender.Id);
            WallPostCommentModel lst = user.Posts.First(p => p.Id == postid).Comments.Last();
            return JsonConvert.SerializeObject(lst);
        }

        #endregion

        #region Task

        [HttpPost]
        public void UploadFileTask()
        {
            if (_attachmentsTask == null)
                _attachmentsTask = new List<FileModel>();

            _attachmentsTask.AddRange(UploadFileToServer().ToArray());
        }

        public void InitTaskViewBag(int userid)
        {
            List<TaskCategoryModel> categories = _dbTask.GetTaskCategories();
            ViewBag.TaskCategories = categories;

            List<TaskSubcatModel> subcats = _dbTask.GetSubcategories(categories.ElementAt(0).Id);
            ViewBag.TaskSubcategories = subcats;

            ViewBag.SubscribedTasks = _dbTask.GetUserSubscribedTasks(userid);
            ViewBag.ExecutiveTasks = _dbTask.GetUserExecutiveTasks(userid);
        }

        public ActionResult Tasks()
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["UserProfileId"]));
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            user.Tasks = _dbTask.GetUserTasks(user.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = user;

            InitTaskViewBag(user.Id);

           
            ViewBag.SearchTask = _searchTask;

            return View("Tasks");
        }



        public ActionResult ViewTasks(int userid)
        {
            if (Session["UserProfileId"] == null)
                Session.Add("UserProfileId", userid);

            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            user.Tasks = _dbTask.GetUserTasks(user.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = user;

            InitTaskViewBag(user.Id);

            return View("Tasks");
        }

        [HttpPost]
        public string WriteTaskComment(int taskid, string text)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            _dbTask.WriteTaskComment(sender, taskid, text);

            ViewBag.CurrentUser = sender;
            TaskCommentModel model = _dbTask.GetUserTasks(userid).First(t => t.Id == taskid).Comments.Last();

            return JsonConvert.SerializeObject(model);
        }

        [HttpPost]
        public int LikeTask(int taskid)
        {
            //UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["UserProfileId"]));
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            int res = _dbTask.LikeTask(sender, taskid);

            //user.Tasks = _dbTask.GetUserTasks(user.Id);

            ViewBag.CurrentUser = sender;
            //ViewBag.UserProfile = user;

            //InitTaskViewBag(user.Id);

            return res;
        }

        [HttpPost]
        public string SubscribeOnTask(int taskid, int? taskprice)
        {
            //UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["UserProfileId"]));
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            try { 
            
            
                _dbTask.SubscribeOnTheTask(sender.Id, taskid, taskprice);


                return JsonConvert.SerializeObject(new { 
                    Id = sender.Id,
                    Username = sender.Username,
                    Price = taskprice
                });
             
            }
            catch(SameUserException e)
            {
                return JsonConvert.SerializeObject(new {
                    Id = -1
                });
            }
        }

        [HttpGet]
        public ActionResult SearchTasks(string request, int? category, int? subcategory,
           int? minprice, int? maxprice, int? executive, int? subscribed)
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["UserProfileId"]));
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            if (executive != null)
            {
                user.Tasks = _dbTask.GetUserExecutiveTasks(user.Id);
            }
            else
            {
                if (subscribed != null)
                {
                    user.Tasks = _dbTask.GetUserSubscribedTasks(user.Id);
                }
                else
                {
                    user.Tasks = _dbTask.SearchTasks(request, user.Id, category, subcategory, minprice, maxprice);
                }
            }


            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = user;

            _searchTask = _dbTask.UpdateSearchModel(request, null, category, subcategory, minprice, maxprice);

            ViewBag.SearchTask = _searchTask;

            InitTaskViewBag(user.Id);

            return View("Tasks");
        }

        #endregion

        #region Messages

        [HttpPost]
        public void WriteMessage(string username)
        {
            Response.Redirect("/Home/Conversation?username=" + username);
        }

        #endregion

        #region Groups

        //public ActionResult Groups()
        //{
        //    int userid = Convert.ToInt32(Session["CurrentUserId"]);
        //    UserModel user = (UserModel)_dbUser.GetEntityById(userid);
        //    int profileid = Convert.ToInt32(Session["UserProfileId"]);

        //    UserModel profile = (UserModel) _dbUser.GetEntityById(profileid);
        //    profile.Groups = _dbGroup.GetUserGroups(profileid);

        //    ViewBag.CurrentUser = user;
        //    ViewBag.UserProfile = profile;

        //    return View("Groups");
        //}

        #endregion

        #region Friends

        public ActionResult Friends()
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["UserProfileId"]));
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            user.Friends = _dbUser.GetUserFriends(user.Id);


            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = user;

            return View("Friends");
        }

        #endregion

        #region Subscribers

        public ActionResult Subscribers()
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["UserProfileId"]));
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            user.Subscribers = _dbUser.GetUserSubscribers(user.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = user;

            return View("Subscribers");
        }

        [HttpPost]
        public string AddSubscriber()
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["UserProfileId"]));
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));
           
           

            user.Posts = _dbWallPost.GetAllPosts(user.Id);

            bool res = false;

            try
            {

                _dbUser.AddSubscriber(user, sender);
                res = true;
            }
            catch (SameUserException e)
            {

            }
            catch (SubscribeOnFriendException e)
            {

            }

            user.Posts = _dbWallPost.GetAllPosts(user.Id);
            user.Friends = _dbUser.GetUserFriends(user.Id);
            user.Subscribers = _dbUser.GetUserSubscribers(user.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = user;

          
            return JsonConvert.ToString(res);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Newtonsoft.Json;
using Kampus.DAL.Abstract;
using Kampus.Models;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Kampus.DAL;
namespace Kampus.Controllers
{
    public class HomeController : Controller
    {
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

        private SearchTaskModel _searchTask = new SearchTaskModel();

        private static List<FileModel> _attachmentsMessages;
        private static List<FileModel> _attachmentsWallpost;
        private static List<FileModel> _attachmentsTask;

        public ActionResult Index()
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            user.Posts = _dbWallPost.GetAllPosts(userid);
            user.Friends = _dbUser.GetUserFriends(userid).Take(6).ToList();
            user.Subscribers = _dbUser.GetUserSubscribers(userid).Take(6).ToList();

            if (user.Friends == null)
                user.Friends = new List<UserShortModel>();

            if (user.Subscribers == null)
                user.Subscribers = new List<UserShortModel>();

            ViewBag.CurrentUser = user;

            return View();
        }

        #region WallPost

        public ActionResult Post(int id)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);

            WallPostModel wallPost = (WallPostModel)_dbWallPost.GetEntityById(id);

            ViewBag.CurrentUser = user;
            ViewBag.CurrentPost = wallPost;

            return View();
        }

        [HttpPost]
        public string WriteWallPost(string text)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            
            WallPostModel last = _dbWallPost.WriteWallPost(userid, userid, text, _attachmentsWallpost);
            last.Id = _dbWallPost.GetAllPosts(userid).Last().Id;

            
            if (_attachmentsWallpost != null)
            {
                _attachmentsWallpost.Clear();
            }
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
        public string WritePostComment(int postid, string text)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            _dbWallPost.WritePostComment(userid, postid, text);

            user.Posts = _dbWallPost.GetAllPosts(userid);
            WallPostCommentModel lst = user.Posts.First(p => p.Id == postid).Comments.Last();
            return JsonConvert.SerializeObject(lst);
        }

        [HttpPost]
        public int LikeWallPost(int postid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            user.Posts = _dbWallPost.GetAllPosts(userid);

            int res = _dbWallPost.LikeWallPost(userid, postid);
            
            return res;
        }

        [HttpGet]
        public string GetNewWallPostComments(int postid, int? postcommentid)
        {
            WallPostCommentModel[] comments = _dbWallPost.GetNewWallPostComments(postid, postcommentid).ToArray();
            string res = JsonConvert.SerializeObject(comments);
            return res;
        }

        [HttpPost]
        public void UploadFileWallpost()
        {
            if (_attachmentsWallpost == null)
                _attachmentsWallpost = new List<FileModel>();

            _attachmentsWallpost.AddRange(UploadFileToServer().ToArray());
        }

        #endregion

        #region Tasks

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
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            user.Tasks = _dbTask.GetUserTasks(user.Id);

            if (user == null)
                return View("Error");

            ViewBag.CurrentUser = user;

            ViewBag.SearchTask = _searchTask;



            InitTaskViewBag(userid);

            return View();
        }

        public ActionResult SearchTasks(string request, int? category, int? subcategory,
            int? minprice, int? maxprice, int? solved, int? executive, int? subscribed)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);

            if (solved != null)
            {
                user.Tasks = _dbTask.GetUserSolvedTasks(userid);
            }
            else 
                if (executive != null)
                {
                    user.Tasks = _dbTask.GetUserExecutiveTasks(userid);
                }
                else 
                    if (subscribed != null)
                    {
                        user.Tasks = _dbTask.GetUserSubscribedTasks(userid);
                    }
                    else
                    {
                        user.Tasks = _dbTask.SearchTasks(request, userid, category, subcategory, minprice, maxprice);
                    }
            
            

            if (user == null)
                return View("Error");

            ViewBag.CurrentUser = user;

            _searchTask = _dbTask.UpdateSearchModel(request, null, category, subcategory, minprice, maxprice);

            ViewBag.SearchTask = _searchTask;

            InitTaskViewBag(userid);

            return View("Tasks");
        }

        

        [HttpPost]
        public ActionResult HideTask(int taskid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            user.Tasks = _dbTask.GetUserTasks(user.Id);

            _dbTask.CheckTaskAsHidden(taskid);

            ViewBag.CurrentUser = user;

            if (user.Tasks == null)
                user.Tasks = _dbTask.GetUserTasks(userid);

            InitTaskViewBag(userid);

            return View("Tasks");
        }


        [HttpGet]
        public string GetSubcategories(string name)
        {
            List<TaskCategoryModel> categories = _dbTask.GetTaskCategories();
            TaskCategoryModel category = categories.First(c => c.Name == name);
            List<TaskSubcatModel> subcats = _dbTask.GetSubcategories(category.Id);
            string res = JsonConvert.SerializeObject(subcats.Select(f => new { f.Id, f.Name }).ToArray());
            return res;
        }

        [HttpPost]
        public string CreateTask(string header, string content, int price, int category, int subcategory)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);

            TaskModel newTask = _dbTask.CreateTask(userid, header, content, price, category, subcategory, _attachmentsTask);

            if (_attachmentsTask != null)
                _attachmentsTask.Clear();

            return JsonConvert.SerializeObject(new
            {
                Id = newTask.Id,
                Header = newTask.Header,
                Content = newTask.Content,
                Price = newTask.Price,
                CategoryName = newTask.CategoryName,
                SubcategoryName = newTask.SubcategoryName,
                Images = newTask.Attachments.Where(f => f.IsImage).ToList(),
                Files = newTask.Attachments.Where(f => !f.IsImage).ToList(),
                Subscribers = newTask.Subscribers,
                Executive = newTask.Executive,
                Comments = newTask.Comments,
                Creator = newTask.Creator,
                Solved = newTask.Solved,
                LikesCount = newTask.Likes.Count
            });
        }

        [HttpPost]
        public void RemoveTask(int taskid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);

            _dbTask.RemoveTask(taskid);
        }

        [HttpPost]
        public int LikeTask(int taskid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            
            int res = _dbTask.LikeTask(user, taskid);

            user.Tasks = _dbTask.GetUserTasks(userid);

            ViewBag.CurrentUser = user;

            InitTaskViewBag(userid);
           
            return res;
        }

        [HttpPost]
        public string WriteTaskComment(int taskid, string text)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            
            _dbTask.WriteTaskComment(user, taskid, text);

            user.Tasks = _dbTask.GetUserTasks(user.Id);

            TaskCommentModel model = user.Tasks.First(t => t.Id == taskid).Comments.Last();

            return JsonConvert.SerializeObject(model);
        }

        [HttpPost]
        public string CheckTaskAsSolved(int taskid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            user.Tasks = _dbTask.GetUserTasks(user.Id);

            int? executiveid = _dbTask.GetTaskExecutiveId(taskid);

            _dbTask.CheckTaskAsSolved(taskid);

            if (user.Tasks == null)
                user.Tasks = _dbTask.GetUserTasks(userid);

            ViewBag.CurrentUser = user;

            InitTaskViewBag(userid);
            
            return JsonConvert.SerializeObject(new { Id = executiveid });
        }

        [HttpPost]
        public string CheckAsTaskMainExecutive(int taskid, string username)
        {
            int userid1 = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid1);
            
            username = username.Remove(0, 1);

            if (username.Contains(" "))
                username = username.Remove(username.IndexOf(" "));

            _dbTask.CheckAsTaskMainExecutive(taskid, username);

            return JsonConvert.SerializeObject(_dbUser.GetByUsername(username));
        }

        [HttpPost]
        public ActionResult RemoveTaskExecutive(int taskid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            user.Tasks = _dbTask.GetUserTasks(user.Id);

            _dbTask.RemoveTaskExecutive(taskid);

            if (user.Tasks == null)
                user.Tasks = _dbTask.GetUserTasks(userid);

            ViewBag.CurrentUser = user;

            ViewBag.SearchTask = _searchTask;

            InitTaskViewBag(userid);

            return View("Tasks");
        }

        [HttpGet]
        public string GetNewTaskComments(int taskid, int? taskcommentid)
        {
            TaskCommentModel[] comments = _dbTask.GetNewTaskComments(taskid, taskcommentid).ToArray();
            string res = JsonConvert.SerializeObject(comments);
            return res;
        }

        #endregion

        #region Subscribers

        public ActionResult Subscribers()
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            user.Subscribers = _dbUser.GetUserSubscribers(userid);
            
            ViewBag.CurrentUser = user;

            return View();
        }

        [HttpPost]
        public ActionResult AddAsAFriend(int userid)
        {
            int userid1 = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid1);

            try
            {
                _dbUser.AddFriend(user.Id, userid);
            }
            catch (SameUserException e)
            {

            }

            user.Subscribers = _dbUser.GetUserSubscribers(userid1);

            ViewBag.CurrentUser = user;

            return View("Subscribers");
        }

        #endregion

        #region Friends

        public ActionResult Friends()
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
            user.Friends = _dbUser.GetUserFriends(userid);

            ViewBag.CurrentUser = user;

            return View();
        }

        [HttpPost]
        public ActionResult RemoveFriend(int friendid)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(userid);
           
            _dbUser.RemoveFriend(user.Id, friendid);
            user.Friends = _dbUser.GetUserFriends(userid);

            ViewBag.CurrentUser = user;

            return View("Friends");
        }

        #endregion

        #region Groups

        //public ActionResult Groups()
        //{
        //    int userid = Convert.ToInt32(Session["CurrentUserId"]);
        //    UserModel user = (UserModel)_dbUser.GetEntityById(userid);
        //    user.Groups = _dbGroup.GetUserGroups(userid);

        //    ViewBag.CurrentUser = user;

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateGroup(GroupModel model)
        //{
        //    int userid = Convert.ToInt32(Session["CurrentUserId"]);
        //    UserModel user = (UserModel)_dbUser.GetEntityById(userid);
        //    user.Groups = _dbGroup.GetUserGroups(userid);

        //    ViewBag.CurrentUser = user;

        //    model.Creator = new UserShortModel() { Id = userid, Username = user.Username, Avatar = user.Avatar};

        //    if (ModelState.IsValid)
        //        _dbGroup.CreateGroup(model);

        //    return View("Groups");
        //}

        #endregion

        #region Messages

        public ActionResult Messages()
        {
            int senderid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(senderid);

            List<MessageModel> messages = _dbMessage.GetUserMessages(user.Id);

            List<UserShortModel> messangers = _dbMessage.GetUserMessangers(senderid);

            if (messangers.Any(u => u.Id == senderid))
                messangers.Remove(messangers.First(u => u.Id == senderid));

            var firstOrDefault = messangers.FirstOrDefault();
            if (firstOrDefault != null)
            {
                UserModel receiver = _dbUser.GetByUsername(firstOrDefault.Username);

                ViewBag.Messangers = messangers;
                ViewBag.SecondUser = receiver.Username;

                List<MessageModel> toViewBag =
                    messangers.Select(u => messages.OrderBy(m => m.CreationDate).Last()).ToList();

                ViewBag.FirstMessages = toViewBag;

                ViewBag.CurrentUser = user;
                ViewBag.UserProfile = receiver;

                ViewBag.Messages = _dbMessage.GetMessages(senderid, receiver.Id).OrderBy(m => m.CreationDate.Ticks);
            }
            else
            {
                ViewBag.CurrentUser = user;
                ViewBag.UserProfile = new UserShortModel() { Id = -1 };
            }

            return View("Conversation");
        }

        public ActionResult Conversation(string username)
        {
            int senderid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(senderid);

            UserModel receiver = _dbUser.GetByUsername(username);

            List<MessageModel> messages = _dbMessage.GetUserMessages(user.Id);

            List<UserShortModel> messangers = _dbMessage.GetUserMessangers(senderid);

            if (messangers.Any(u => u.Id == senderid))
                messangers.RemoveAll(u => u.Id == senderid);



            if (messangers.All(u => u.Username != username))
            {
                messangers.Add(new UserShortModel()
                {
                    Id = receiver.Id,
                    Username = receiver.Username,
                    Avatar = receiver.Avatar
                });
            }

            ViewBag.Messangers = messangers;
            ViewBag.SecondUser = username;

            List<MessageModel> toViewBag = messangers.Select(u => messages.OrderBy(m => m.CreationDate).LastOrDefault()).ToList();
            
            ViewBag.FirstMessages = toViewBag;

            ViewBag.CurrentUser = user;
            ViewBag.UserProfile = receiver;

            ViewBag.Messages = _dbMessage.GetMessages(senderid, receiver.Id);

            return View("Conversation");
        }

        public string GetEncodedHash(string path)
        {
            const string salt = "adhasdhasdhas";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(path + salt));
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            return base64digest.Substring(0, base64digest.Length - 2);
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
        public void UploadFileMessage()
        {
            if (_attachmentsMessages == null)
                _attachmentsMessages = new List<FileModel>();

            _attachmentsMessages.AddRange(UploadFileToServer().ToArray());
        }

        [HttpPost]
        public string WriteMessage(int userid, string text)
        {
            int senderid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = (UserModel)_dbUser.GetEntityById(senderid);
            UserModel receiver = (UserModel)_dbUser.GetEntityById(userid);

            _dbMessage.WriteMessage(senderid, userid, text, _attachmentsMessages);

            List<MessageModel> messages = _dbMessage.GetUserMessages(user.Id);

            List<MessageModel> models = _dbMessage.GetMessages(senderid, userid);
            ViewBag.Messages = models;

            if (_attachmentsMessages != null)
                _attachmentsMessages.Clear();

            MessageModel last = models.OrderBy(m => m.CreationDate.Ticks).Last();

            return JsonConvert.SerializeObject(new {
                Id = last.Id,
                Username = last.Sender.Username,
                Receiver = last.Receiver.Username,
                Content = last.Content,
                Avatar = last.Sender.Avatar,
                Files = last.Attachments.Where(a => !a.IsImage),
                Images = last.Attachments.Where(a => a.IsImage),
                IsSender = true
            });
        }

        [HttpGet]
        public string GetNewMessages(int senderid, int receiverid, int lastmsgid)
        {
            List<MessageModel> messages = _dbMessage.GetNewMessages(senderid, receiverid, lastmsgid);

            return JsonConvert.SerializeObject(messages.Select(m => new
            {
                Id = m.Id,
                Username = m.Sender.Username,
                Content = m.Content,
                Avatar = m.Sender.Avatar,
                Files = m.Attachments.Where(a => !a.IsImage),
                Images = m.Attachments.Where(a => a.IsImage),
                IsSender = false
            }).ToArray());
        }

        #endregion

        #region Notifications

        [HttpGet]
        public string GetNewNotifications()
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            List<NotificationModel> notifications = _dbNotification.GetNewNotifications(userid);
            string res = JsonConvert.SerializeObject(notifications);
            return res;
        }

        [HttpPost]
        public void ViewNotifications()
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            _dbNotification.ViewUnseenNotifications(userid);
        }

        #endregion
    }
}

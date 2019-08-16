using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kampus.Application.Exceptions;
using Kampus.Application.Services;
using Kampus.Application.Services.Users;
using Kampus.Host.Constants;
using Kampus.Host.Extensions;
using Kampus.Host.Services;
using Kampus.Models;
using Kampus.Persistence.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kampus.Host.Controllers
{
    public class TaskController : Controller
    {
        private static List<FileModel> _attachmentsTask;
        private SearchTaskModel _searchTask = new SearchTaskModel();

        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public TaskController(ITaskService taskService, IUserService userService, IFileService fileService)
        {
            _taskService = taskService;
            _userService = userService;
            _fileService = fileService;
        }

        public void InitTaskViewBag()
        {
            List<TaskCategoryModel> categories = _taskService.GetTaskCategories();
            ViewBag.TaskCategories = categories;

            List<TaskSubcategoryModel> subcats = _taskService.GetSubcategories(categories.ElementAt(0).Id);
            ViewBag.TaskSubcategories = subcats;

            ViewBag.SearchTask = _searchTask;
        }

        public void InitHomeTaskViewBag(int userId)
        {
            List<TaskCategoryModel> categories = _taskService.GetTaskCategories();
            ViewBag.TaskCategories = categories;

            List<TaskSubcategoryModel> subcats = _taskService.GetSubcategories(categories.ElementAt(0).Id);
            ViewBag.TaskSubcategories = subcats;

            ViewBag.SubscribedTasks = _taskService.GetUserSubscribedTasks(userId);
            ViewBag.ExecutiveTasks = _taskService.GetUserExecutiveTasks(userId);

            ViewBag.SearchTask = _searchTask;
        }

        public ActionResult Index()
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            ViewBag.Tasks = _taskService.GetAll().OrderByDescending(t => t.Id).ToList();
            ViewBag.CurrentUser = sender;
            ViewBag.SearchTask = _searchTask;

            InitTaskViewBag();

            return View();
        }

        public ActionResult Id(int id)
        {
            UserModel user = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            TaskModel task = _taskService.GetById(id);

            ViewBag.CurrentUser = user;
            ViewBag.CurrentTask = task;

            return View("Task");
        }

        public ActionResult SearchTasks(string request, int? category, int? subcategory,
            int? minprice, int? maxprice, int? solved, int? executive, int? subscribed)
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            if (solved != null)
            {
                ViewBag.Tasks = _taskService.GetUserSolvedTasks(sender.Id);
            }
            else if (executive != null)
            {
                ViewBag.Tasks = _taskService.GetUserExecutiveTasks(sender.Id);
            }
            else if (subscribed != null)
            {
                ViewBag.Tasks = _taskService.GetUserSubscribedTasks(sender.Id);
            }
            else
            {
                ViewBag.Tasks = _taskService.SearchTasks(request, sender.Id, category, subcategory, minprice, maxprice);
            }

            ViewBag.CurrentUser = sender;

            _searchTask = _taskService.UpdateSearchModel(request, null, category, subcategory, minprice, maxprice);

            ViewBag.SearchTask = _searchTask;

            InitHomeTaskViewBag(sender.Id);

            return View("Index");
        }

        public ActionResult Categories()
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            ViewBag.CurrentUser = sender;

            List<TaskCategoryModel> categories = _taskService.GetTaskCategories().DistinctBy(c => c.Name).ToList();
            ViewBag.TaskCategories = categories;

            List<TaskSubcategoryModel> subcats = new List<TaskSubcategoryModel>();

            foreach (var category in categories)
            {
                subcats.AddRange(_taskService.GetSubcategories(category.Id).DistinctBy(c => c.Name).ToList());
            }

            ViewBag.TaskSubcategories = subcats;

            return View();
        }

        public ActionResult ExecutionReview(int taskId)
        {
            TaskModel task = _taskService.GetById(taskId);
            ViewBag.Task = task;

            ExecutionReviewModel model = new ExecutionReviewModel();

            model.TaskId = task.Id;
            model.Executor = task.Executive;

            return View();
        }

        [HttpPost]
        public ActionResult CreateReview(ExecutionReviewModel model)
        {
            if (model.Rating == null)
                return RedirectToAction("ViewHomeTasks", "Task");

            _taskService.AddExecutionReview(model);

            return RedirectToAction("ViewHomeTasks", "Task");
        }

        [HttpPost]
        public async Task UploadFileTask()
        {
            if (_attachmentsTask == null)
                _attachmentsTask = new List<FileModel>();

            _attachmentsTask.AddRange(await _fileService.UploadFilesToServer(HttpContext));
        }

        public ActionResult ViewTasks(int userId)
        {
            if (!HttpContext.Session.TryGetValue(SessionKeyConstants.UserProfileId, out _))
                HttpContext.Session.Add(SessionKeyConstants.UserProfileId, userId);

            UserModel profile = _userService.GetById(userId);
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            profile.Tasks = _taskService.GetUserTasks(profile.Id)
                .Where(t => t.Solved != true).ToList();

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = profile;


            return View("Index");
        }

        public ActionResult ViewHomeTasks()
        {
            UserModel user = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            UserModel sender = user;

            user.Tasks = _taskService.GetUserTasks(user.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = user;

            InitHomeTaskViewBag(user.Id);

            return View("HomeTasks");
        }

        [HttpPost]
        public string WriteTaskComment(int taskId, string text)
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            var comment = _taskService.WriteTaskComment(sender, taskId, text);
            return JsonConvert.SerializeObject(comment);
        }

        [HttpPost]
        public LikeResult LikeTask(int taskId)
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);
            ViewBag.CurrentUser = sender;
            return _taskService.LikeTask(sender, taskId);
        }

        [HttpPost]
        public string SubscribeOnTask(int taskId, int? taskPrice)
        {
            UserModel sender = HttpContext.Session.Get<UserModel>(SessionKeyConstants.CurrentUser);

            try
            {
                _taskService.SubscribeOnTheTask(sender.Id, taskId, taskPrice);

                return JsonConvert.SerializeObject(new
                {
                    sender.Id,
                    sender.Username,
                    Price = taskPrice
                });

            }
            catch (SameUserException e)
            {
                Console.WriteLine(e);
                return JsonConvert.SerializeObject(new
                {
                    Id = -1
                });
            }
        }

        [HttpPost]
        public ActionResult HideTask(int taskId)
        {
            int userid = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            UserModel user = _userService.GetById(userid);
            user.Tasks = _taskService.GetUserTasks(user.Id);

            _taskService.CheckTaskAsHidden(taskId);

            ViewBag.CurrentUser = user;

            if (user.Tasks == null)
                user.Tasks = _taskService.GetUserTasks(userid);

            InitHomeTaskViewBag(userid);

            return View("HomeTasks");
        }


        [HttpGet]
        public string GetSubcategories(string name)
        {
            List<TaskCategoryModel> categories = _taskService.GetTaskCategories();
            TaskCategoryModel category = categories.First(c => c.Name == name);
            List<TaskSubcategoryModel> subcats = _taskService.GetSubcategories(category.Id);
            return JsonConvert.SerializeObject(subcats.Select(f => new { f.Id, f.Name }).ToArray());
        }

        [HttpPost]
        public string CreateTask(string header, string content, int price, int category, int subcategory)
        {
            int userid = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            UserModel user = _userService.GetById(userid);

            TaskModel newTask = _taskService.CreateTask(userid, header, content, price, category, subcategory, _attachmentsTask);

            if (_attachmentsTask != null)
                _attachmentsTask.Clear();

            return JsonConvert.SerializeObject(new
            {
                newTask.Id,
                newTask.Header,
                newTask.Content,
                newTask.Price,
                newTask.CategoryName,
                newTask.SubcategoryName,
                Images = newTask.Attachments.Where(f => f.IsImage()).ToList(),
                Files = newTask.Attachments.Where(f => !f.IsImage()).ToList(),
                newTask.Subscribers,
                newTask.Executive,
                newTask.Comments,
                newTask.Creator,
                newTask.Solved,
                LikesCount = newTask.Likes.Count
            });
        }

        [HttpPost]
        public void RemoveTask(int taskId)
        {
            int userid = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            UserModel user = _userService.GetById(userid);

            _taskService.RemoveTask(taskId);
        }

        [HttpPost]
        public string CheckTaskAsSolved(int taskId)
        {
            int? executiveId = _taskService.GetTaskExecutiveId(taskId);
            _taskService.CheckTaskAsSolved(taskId);
            return JsonConvert.SerializeObject(new { Id = executiveId });
        }

        [HttpPost]
        public string CheckAsTaskMainExecutive(int taskId, string username)
        {
            int userid1 = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            UserModel user = _userService.GetById(userid1);

            username = username.Remove(0, 1);

            if (username.Contains(" "))
                username = username.Remove(username.IndexOf(" "));

            _taskService.CheckAsTaskMainExecutive(taskId, username);

            return JsonConvert.SerializeObject(_userService.GetByUsername(username));
        }

        [HttpPost]
        public string RemoveTaskExecutive(int taskId)
        {
            try
            {
                var userid = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);

                UserModel user = _userService.GetById(userid);
                user.Tasks = _taskService.GetUserTasks(user.Id);
                _taskService.RemoveTaskExecutive(taskId);
                return JsonConvert.SerializeObject(true);
            }
            catch
            {
                return JsonConvert.SerializeObject(false);
            }
        }

        [HttpGet]
        public string GetNewTaskComments(int taskId, int? taskCommentId)
        {
            var comments = _taskService.GetNewTaskComments(taskId, taskCommentId);
            return JsonConvert.SerializeObject(comments);
        }
    }
}


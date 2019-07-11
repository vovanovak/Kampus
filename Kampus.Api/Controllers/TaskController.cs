using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Kampus.DAL.Abstract;
using Kampus.DAL.Concrete;
using Kampus.Models;
using Microsoft.Ajax.Utilities;
using Kampus.DAL;
using Newtonsoft.Json;
using Kampus.DAL.Enums;
using Kampus.Api.Controllers;
using Kampus.Application.Exceptions;

namespace Kampus.Controllers
{
    public class TaskController : Controller
    {
        private static List<FileModel> _attachmentsTask;
        private IUnitOfWork _unitOfWork;
        private SearchTaskModel _searchTask = new SearchTaskModel();
        public TaskController()
        {
            _unitOfWork = UnitOfWorkResolver.UnitOfWork;
        }

        public void InitTaskViewBag()
        {
            List<TaskCategoryModel> categories = _unitOfWork.Tasks.GetTaskCategories();
            ViewBag.TaskCategories = categories;

            List<TaskSubcatModel> subcats = _unitOfWork.Tasks.GetSubcategories(categories.ElementAt(0).Id);
            ViewBag.TaskSubcategories = subcats;

            ViewBag.SearchTask = _searchTask;
        }

        public void InitHomeTaskViewBag(int userId)
        {
            List<TaskCategoryModel> categories = _unitOfWork.Tasks.GetTaskCategories();
            ViewBag.TaskCategories = categories;

            List<TaskSubcatModel> subcats = _unitOfWork.Tasks.GetSubcategories(categories.ElementAt(0).Id);
            ViewBag.TaskSubcategories = subcats;

            ViewBag.SubscribedTasks = _unitOfWork.Tasks.GetUserSubscribedTasks(userId);
            ViewBag.ExecutiveTasks = _unitOfWork.Tasks.GetUserExecutiveTasks(userId);

            ViewBag.SearchTask = _searchTask;
        }

        public ActionResult Index()
        {
            UserModel sender = Session[SessionKeyConstants.CurrentUser] as UserModel;

            ViewBag.Tasks = _unitOfWork.Tasks.GetAll().OrderByDescending(t => t.Id).ToList();
            ViewBag.CurrentUser = sender;
            ViewBag.SearchTask = _searchTask;

            InitTaskViewBag();

            return View();
        }

        public ActionResult Id(int id)
        {
            UserModel user = Session[SessionKeyConstants.CurrentUser] as UserModel;
            TaskModel task = _unitOfWork.Tasks.GetEntityById(id);

            ViewBag.CurrentUser = user;
            ViewBag.CurrentTask = task;
            
            return View("Task");
        }

        public ActionResult SearchTasks(string request, int? category, int? subcategory,
            int? minprice, int? maxprice, int? solved, int? executive, int? subscribed)
        {
            UserModel sender = Session[SessionKeyConstants.CurrentUser] as UserModel;

            if (solved != null)
            {
                ViewBag.Tasks = _unitOfWork.Tasks.GetUserSolvedTasks(sender.Id);
            }
            else if (executive != null)
            {
                ViewBag.Tasks = _unitOfWork.Tasks.GetUserExecutiveTasks(sender.Id);
            }
            else if (subscribed != null)
            {
                ViewBag.Tasks = _unitOfWork.Tasks.GetUserSubscribedTasks(sender.Id);
            }
            else
            {
                ViewBag.Tasks = _unitOfWork.Tasks.SearchTasks(request, sender.Id, category, subcategory, minprice, maxprice);
            }

            ViewBag.CurrentUser = sender;

            _searchTask = _unitOfWork.Tasks.UpdateSearchModel(request, null, category, subcategory, minprice, maxprice);

            ViewBag.SearchTask = _searchTask;

            InitHomeTaskViewBag(sender.Id);

            return View("Index");
        }

        public ActionResult Categories()
        {
            UserModel sender = Session[SessionKeyConstants.CurrentUser] as UserModel;
            ViewBag.CurrentUser = sender;

            List<TaskCategoryModel> categories = _unitOfWork.Tasks.GetTaskCategories().DistinctBy(s => s.Name).ToList();
            ViewBag.TaskCategories = categories;

            List<TaskSubcatModel> subcats = new List<TaskSubcatModel>();

            foreach (var category in categories)
            {
                subcats.AddRange(_unitOfWork.Tasks.GetSubcategories(category.Id).DistinctBy(s => s.Name));
            }

            ViewBag.TaskSubcategories = subcats;

            return View();
        }

        public ActionResult ExecutionReview(int taskId)
        {
            TaskModel task = _unitOfWork.Tasks.GetEntityById(taskId);
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

            _unitOfWork.Tasks.AddExecutionReview(model);

            return RedirectToAction("ViewHomeTasks", "Task");
        }

        [HttpPost]
        public void UploadFileTask()
        {
            if (_attachmentsTask == null)
                _attachmentsTask = new List<FileModel>();

            _attachmentsTask.AddRange(FileController.UploadFilesToServer(HttpContext).ToArray());
        }

        public ActionResult ViewTasks(int userId)
        {
            if (Session["UserProfileId"] == null)
                Session.Add("UserProfileId", userId);

            UserModel profile = _unitOfWork.Users.GetEntityById(userId);
            UserModel sender = Session[SessionKeyConstants.CurrentUser] as UserModel;

            profile.Tasks = _unitOfWork.Tasks.GetUserTasks(profile.Id)
                .Where(t => !(t.Solved == true)).ToList();

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = profile;
            

            return View("Index");
        }

        public ActionResult ViewHomeTasks()
        {
            UserModel user = Session[SessionKeyConstants.CurrentUser] as UserModel;
            UserModel sender = user;

            user.Tasks = _unitOfWork.Tasks.GetUserTasks(user.Id);

            ViewBag.CurrentUser = sender;
            ViewBag.UserProfile = user;

            InitHomeTaskViewBag(user.Id);

            return View("HomeTasks");
        }

        [HttpPost]
        public string WriteTaskComment(int taskId, string text)
        {
            UserModel sender = Session[SessionKeyConstants.CurrentUser] as UserModel;
            var comment = _unitOfWork.Tasks.WriteTaskComment(sender, taskId, text);
            return JsonConvert.SerializeObject(comment);
        }

        [HttpPost]
        public LikeResult LikeTask(int taskId)
        {
            UserModel sender = Session[SessionKeyConstants.CurrentUser] as UserModel;
            ViewBag.CurrentUser = sender;
            return _unitOfWork.Tasks.LikeTask(sender, taskId);
        }

        [HttpPost]
        public string SubscribeOnTask(int taskId, int? taskPrice)
        {
            UserModel sender = Session[SessionKeyConstants.CurrentUser] as UserModel;

            try
            {
                _unitOfWork.Tasks.SubscribeOnTheTask(sender.Id, taskId, taskPrice);

                return JsonConvert.SerializeObject(new
                {
                    Id = sender.Id,
                    Username = sender.Username,
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
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = _unitOfWork.Users.GetEntityById(userid);
            user.Tasks = _unitOfWork.Tasks.GetUserTasks(user.Id);

            _unitOfWork.Tasks.CheckTaskAsHidden(taskId);

            ViewBag.CurrentUser = user;

            if (user.Tasks == null)
                user.Tasks = _unitOfWork.Tasks.GetUserTasks(userid);

            InitHomeTaskViewBag(userid);

            return View("HomeTasks");
        }


        [HttpGet]
        public string GetSubcategories(string name)
        {
            List<TaskCategoryModel> categories = _unitOfWork.Tasks.GetTaskCategories();
            TaskCategoryModel category = categories.First(c => c.Name == name);
            List<TaskSubcatModel> subcats = _unitOfWork.Tasks.GetSubcategories(category.Id);
            return JsonConvert.SerializeObject(subcats.Select(f => new { f.Id, f.Name }).ToArray());
        }

        [HttpPost]
        public string CreateTask(string header, string content, int price, int category, int subcategory)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = _unitOfWork.Users.GetEntityById(userid);

            TaskModel newTask = _unitOfWork.Tasks.CreateTask(userid, header, content, price, category, subcategory, _attachmentsTask);

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
                Images = newTask.Attachments.Where(f => f.IsImage()).ToList(),
                Files = newTask.Attachments.Where(f => !f.IsImage()).ToList(),
                Subscribers = newTask.Subscribers,
                Executive = newTask.Executive,
                Comments = newTask.Comments,
                Creator = newTask.Creator,
                Solved = newTask.Solved,
                LikesCount = newTask.Likes.Count
            });
        }

        [HttpPost]
        public void RemoveTask(int taskId)
        {
            int userid = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = _unitOfWork.Users.GetEntityById(userid);

            _unitOfWork.Tasks.RemoveTask(taskId);
        }

        [HttpPost]
        public string CheckTaskAsSolved(int taskId)
        {
            int? executiveId = _unitOfWork.Tasks.GetTaskExecutiveId(taskId);
            _unitOfWork.Tasks.CheckTaskAsSolved(taskId);
            return JsonConvert.SerializeObject(new { Id = executiveId });
        }

        [HttpPost]
        public string CheckAsTaskMainExecutive(int taskId, string username)
        {
            int userid1 = Convert.ToInt32(Session["CurrentUserId"]);
            UserModel user = _unitOfWork.Users.GetEntityById(userid1);

            username = username.Remove(0, 1);

            if (username.Contains(" "))
                username = username.Remove(username.IndexOf(" "));

            _unitOfWork.Tasks.CheckAsTaskMainExecutive(taskId, username);

            return JsonConvert.SerializeObject(_unitOfWork.Users.GetByUsername(username));
        }

        [HttpPost]
        public string RemoveTaskExecutive(int taskId)
        {
            try
            {
                int userid = Convert.ToInt32(Session["CurrentUserId"]);

                UserModel user = _unitOfWork.Users.GetEntityById(userid);
                user.Tasks = _unitOfWork.Tasks.GetUserTasks(user.Id);
                _unitOfWork.Tasks.RemoveTaskExecutive(taskId);
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
            TaskCommentModel[] comments = _unitOfWork.Tasks.GetNewTaskComments(taskId, taskCommentId).ToArray();
            return JsonConvert.SerializeObject(comments);
        }

    }
}


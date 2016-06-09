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

namespace Kampus.Controllers
{
    public class TasksController : Controller
    {
        //
        // GET: /Tasks/

        private readonly IUserRepository _dbUser =
              Kampus.Container.Autofac.Container.Resolve<IUserRepository>();

        private readonly ITaskRepository _dbTask =
           Kampus.Container.Autofac.Container.Resolve<ITaskRepository>();

        private SearchTaskModel _searchTask = new SearchTaskModel();
       
        public void InitTaskViewBag()
        {
            List<TaskCategoryModel> categories = _dbTask.GetTaskCategories();
            ViewBag.TaskCategories = categories;

            List<TaskSubcatModel> subcats = _dbTask.GetSubcategories(categories.ElementAt(0).Id);
            ViewBag.TaskSubcategories = subcats;


        }

        public ActionResult Id(int id)
        {
            UserModel user = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));
            TaskModel task = (TaskModel)_dbTask.GetEntityById(id);

            ViewBag.CurrentUser = user;
            ViewBag.CurrentTask = task;
            
            return View("Task");
        }

        public ActionResult Index()
        {
            UserModel sender = (UserModel) _dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            ViewBag.Tasks = _dbTask.GetAll().OrderByDescending(t => t.Id).ToList();

            ViewBag.CurrentUser = sender;

            ViewBag.SearchTask = _searchTask;

            InitTaskViewBag();

            return View();
        }

        [HttpGet]
        public ActionResult SearchTasks(string request, int? category, int? subcategory,
            int? minprice, int? maxprice, int? executive, int? subscribed)
        {

            UserModel sender = (UserModel) _dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));

            ViewBag.Tasks = _dbTask.SearchTasks(request, null, category, subcategory, minprice, maxprice);

            ViewBag.CurrentUser = sender;

            _searchTask = _dbTask.UpdateSearchModel(request, null, category, subcategory, minprice, maxprice);

            ViewBag.SearchTask = _searchTask;

            InitTaskViewBag();

            return View("Index");
        }


        public ActionResult Categories()
        {
            UserModel sender = (UserModel)_dbUser.GetEntityById(Convert.ToInt32(Session["CurrentUserId"]));
            ViewBag.CurrentUser = sender;

            List<TaskCategoryModel> categories = _dbTask.GetTaskCategories().DistinctBy(s => s.Name).ToList();
            ViewBag.TaskCategories = categories;

            List<TaskSubcatModel> subcats = new List<TaskSubcatModel>();

            foreach (var category in categories)
            {
                subcats.AddRange(_dbTask.GetSubcategories(category.Id).DistinctBy(s => s.Name));
            }

            ViewBag.TaskSubcategories = subcats;

            return View();
        }

        public ActionResult ExecutionReview(int taskid)
        {
            TaskModel task = (TaskModel)_dbTask.GetEntityById(taskid);
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
                return RedirectToAction("Tasks", "Home");

            _dbTask.AddExecutionReview(model);

            return RedirectToAction("Tasks", "Home");
        }
    }
}


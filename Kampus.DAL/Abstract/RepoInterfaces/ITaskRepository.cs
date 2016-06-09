using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Abstract
{
    public interface ITaskRepository: IRepository<TaskModel>
    {
        List<TaskModel> GetUserTasks(int userid);
        List<TaskModel> GetUserSolvedTasks(int userid);
        List<TaskCategoryModel> GetTaskCategories();
        List<TaskSubcatModel> GetSubcategories(int TaskCategoryId);
        List<TaskModel> GetUserSubscribedTasks(int userid);
        List<TaskModel> GetUserExecutiveTasks(int userid);
        void CheckTaskAsHidden(int taskid);
        void CreateTask(TaskModel model);
        TaskModel CreateTask(int userid, string header, string content, int price,
            int category, int subcategory, List<FileModel> attachmentes);
        List<TaskCommentModel> GetNewTaskComments(int taskid, int? taskcommentid);
        int LikeTask(UserModel userModel, int taskid);
        void WriteTaskComment(UserModel userModel, int taskid, string text);
        void CheckTaskAsSolved(int taskid);
        void CheckAsTaskMainExecutive(int taskid, string username);
        void RemoveTaskExecutive(int taskid);
        void SubscribeOnTheTask(int senderid, int taskid, int? taskprice);
        List<TaskModel> SearchTasks(string request, int? userid, int? category,
            int? subcategory, int? minprice, int? maxprice);
        void RemoveTask(int taskid);
        void AddExecutionReview(ExecutionReviewModel model);
        SearchTaskModel UpdateSearchModel(string request, int? userid, int? category, int? subcategory,
            int? minprice, int? maxprice);
        int? GetTaskExecutiveId(int taskid);
    }
}
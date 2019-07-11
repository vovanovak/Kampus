using Kampus.Models;
using Kampus.Persistence.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kampus.Application.Services
{
    public interface ITaskService
    {
        IReadOnlyList<TaskModel> GetUserTasks(int userId);
        IReadOnlyList<TaskModel> GetUserSolvedTasks(int userId);
        IReadOnlyList<TaskCategoryModel> GetTaskCategories();
        IReadOnlyList<TaskSubcatModel> GetSubcategories(int TaskCategoryId);
        IReadOnlyList<TaskModel> GetUserSubscribedTasks(int userId);
        IReadOnlyList<TaskModel> GetUserExecutiveTasks(int userId);
        void CheckTaskAsHidden(int taskId);
        TaskModel CreateTask(int userId, string header, string content, int price,
            int category, int subcategory, List<FileModel> attachmentes);
        List<TaskCommentModel> GetNewTaskComments(int taskId, int? taskCommentId);
        LikeResult LikeTask(UserModel userModel, int taskId);
        TaskCommentModel WriteTaskComment(UserModel userModel, int taskId, string text);
        void CheckTaskAsSolved(int taskId);
        void CheckAsTaskMainExecutive(int taskId, string username);
        void RemoveTaskExecutive(int taskId);
        void SubscribeOnTheTask(int senderId, int taskId, int? taskPrice);
        List<TaskModel> SearchTasks(string request, int? userId, int? category,
            int? subcategory, int? minPrice, int? maxPrice);
        void RemoveTask(int taskId);
        void AddExecutionReview(ExecutionReviewModel model);
        SearchTaskModel UpdateSearchModel(string request, int? userId, int? category, int? subcategory,
            int? minPrice, int? maxPrice);
        int? GetTaskExecutiveId(int taskId);
    }
}

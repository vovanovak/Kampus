using Kampus.Models;
using Kampus.Persistence.Enums;
using System.Collections.Generic;

namespace Kampus.Application.Services
{
    public interface ITaskService
    {
        List<TaskModel> GetAll();
        List<TaskModel> GetUserTasks(int userId);
        List<TaskModel> GetUserSolvedTasks(int userId);
        List<TaskCategoryModel> GetTaskCategories();
        List<TaskSubcategoryModel> GetSubcategories(int TaskCategoryId);
        List<TaskModel> GetUserSubscribedTasks(int userId);
        List<TaskModel> GetUserExecutiveTasks(int userId);
        TaskModel GetById(int taskId);
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

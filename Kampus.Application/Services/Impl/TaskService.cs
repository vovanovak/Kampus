using Kampus.Application.Exceptions;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.AttachmentsRelated;
using Kampus.Persistence.Entities.NotificationRelated;
using Kampus.Persistence.Entities.TaskRelated;
using Kampus.Persistence.Entities.UserRelated;
using Kampus.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kampus.Application.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly KampusContext _context;
        private readonly ITaskMapper _taskMapper;

        public TaskService(KampusContext context, ITaskMapper taskMapper)
        {
            _context = context;
            _taskMapper = taskMapper;
        }

        private IQueryable<TaskEntry> GetTasks()
        {
            return _context.Tasks
                .Include(t => t.TaskCategory)
                .Include(t => t.TaskSubcategory)
                .Include(t => t.Creator)
                .Include(t => t.Executive)
                .Include(t => t.TaskLikes)
                    .ThenInclude(l => l.Liker)
                .Include(t => t.TaskSubscribers)
                    .ThenInclude(s => s.Subscriber)
                .Include(t => t.TaskComments)
                    .ThenInclude(c => c.Creator)
                .Include(t => t.Attachments);
        }

        public List<TaskModel> GetUserTasks(int userId)
        {
            return GetTasks().Where(t => t.CreatorId == userId).Select(_taskMapper.Map).OrderByDescending(t => t.Id).ToList();
        }

        public List<TaskModel> GetUserSolvedTasks(int userId)
        {
            return GetTasks().Where(t => t.CreatorId == userId && t.Solved == true).Select(_taskMapper.Map).ToList();
        }

        public List<TaskCategoryModel> GetTaskCategories()
        {
            return _context.TaskCategories.Select(c => new TaskCategoryModel(c.TaskCategoryId, c.Name)).ToList();
        }

        public List<TaskSubcategoryModel> GetSubcategories(int taskCategoryId)
        {
            return _context.TaskSubcategories
                .Where(s => s.TaskCategoryId == taskCategoryId)
                .Select(c => new TaskSubcategoryModel(c.TaskSubcategoryId, c.Name, c.TaskCategoryId))
                .ToList();
        }

        public List<TaskModel> GetUserSubscribedTasks(int userId)
        {
            if (_context.Tasks.Any(task => task.TaskSubscribers.Any(t1 => t1.SubscriberId == userId)))
            {
                return GetTasks()
                    .Where(task => task.TaskSubscribers.Any(t1 => t1.SubscriberId == userId))
                    .Select(_taskMapper.Map)
                    .ToList();
            }

            return new List<TaskModel>();
        }

        public List<TaskModel> GetUserExecutiveTasks(int userId)
        {
            return GetTasks().Where(t => t.ExecutiveId == userId).Select(_taskMapper.Map).ToList();
        }

        public void CheckTaskAsHidden(int taskId)
        {
            var taskEntry = _context.Tasks.First(t => t.TaskId == taskId);

            taskEntry.Hide = !taskEntry.Hide;

            _context.SaveChanges();
        }

        public List<TaskCommentModel> GetNewTaskComments(int taskId, int? taskCommentId)
        {
            var taskEntry = _context.Tasks.First(p => p.TaskId == taskId);

            if (taskCommentId == null)
            {
                if (taskEntry.TaskComments.Any())
                {
                    return taskEntry.TaskComments.Select(c => new TaskCommentModel()
                    {
                        Id = c.TaskCommentId,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        TaskId = c.TaskId,
                        Creator = new UserShortModel(c.Creator.UserId, c.Creator.Username, c.Creator.Avatar)
                    }).ToList();
                }

                return new List<TaskCommentModel>();
            }

            var comment = taskEntry.TaskComments.First(c => c.TaskCommentId == taskCommentId);

            var comments = taskEntry.TaskComments.Where(p => comment.TaskCommentId < p.TaskCommentId).ToList();

            if (comments.Any())
            {
                return comments.Select(c => new TaskCommentModel()
                {
                    Id = c.TaskCommentId,
                    CreationTime = c.CreationTime,
                    Content = c.Content,
                    TaskId = c.TaskId,
                    Creator = new UserShortModel(c.Creator.UserId, c.Creator.Username, c.Creator.Avatar)
                }).ToList();
            }

            return new List<TaskCommentModel>();
        }

        public LikeResult LikeTask(UserModel userModel, int taskId)
        {
            var user = _context.Users.First(u => u.UserId == userModel.Id);

            if (_context.TaskLikes.Any(l => l.LikerId == user.UserId && l.TaskId == taskId))
            {
                var likes =
                    _context.TaskLikes.Where(l => l.LikerId == user.UserId && l.TaskId == taskId).ToList();
                _context.TaskLikes.RemoveRange(likes);
                _context.SaveChanges();

                return LikeResult.Unliked;
            }

            var like = new TaskLike { Task = _context.Tasks.First(t => t.TaskId == taskId) };

            like.TaskId = like.Task.TaskId;

            like.Liker = user;
            like.LikerId = user.UserId;

            _context.TaskLikes.Add(like);

            if (like.Task.Creator.UserId != user.UserId)
            {
                var notification = new Notification(DateTime.Now, NotificationType.TaskLike,
                    like.Task.Creator, user, "/Tasks/Id/" + taskId, "@" + user.Username + " оцінив ваше завдання");
                _context.Notifications.Add(notification);
            }

            _context.SaveChanges();

            return LikeResult.Liked;
        }

        public TaskCommentModel WriteTaskComment(UserModel userModel, int taskId, string text)
        {
            var user = _context.Users.First(u => u.UserId == userModel.Id);

            var comment = new TaskComment
            {
                Content = text,
                TaskId = taskId,
                TaskEntry = _context.Tasks.Single(p => p.TaskId == taskId),
                Creator = user,
                CreatorId = user.UserId,
                CreationTime = DateTime.Now
            };


            if (comment.TaskEntry.CreatorId != user.UserId)
            {
                var notification = new Notification(DateTime.Now, NotificationType.TaskComment,
                    user, comment.TaskEntry.Creator, "/Tasks/Id/" + taskId, "@" + user.Username + " коментував ваше завдання");

                _context.Notifications.Add(notification);
            }

            _context.TaskComments.Add(comment);
            _context.SaveChanges();

            return _context.TaskComments.Where(c => c.TaskId == taskId &&
                                                    c.CreationTime == comment.CreationTime)
                .Select(c => new TaskCommentModel()
                {
                    Id = c.TaskCommentId,
                    CreationTime = c.CreationTime,
                    Content = c.Content,
                    TaskId = c.TaskId,
                    Creator = new UserShortModel()
                    {
                        Id = c.Creator.UserId,
                        Username = c.Creator.Username,
                        Avatar = c.Creator.Avatar
                    }
                })
                .OrderByDescending(c => c.Id)
                .First();
        }

        public void CheckTaskAsSolved(int taskId)
        {
            var taskEntry = _context.Tasks.First(t => t.TaskId == taskId);

            if (taskEntry.Solved)
            {
                taskEntry.Solved = false;
                taskEntry.Executive = null;
                taskEntry.ExecutiveId = null;
            }
            else
            {
                taskEntry.Solved = true;

                if (taskEntry.Executive != null)
                {
                    if (taskEntry.Executive.Achievements == null)
                        taskEntry.Executive.Achievements = new List<Achievement>();

                    if (taskEntry.Executive.Achievements.All(t => t.TaskCategory.Name != taskEntry.TaskCategory.Name) &&
                        _context.Tasks.Count(t => t.Solved == true && t.Executive.UserId == taskEntry.Executive.UserId) >= 3)
                    {
                        taskEntry.Executive.Achievements.Add(new Achievement { TaskCategory = taskEntry.TaskCategory, User = taskEntry.Executive });

                        Notification notificationAchievement = Notification.From(DateTime.Now, NotificationType.Achievement,
                            taskEntry.Creator, taskEntry.Executive, "/Tasks/Id/" + taskEntry.TaskId, "Новий бейдж в категорії \"" + taskEntry.TaskCategory.Name + "\"");

                        _context.Notifications.Add(notificationAchievement);
                    }

                    Notification notificationSolved = Notification.From(DateTime.Now, NotificationType.Solved,
                        taskEntry.Creator, taskEntry.Executive, "/Tasks/Id/" + taskEntry.TaskId, "Завдання виконане!");

                    _context.Notifications.Add(notificationSolved);
                }
            }

            _context.SaveChanges();
        }

        public void CheckAsTaskMainExecutive(int taskId, string username)
        {
            var taskEntry = _context.Tasks.First(t => t.TaskId == taskId);
            var user = _context.Users.First(u => u.Username == username);

            taskEntry.TaskSubscribers.RemoveAll(t => t.Subscriber == user);

            if (taskEntry.ExecutiveId == user.UserId)
            {
                taskEntry.Executive = null;
                taskEntry.ExecutiveId = null;
            }
            else
            {
                taskEntry.Executive = user;
                taskEntry.ExecutiveId = user.UserId;
            }

            var notification = Notification.From(DateTime.Now, NotificationType.CheckedAsTaskExecutive,
                taskEntry.Creator, user, "/Tasks/Id/" + taskEntry.TaskId, "@" + taskEntry.Creator.Username + " поставив вас виконавцем завдання");

            _context.Notifications.Add(notification);

            _context.SaveChanges();
        }

        public void RemoveTaskExecutive(int taskId)
        {
            TaskEntry taskEntry = _context.Tasks.Single(t => t.TaskId == taskId);

            taskEntry.TaskSubscribers.RemoveAll(t => t.SubscriberId == taskEntry.ExecutiveId);
            taskEntry.ExecutiveId = null;
            taskEntry.Executive = null;

            _context.SaveChanges();
        }

        public void SubscribeOnTheTask(int senderId, int taskId, int? taskPrice)
        {
            var sender = _context.Users.Single(u => u.UserId == senderId);

            var taskEntry = _context.Tasks.First(t => t.TaskId == taskId);

            if (taskEntry.Executive != null)
            {
                if (taskEntry.Executive.UserId == sender.UserId)
                    throw new SameUserException();
            }

            if (sender.UserId == taskEntry.Creator.UserId)
                throw new SameUserException();

            if (taskEntry.TaskSubscribers == null)
                taskEntry.TaskSubscribers = new List<TaskSubscriber>();
            else
            {
                if (taskEntry.TaskSubscribers.Any(t => t.SubscriberId == senderId))
                    throw new SameUserException();
            }

            taskEntry.TaskSubscribers.RemoveAll(s => s.SubscriberId == senderId);
            taskEntry.TaskSubscribers.Add(new TaskSubscriber() { Subscriber = sender, Price = taskPrice.GetValueOrDefault() });

            Notification notification = Notification.From(DateTime.Now, NotificationType.TaskSubscribed,
                sender, taskEntry.Creator, "/Tasks/Id/" + taskEntry.TaskId, " підписався на виконання завдання");

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public List<TaskModel> SearchTasks(string request, int? userId, int? category,
            int? subcategory, int? minPrice, int? maxPrice)
        {
            var tasks = new List<TaskModel>();

            if (userId == null)
            {
                tasks = GetTasks().Select(_taskMapper.Map).ToList();
            }
            else
            {
                tasks = GetTasks().Where(t => t.CreatorId == userId).Select(_taskMapper.Map).ToList();
            }

            if (subcategory != -1)
            {
                tasks.RemoveAll(t => t.Subcategory != subcategory);
            }

            if (category != -1)
            {
                tasks.RemoveAll(t => t.Category != category);
            }

            if (!string.IsNullOrEmpty(request))
            {
                request = request.ToLower();

                tasks.RemoveAll(t => !t.Header.ToLower().Contains(request) && !t.Content.ToLower().Contains(request));
            }

            if (minPrice != null && maxPrice != null && minPrice.Value < maxPrice.Value)
            {
                tasks.RemoveAll(t => t.Price < minPrice || t.Price > maxPrice);
            }

            return tasks;
        }

        public void RemoveTask(int taskId)
        {
            _context.Tasks.Remove(_context.Tasks.First(t => t.TaskId == taskId));
            _context.SaveChanges();
        }

        public void AddExecutionReview(ExecutionReviewModel model)
        {
            var taskEntry = _context.Tasks.First(t => t.TaskId == model.TaskId);

            var review = new TaskExecutionReview();
            review.TaskId = model.TaskId.Value;
            review.Task = taskEntry;
            review.ExecutorId = taskEntry.ExecutiveId.Value;
            review.Executor = taskEntry.Executive;

            review.Rating = model.Rating;
            review.Review = model.Review;

            float? sum = _context.Reviews.Where(r => r.ExecutorId == review.ExecutorId).Sum(r => r.Rating);
            int length = _context.Reviews.Count(r => r.ExecutorId == review.ExecutorId) + 1;
            review.Executor.Rating = (float)(Math.Round((((sum ?? 0) + review.Rating.Value) / length), 2));

            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public SearchTaskModel UpdateSearchModel(string request, int? userId, int? category, int? subcategory,
            int? minPrice, int? maxPrice)
        {
            SearchTaskModel model = new SearchTaskModel
            {
                Request = request,
                CategoryId = category,
                SubcategoryId = subcategory,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return model;
        }


        public TaskModel CreateTask(int userId, string header, string content, int price,
            int category, int subcategory, List<FileModel> attachments)
        {
            var taskEntry = new TaskEntry
            {
                Header = header,
                Content = content,
                Price = price,
                Solved = false,
                CreatorId = userId,
                Creator = _context.Users.First(u => u.UserId == userId),
                TaskCategoryId = category,
                TaskCategory = _context.TaskCategories.First(c => c.TaskCategoryId == category),
                TaskSubcategoryId = subcategory,
                TaskSubcategory = _context.TaskSubcategories.First(s => s.TaskSubcategoryId == subcategory),
                TaskLikes = new List<TaskLike>(),
                TaskComments = new List<TaskComment>(),
                TaskSubscribers = new List<TaskSubscriber>()
            };


            if (attachments != null)
            {
                List<File> files = new List<File>();
                foreach (var link in attachments)
                {
                    File file = new File();
                    file.RealFileName = link.RealFileName;
                    file.FileName = link.FileName;

                    files.Add(file);
                    _context.Files.Add(file);
                }

                taskEntry.Attachments = files.Select(f => new TaskFile { File = f, Task = taskEntry }).ToList();
            }
            else
            {
                taskEntry.Attachments = new List<TaskFile>();
            }

            _context.Tasks.Add(taskEntry);
            _context.SaveChanges();

            return GetTasks().Where(t => t.TaskId == _context.Tasks.Max(t1 => t1.TaskId)).Select(_taskMapper.Map).First();
        }

        public int? GetTaskExecutiveId(int taskId)
        {
            var task = _context.Tasks.First(t => t.TaskId == taskId);

            if (task.Executive == null)
                return -1;

            return task.Executive.UserId;
        }

        public List<TaskModel> GetAll()
        {
            return GetTasks().Select(_taskMapper.Map).ToList();
        }

        public TaskModel GetById(int taskId)
        {
            return _taskMapper.Map(GetTasks().Single(t => t.TaskId == taskId));
        }
    }
}

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
                .Include(t => t.TaskCat)
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
            return _context.TaskCategories.Select(c => new TaskCategoryModel() { Id = c.Id, Name = c.Name }).ToList();
        }

        public List<TaskSubcatModel> GetSubcategories(int taskCategoryId)
        {
            return _context.TaskSubcategories.Where(s => s.TaskCategoryId == taskCategoryId)
                           .Select(c => new TaskSubcatModel()
                           {
                               Id = c.Id,
                               Name = c.Name,
                               TaskCategoryId = c.TaskCategoryId
                           }).ToList();
        }

        public List<TaskModel> GetUserSubscribedTasks(int userId)
        {
            User user = _context.Users.First(u => u.Id == userId);

            List<TaskModel> subTasks = new List<TaskModel>();

            if (_context.Tasks.Where(task => task.TaskSubscribers.Any()).
                Any(task => task.TaskSubscribers.Any(t1 => t1.SubscriberId == userId)))
            {
                subTasks = GetTasks().Where(task => task.TaskSubscribers.Any()).
                                     Where(task => task.TaskSubscribers.Any(t1 => t1.SubscriberId == userId)).
                                     Select(_taskMapper.Map).ToList();
            }

            return subTasks;
        }

        public List<TaskModel> GetUserExecutiveTasks(int userId)
        {
            return GetTasks().Where(t => t.ExecutiveId == userId).Select(_taskMapper.Map).ToList();
        }

        public void CheckTaskAsHidden(int taskId)
        {
            TaskEntry taskEntry = _context.Tasks.First(t => t.Id == taskId);

            if (taskEntry.Hide == null)
            {
                taskEntry.Hide = true;
            }
            else
            {
                taskEntry.Hide = !taskEntry.Hide;
            }

            _context.SaveChanges();
        }

        public List<TaskCommentModel> GetNewTaskComments(int taskId, int? taskCommentId)
        {
            TaskEntry taskEntry = _context.Tasks.First(p => p.Id == taskId);

            if (taskCommentId == null)
            {
                if (taskEntry.TaskComments.Any())
                {
                    return taskEntry.TaskComments.Select(c => new TaskCommentModel()
                    {
                        Id = c.Id,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        TaskId = c.TaskId,
                        Creator = new UserShortModel()
                        {
                            Id = c.Creator.Id,
                            Username = c.Creator.Username,
                            Avatar = c.Creator.Avatar
                        }
                    }).ToList();
                }
                else
                {
                    return new List<TaskCommentModel>();
                }
            }
            else
            {
                TaskComment comment = taskEntry.TaskComments.First(c => c.Id == taskCommentId);

                List<TaskComment> comments = taskEntry.TaskComments.Where(p => comment.Id < p.Id).ToList();

                if (comments.Any())
                {
                    return comments.Select(c => new TaskCommentModel()
                    {
                        Id = c.Id,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        TaskId = c.TaskId,
                        Creator = UserShortModel.From(c.Creator.Id, c.Creator.Username, c.Creator.Avatar)
                    }).ToList();
                }
                else
                {
                    return new List<TaskCommentModel>();
                }
            }
        }

        public LikeResult LikeTask(UserModel userModel, int taskId)
        {
            User user = _context.Users.First(u => u.Id == userModel.Id);

            if (_context.TaskLikes.Any(l => l.LikerId == user.Id && l.TaskId == taskId))
            {
                List<TaskLike> likes =
                    _context.TaskLikes.Where(l => l.LikerId == user.Id && l.TaskId == taskId).ToList();
                _context.TaskLikes.RemoveRange(likes);
                _context.SaveChanges();

                return LikeResult.Unliked;
            }
            else
            {
                TaskLike like = new TaskLike();

                like.TaskEntry = _context.Tasks.First(p => p.Id == taskId);
                like.TaskId = like.TaskEntry.Id;

                like.Liker = user;
                like.LikerId = user.Id;

                _context.TaskLikes.Add(like);

                if (like.TaskEntry.Creator.Id != user.Id)
                {
                    Notification notification = Notification.From(DateTime.Now, NotificationType.TaskLike,
                        like.TaskEntry.Creator, user, "/Tasks/Id/" + taskId, "@" + user.Username + " оцінив ваше завдання");
                    _context.Notifications.Add(notification);
                }
                _context.SaveChanges();

                return LikeResult.Liked;
            }
        }

        public TaskCommentModel WriteTaskComment(UserModel userModel, int taskId, string text)
        {
            User user = _context.Users.First(u => u.Id == userModel.Id);

            TaskComment comment = new TaskComment();

            comment.Content = text;
            comment.TaskId = taskId;
            comment.TaskEntry = _context.Tasks.First(p => p.Id == taskId);
            comment.Creator = user;
            comment.CreatorId = user.Id;
            comment.CreationTime = DateTime.Now;

            if (comment.TaskEntry.CreatorId != user.Id)
            {
                Notification notification = Notification.From(DateTime.Now, NotificationType.TaskComment,
                    user, comment.TaskEntry.Creator, "/Tasks/Id/" + taskId, "@" + user.Username + " коментував ваше завдання");

                _context.Notifications.Add(notification);
            }

            _context.TaskComments.Add(comment);
            _context.SaveChanges();

            return _context.TaskComments.Where(c => c.TaskId == taskId &&
                   c.CreationTime == comment.CreationTime).Select(dbComment => new TaskCommentModel()
                   {
                       Id = dbComment.Id,
                       CreationTime = dbComment.CreationTime,
                       Content = dbComment.Content,
                       TaskId = dbComment.TaskId,
                       Creator = new UserShortModel()
                       {
                           Id = dbComment.Creator.Id,
                           Username = dbComment.Creator.Username,
                           Avatar = dbComment.Creator.Avatar
                       }
                   }).OrderByDescending(c => c.Id).Take(1).Single();
        }

        public void CheckTaskAsSolved(int taskId)
        {
            TaskEntry taskEntry = _context.Tasks.First(t => t.Id == taskId);
            if (taskEntry.Solved == true)
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
                        taskEntry.Executive.Achievements = new List<TaskCategory>();

                    if (!taskEntry.Executive.Achievements.Any(t => t.Name == taskEntry.TaskCat.Name) &&
                        _context.Tasks.Count(t => t.Solved == true && t.Executive.Id == taskEntry.Executive.Id) >= 3)
                    {
                        taskEntry.Executive.Achievements.Add(taskEntry.TaskCat);

                        Notification notificationAchievement = Notification.From(DateTime.Now, NotificationType.Achievement,
                            taskEntry.Creator, taskEntry.Executive, "/Tasks/Id/" + taskEntry.Id, "Новий бейдж в категорії \"" + taskEntry.TaskCat.Name + "\"");

                        _context.Notifications.Add(notificationAchievement);
                    }

                    Notification notificationSolved = Notification.From(DateTime.Now, NotificationType.Solved,
                        taskEntry.Creator, taskEntry.Executive, "/Tasks/Id/" + taskEntry.Id, "Завдання виконане!");

                    _context.Notifications.Add(notificationSolved);
                }
            }

            _context.SaveChanges();
        }

        public void CheckAsTaskMainExecutive(int taskId, string username)
        {
            TaskEntry taskEntry = _context.Tasks.First(t => t.Id == taskId);
            User user = _context.Users.First(u => u.Username == username);

            taskEntry.TaskSubscribers.RemoveAll(t => t.Subscriber == user);

            if (taskEntry.ExecutiveId == user.Id)
            {
                taskEntry.Executive = null;
                taskEntry.ExecutiveId = null;
            }
            else
            {
                taskEntry.Executive = user;
                taskEntry.ExecutiveId = user.Id;
            }

            Notification notification = Notification.From(DateTime.Now, NotificationType.CheckedAsTaskExecutive,
                taskEntry.Creator, user, "/Tasks/Id/" + taskEntry.Id, "@" + taskEntry.Creator.Username + " поставив вас виконавцем завдання");

            _context.Notifications.Add(notification);

            _context.SaveChanges();
        }

        public void RemoveTaskExecutive(int taskId)
        {
            TaskEntry taskEntry = _context.Tasks.First(t => t.Id == taskId);

            taskEntry.TaskSubscribers.RemoveAll(t => t.SubscriberId == taskEntry.ExecutiveId);
            taskEntry.ExecutiveId = null;
            taskEntry.Executive = null;

            _context.SaveChanges();
        }

        public void SubscribeOnTheTask(int senderId, int taskId, int? taskPrice)
        {
            User sender = _context.Users.First(u => u.Id == senderId);

            TaskEntry taskEntry = _context.Tasks.First(t => t.Id == taskId);

            if (taskEntry.Executive != null)
            {
                if (taskEntry.Executive.Id == sender.Id)
                    throw new SameUserException();
            }

            if (sender.Id == taskEntry.Creator.Id)
                throw new SameUserException();

            if (taskEntry.TaskSubscribers == null)
                taskEntry.TaskSubscribers = new List<TaskSubscriber>();
            else
            {
                if (taskEntry.TaskSubscribers.Any(t => t.SubscriberId == senderId))
                    throw new SameUserException();
            }

            taskEntry.TaskSubscribers.RemoveAll(s => s.SubscriberId == senderId);
            taskEntry.TaskSubscribers.Add(new TaskSubscriber() { Subscriber = sender, Price = taskPrice });

            Notification notification = Notification.From(DateTime.Now, NotificationType.TaskSubscribed,
                sender, taskEntry.Creator, "/Tasks/Id/" + taskEntry.Id, " підписався на виконання завдання");

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public List<TaskModel> SearchTasks(string request, int? userId, int? category,
            int? subcategory, int? minPrice, int? maxPrice)
        {
            List<TaskModel> tasks = new List<TaskModel>();

            if (userId == null)
            {
                tasks = GetTasks().Select(_taskMapper.Map).ToList();
            }
            else
            {
                User user = _context.Users.First(u => u.Id == userId.Value);

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
            _context.Tasks.Remove(_context.Tasks.First(t => t.Id == taskId));
            _context.SaveChanges();
        }

        public void AddExecutionReview(ExecutionReviewModel model)
        {
            TaskExecutionReview review = new TaskExecutionReview();
            TaskEntry taskEntry = _context.Tasks.First(t => t.Id == model.TaskId);

            review.TaskId = model.TaskId;
            review.TaskEntry = taskEntry;
            review.ExecutorId = taskEntry.ExecutiveId;
            review.Executor = taskEntry.Executive;

            review.Rating = model.Rating;
            review.Review = model.Review;

            float? sum = _context.Reviews.Where(r => r.ExecutorId == review.ExecutorId).Sum(r => r.Rating);
            int length = _context.Reviews.Count(r => r.ExecutorId == review.ExecutorId) + 1;
            review.Executor.Rating = (float)(Math.Round((((sum == null ? 0 : sum.Value) + review.Rating.Value) / length), 2));

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
            TaskEntry taskEntry = new TaskEntry();

            taskEntry.Header = header;
            taskEntry.Content = content;
            taskEntry.Price = price;
            taskEntry.Solved = false;
            taskEntry.CreatorId = userId;
            taskEntry.Creator = _context.Users.First(u => u.Id == userId);
            taskEntry.TaskCategoryId = category;
            taskEntry.TaskCat = _context.TaskCategories.First(c => c.Id == category);
            taskEntry.TaskSubcategoryId = subcategory;
            taskEntry.TaskSubcategory = _context.TaskSubcategories.First(s => s.Id == subcategory);
            taskEntry.TaskLikes = new List<TaskLike>();
            taskEntry.TaskComments = new List<TaskComment>();
            taskEntry.TaskSubscribers = new List<TaskSubscriber>();

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
                taskEntry.Attachments = files;
            }
            else
            {
                taskEntry.Attachments = new List<File>();
            }

            _context.Tasks.Add(taskEntry);
            _context.SaveChanges();

            return GetTasks().Where(t => t.Id == _context.Tasks.Max(t1 => t1.Id)).Select(_taskMapper.Map).First();
        }

        public int? GetTaskExecutiveId(int taskId)
        {
            TaskEntry t = _context.Tasks.First(t1 => t1.Id == taskId);

            if (t.Executive == null)
                return -1;
            else
                return t.Executive.Id;
        }

        public List<TaskModel> GetAll()
        {
            return GetTasks().Select(_taskMapper.Map).ToList();
        }

        public TaskModel GetById(int taskId)
        {
            return _taskMapper.Map(GetTasks().Single(t => t.Id == taskId));
        }
    }
}

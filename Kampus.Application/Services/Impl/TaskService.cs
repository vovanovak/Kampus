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

        private IQueryable<Task> GetTasks()
        {
            return _context.Tasks
                .Include(t => t.TaskCat)
                .Include(t => t.TaskSubcat)
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
            return _context.TaskSubcats.Where(s => s.TaskCategoryId == taskCategoryId)
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
            Task task = _context.Tasks.First(t => t.Id == taskId);

            if (task.Hide == null)
            {
                task.Hide = true;
            }
            else
            {
                task.Hide = !task.Hide;
            }

            _context.SaveChanges();
        }

        public List<TaskCommentModel> GetNewTaskComments(int taskId, int? taskCommentId)
        {
            Task task = _context.Tasks.First(p => p.Id == taskId);

            if (taskCommentId == null)
            {
                if (task.TaskComments.Any())
                {
                    return task.TaskComments.Select(c => new TaskCommentModel()
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
                TaskComment comment = task.TaskComments.First(c => c.Id == taskCommentId);

                List<TaskComment> comments = task.TaskComments.Where(p => comment.Id < p.Id).ToList();

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

                like.Task = _context.Tasks.First(p => p.Id == taskId);
                like.TaskId = like.Task.Id;

                like.Liker = user;
                like.LikerId = user.Id;

                _context.TaskLikes.Add(like);

                if (like.Task.Creator.Id != user.Id)
                {
                    Notification notification = Notification.From(DateTime.Now, NotificationType.TaskLike,
                        like.Task.Creator, user, "/Tasks/Id/" + taskId, "@" + user.Username + " оцінив ваше завдання");
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
            comment.Task = _context.Tasks.First(p => p.Id == taskId);
            comment.Creator = user;
            comment.CreatorId = user.Id;
            comment.CreationTime = DateTime.Now;

            if (comment.Task.CreatorId != user.Id)
            {
                Notification notification = Notification.From(DateTime.Now, NotificationType.TaskComment,
                    user, comment.Task.Creator, "/Tasks/Id/" + taskId, "@" + user.Username + " коментував ваше завдання");

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
            Task task = _context.Tasks.First(t => t.Id == taskId);
            if (task.Solved == true)
            {
                task.Solved = false;
                task.Executive = null;
                task.ExecutiveId = null;
            }
            else
            {
                task.Solved = true;

                if (task.Executive != null)
                {
                    if (task.Executive.Achievements == null)
                        task.Executive.Achievements = new List<TaskCategory>();

                    if (!task.Executive.Achievements.Any(t => t.Name == task.TaskCat.Name) &&
                        _context.Tasks.Count(t => t.Solved == true && t.Executive.Id == task.Executive.Id) >= 3)
                    {
                        task.Executive.Achievements.Add(task.TaskCat);

                        Notification notificationAchievement = Notification.From(DateTime.Now, NotificationType.Achievement,
                            task.Creator, task.Executive, "/Tasks/Id/" + task.Id, "Новий бейдж в категорії \"" + task.TaskCat.Name + "\"");

                        _context.Notifications.Add(notificationAchievement);
                    }

                    Notification notificationSolved = Notification.From(DateTime.Now, NotificationType.Solved,
                        task.Creator, task.Executive, "/Tasks/Id/" + task.Id, "Завдання виконане!");

                    _context.Notifications.Add(notificationSolved);
                }
            }

            _context.SaveChanges();
        }

        public void CheckAsTaskMainExecutive(int taskId, string username)
        {
            Task task = _context.Tasks.First(t => t.Id == taskId);
            User user = _context.Users.First(u => u.Username == username);

            task.TaskSubscribers.RemoveAll(t => t.Subscriber == user);

            if (task.ExecutiveId == user.Id)
            {
                task.Executive = null;
                task.ExecutiveId = null;
            }
            else
            {
                task.Executive = user;
                task.ExecutiveId = user.Id;
            }

            Notification notification = Notification.From(DateTime.Now, NotificationType.CheckedAsTaskExecutive,
                task.Creator, user, "/Tasks/Id/" + task.Id, "@" + task.Creator.Username + " поставив вас виконавцем завдання");

            _context.Notifications.Add(notification);

            _context.SaveChanges();
        }

        public void RemoveTaskExecutive(int taskId)
        {
            Task task = _context.Tasks.First(t => t.Id == taskId);

            task.TaskSubscribers.RemoveAll(t => t.SubscriberId == task.ExecutiveId);
            task.ExecutiveId = null;
            task.Executive = null;

            _context.SaveChanges();
        }

        public void SubscribeOnTheTask(int senderId, int taskId, int? taskPrice)
        {
            User sender = _context.Users.First(u => u.Id == senderId);

            Task task = _context.Tasks.First(t => t.Id == taskId);

            if (task.Executive != null)
            {
                if (task.Executive.Id == sender.Id)
                    throw new SameUserException();
            }

            if (sender.Id == task.Creator.Id)
                throw new SameUserException();

            if (task.TaskSubscribers == null)
                task.TaskSubscribers = new List<TaskSubscriber>();
            else
            {
                if (task.TaskSubscribers.Any(t => t.SubscriberId == senderId))
                    throw new SameUserException();
            }

            task.TaskSubscribers.RemoveAll(s => s.SubscriberId == senderId);
            task.TaskSubscribers.Add(new TaskSubscriber() { Subscriber = sender, Price = taskPrice });

            Notification notification = Notification.From(DateTime.Now, NotificationType.TaskSubscribed,
                sender, task.Creator, "/Tasks/Id/" + task.Id, " підписався на виконання завдання");

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
            ExecutionReview review = new ExecutionReview();
            Task task = _context.Tasks.First(t => t.Id == model.TaskId);

            review.TaskId = model.TaskId;
            review.Task = task;
            review.ExecutorId = task.ExecutiveId;
            review.Executor = task.Executive;

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
            Task task = new Task();

            task.Header = header;
            task.Content = content;
            task.Price = price;
            task.Solved = false;
            task.CreatorId = userId;
            task.Creator = _context.Users.First(u => u.Id == userId);
            task.TaskCategoryId = category;
            task.TaskCat = _context.TaskCategories.First(c => c.Id == category);
            task.TaskSubcatId = subcategory;
            task.TaskSubcat = _context.TaskSubcats.First(s => s.Id == subcategory);
            task.TaskLikes = new List<TaskLike>();
            task.TaskComments = new List<TaskComment>();
            task.TaskSubscribers = new List<TaskSubscriber>();

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
                task.Attachments = files;
            }
            else
            {
                task.Attachments = new List<File>();
            }

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return GetTasks().Where(t => t.Id == _context.Tasks.Max(t1 => t1.Id)).Select(_taskMapper.Map).First();
        }

        public int? GetTaskExecutiveId(int taskId)
        {
            Task t = _context.Tasks.First(t1 => t1.Id == taskId);

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

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;
using Kampus.DAL.Abstract.Repositories;
using Kampus.DAL.Exceptions;
using Kampus.DAL.Enums;

namespace Kampus.DAL.Concrete.Repositories
{
    internal class TaskRepositoryBase : RepositoryBase<TaskModel, Task>, ITaskRepository
    {
        public TaskRepositoryBase(KampusContext context) : base(context)
        {
        }

        protected override DbSet<Task> GetTable()
        {
            return ctx.Tasks;
        }

        protected override Expression<Func<Task, TaskModel>> GetConverter()
        {
            return t => new TaskModel()
            {
                Id = t.Id,
                Category = t.TaskCat.Id,
                CategoryName = t.TaskCat.Name,
                Header = t.Header,
                Content = t.Content,
                Hide = t.Hide,
                Solved = t.Solved,
                Price = t.Price,
                Subcategory = t.TaskSubcatId,
                SubcategoryName = t.TaskSubcat.Name,
                Creator = new UserShortModel() { Id = t.Creator.Id, Username = t.Creator.Username, Avatar = t.Creator.Avatar },
                Executive = (t.Executive != null) ? new UserShortModel() { Id = t.Executive.Id, Username = t.Executive.Username, Avatar = t.Executive.Avatar } : null,
                Likes = ctx.TaskLikes.Where(l => l.TaskId == t.Id).Select(l1 => l1.Liker).
                    Select(p2 => new UserShortModel() { Id = p2.Id, Username = p2.Username, Avatar = p2.Avatar }).ToList(),
                Subscribers = t.TaskSubscribers.Select(p2 => new TaskSubscriberModel() { Id = p2.Id, Price = p2.Price, Subscriber = new UserShortModel() { Id = p2.Subscriber.Id, Username = p2.Subscriber.Username, Avatar = p2.Subscriber.Avatar } }).ToList(),
                Comments = ctx.TaskComments.Where(c => c.TaskId == t.Id).Select(t1 => new TaskCommentModel() { Content = t1.Content, Id = t1.Id, TaskId = t1.TaskId, Creator = new UserShortModel() { Id = t1.Creator.Id, Username = t1.Creator.Username, Avatar = t1.Creator.Avatar } }).ToList(),
                Attachments = t.Attachments.Select(f => new FileModel()
                {
                    Id = f.Id,
                    RealFileName = f.RealFileName,
                    FileName = f.FileName
                }).ToList()
            };
        }

        protected override void UpdateEntry(Task dbEntity, TaskModel entity)
        {
            dbEntity.Id = entity.Id;
            dbEntity.Header = entity.Header;
            dbEntity.Content = entity.Content;
            dbEntity.Price = entity.Price;
            dbEntity.Executive = (entity.Executive == null) ? null : ctx.Users.First(u => u.Id == entity.Executive.Id);
            dbEntity.Creator = (entity.Creator == null) ? null : ctx.Users.First(u => u.Id == entity.Creator.Id);
            dbEntity.TaskSubcatId = entity.Subcategory;
            dbEntity.TaskCategoryId = entity.Category;
            dbEntity.Solved = entity.Solved;
            dbEntity.Hide = entity.Hide;
        }

        public List<TaskModel> GetUserTasks(int userId)
        {
            return ctx.Tasks.Where(t => t.CreatorId == userId).Select(GetConverter()).OrderByDescending(t => t.Id).ToList();

        }

        public List<TaskModel> GetUserSolvedTasks(int userId)
        {
            return ctx.Tasks.Where(t => t.CreatorId == userId && t.Solved == true).Select(GetConverter()).ToList();
        }

        public List<TaskCategoryModel> GetTaskCategories()
        {
            return ctx.TaskCategories.Select(c => new TaskCategoryModel() { Id = c.Id, Name = c.Name }).ToList();
        }

        public List<TaskSubcatModel> GetSubcategories(int taskCategoryId)
        {
            return ctx.TaskSubcats.Where(s => s.TaskCategoryId == taskCategoryId)
                           .Select(c => new TaskSubcatModel() {
                               Id = c.Id,
                               Name = c.Name,
                               TaskCategoryId = c.TaskCategoryId }).ToList();
        }

        public List<TaskModel> GetUserSubscribedTasks(int userId)
        {
            User user = ctx.Users.First(u => u.Id == userId);

            List<TaskModel> subTasks = new List<TaskModel>();

            if (ctx.Tasks.Where(task => task.TaskSubscribers.Any()).
                Any(task => task.TaskSubscribers.Any(t1 => t1.SubscriberId == userId)))
            {
                subTasks = ctx.Tasks.Where(task => task.TaskSubscribers.Any()).
                                     Where(task => task.TaskSubscribers.Any(t1 => t1.SubscriberId == userId)).
                                     Select(GetConverter()).ToList();
            }

            return subTasks;
        }

        public List<TaskModel> GetUserExecutiveTasks(int userId)
        {
            return ctx.Tasks.Where(t => t.ExecutiveId == userId).Select(GetConverter()).ToList();
        }

        public void CheckTaskAsHidden(int taskId)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskId);

            if (task.Hide == null)
            {
                task.Hide = true;
            }
            else
            {
                task.Hide = !task.Hide;
            }

            ctx.SaveChanges();
        }

        public void CreateTask(TaskModel model)
        {
            Save(model);
        }

        public List<TaskCommentModel> GetNewTaskComments(int taskId, int? taskCommentId)
        {
            Task task = ctx.Tasks.First(p => p.Id == taskId);

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
                        Creator = new UserShortModel() { Id = c.Creator.Id,
                                                         Username = c.Creator.Username,
                                                         Avatar = c.Creator.Avatar }
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
            User user = ctx.Users.First(u => u.Id == userModel.Id);

            if (ctx.TaskLikes.Any(l => l.LikerId == user.Id && l.TaskId == taskId))
            {
                List<TaskLike> likes =
                    ctx.TaskLikes.Where(l => l.LikerId == user.Id && l.TaskId == taskId).ToList();
                ctx.TaskLikes.RemoveRange(likes);
                ctx.SaveChanges();

                return LikeResult.Unliked;
            }
            else
            {
                TaskLike like = new TaskLike();

                like.Task = ctx.Tasks.First(p => p.Id == taskId);
                like.TaskId = like.Task.Id;

                like.Liker = user;
                like.LikerId = user.Id;

                ctx.TaskLikes.Add(like);

                if (like.Task.Creator.Id != user.Id)
                {
                    Notification notification = Notification.From(DateTime.Now, NotificationType.TaskLike,
                        like.Task.Creator, user, "/Tasks/Id/" + taskId, "@" + user.Username + " оцінив ваше завдання");
                    ctx.Notifications.Add(notification);
                }
                ctx.SaveChanges();

                return LikeResult.Liked;
            }
        }

        public TaskCommentModel WriteTaskComment(UserModel userModel, int taskId, string text)
        {
            User user = ctx.Users.First(u => u.Id == userModel.Id);

            TaskComment comment = new TaskComment();

            comment.Content = text;
            comment.TaskId = taskId;
            comment.Task = ctx.Tasks.First(p => p.Id == taskId);
            comment.Creator = user;
            comment.CreatorId = user.Id;
            comment.CreationTime = DateTime.Now;

            if (comment.Task.CreatorId != user.Id)
            {
                Notification notification = Notification.From(DateTime.Now, NotificationType.TaskComment,
                    user, comment.Task.Creator, "/Tasks/Id/" + taskId, "@" + user.Username + " коментував ваше завдання");

                ctx.Notifications.Add(notification);
            }

            ctx.TaskComments.Add(comment);
            ctx.SaveChanges();

            return ctx.TaskComments.Where(c => c.TaskId == taskId &&
                   c.CreationTime == comment.CreationTime).Select(dbComment => new TaskCommentModel()
                   {
                       Id = dbComment.Id,
                       CreationTime = dbComment.CreationTime,
                       Content = dbComment.Content,
                       TaskId = dbComment.TaskId,
                       Creator = new UserShortModel() { Id = dbComment.Creator.Id,
                                                        Username = dbComment.Creator.Username,
                                                        Avatar = dbComment.Creator.Avatar }
                   }).OrderByDescending(c => c.Id).Take(1).Single();
        }

        public void CheckTaskAsSolved(int taskId)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskId);
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
                        ctx.Tasks.Count(t => t.Solved == true && t.Executive.Id == task.Executive.Id) >= 3)
                    {
                        task.Executive.Achievements.Add(task.TaskCat);

                        Notification notificationAchievement = Notification.From(DateTime.Now, NotificationType.Achievement,
                            task.Creator, task.Executive, "/Tasks/Id/" + task.Id, "Новий бейдж в категорії \"" + task.TaskCat.Name + "\"");

                        ctx.Notifications.Add(notificationAchievement);
                    }

                    Notification notificationSolved = Notification.From(DateTime.Now, NotificationType.Solved,
                        task.Creator, task.Executive, "/Tasks/Id/" + task.Id, "Завдання виконане!");

                    ctx.Notifications.Add(notificationSolved);
                }
            }

            ctx.SaveChanges();
        }

        public void CheckAsTaskMainExecutive(int taskId, string username)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskId);
            User user = ctx.Users.First(u => u.Username == username);

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

            ctx.Notifications.Add(notification);

            ctx.SaveChanges();
        }

        public void RemoveTaskExecutive(int taskId)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskId);

            task.TaskSubscribers.RemoveAll(t => t.SubscriberId == task.ExecutiveId);
            task.ExecutiveId = null;
            task.Executive = null;

            ctx.SaveChanges();
        }

        public void SubscribeOnTheTask(int senderId, int taskId, int? taskPrice)
        {
            User sender = ctx.Users.First(u => u.Id == senderId);

            Task task = ctx.Tasks.First(t => t.Id == taskId);

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

            ctx.Notifications.Add(notification);
            ctx.SaveChanges();
        }

        public List<TaskModel> SearchTasks(string request, int? userId, int? category,
            int? subcategory, int? minPrice, int? maxPrice)
        {
            List<TaskModel> tasks = new List<TaskModel>();
            
            if (userId == null)
            {
                tasks = ctx.Tasks.Select(GetConverter()).ToList();
            }
            else
            {
                User user = ctx.Users.First(u => u.Id == userId.Value);

                tasks = ctx.Tasks.Where(t => t.CreatorId == userId).Select(GetConverter()).ToList();
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
            ctx.Tasks.Remove(ctx.Tasks.First(t => t.Id == taskId));
            ctx.SaveChanges();
        }

        public void AddExecutionReview(ExecutionReviewModel model)
        {
            ExecutionReview review = new ExecutionReview();
            Task task = ctx.Tasks.First(t => t.Id == model.TaskId);

            review.TaskId = model.TaskId;
            review.Task = task;
            review.ExecutorId = task.ExecutiveId;
            review.Executor = task.Executive;

            review.Rating = model.Rating;
            review.Review = model.Review;

            float? sum = ctx.Reviews.Where(r => r.ExecutorId == review.ExecutorId).Sum(r => r.Rating);
            int length = ctx.Reviews.Count(r => r.ExecutorId == review.ExecutorId) + 1;
            review.Executor.Rating = (float)(Math.Round((((sum == null ? 0 : sum.Value) + review.Rating.Value) / length), 2));

            ctx.Reviews.Add(review);
            ctx.SaveChanges();
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
            task.Creator = ctx.Users.First(u => u.Id == userId);
            task.TaskCategoryId = category;
            task.TaskCat = ctx.TaskCategories.First(c => c.Id == category);
            task.TaskSubcatId = subcategory;
            task.TaskSubcat = ctx.TaskSubcats.First(s => s.Id == subcategory);
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
                    ctx.Files.Add(file);
                }
                task.Attachments = files;
            }
            else
            {
                task.Attachments = new List<File>();
            }

            ctx.Tasks.Add(task);
            ctx.SaveChanges();

            return ctx.Tasks.Where(t => t.Id == ctx.Tasks.Max(t1 => t1.Id)).Select(GetConverter()).First();
        }

        public int? GetTaskExecutiveId(int taskId)
        {
            Task t = ctx.Tasks.First(t1 => t1.Id == taskId);

            if (t.Executive == null)
                return -1;
            else
                return t.Executive.Id;
        }
    }
}
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

namespace Kampus.DAL.Concrete
{
    public class TaskRepositoryBase: RepositoryBase<TaskModel, Task>, ITaskRepository
    {
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
                Creator = new UserShortModel() { Id = t.User.Id, Username = t.User.Username, Avatar = t.User.Avatar },
                Executive = (t.Executive != null) ? new UserShortModel() { Id = t.Executive.Id, 
                    Username = t.Executive.Username, Avatar = t.Executive.Avatar } : null,
                Likes = ctx.TaskLikes.Where(l => l.TaskId == t.Id).Select(l1 => l1.User).
                    Select(p2 => new UserShortModel() { Id = p2.Id , Username = p2.Username, Avatar = p2.Avatar }).ToList(),
                Subscribers = t.TaskSubscribers.Select(p2 => new TaskSubscriberModel() { Id = p2.Id, Price = p2.Price, User = new UserShortModel() { Id = p2.User.Id, Username = p2.User.Username, Avatar = p2.User.Avatar } }).ToList(),
                Comments = ctx.TaskComments.Where(c => c.TaskId == t.Id).Select(t1 => new TaskCommentModel() { Content = t1.Content, Id = t1.Id, TaskId = t1.TaskId, User = new UserShortModel() { Id = t1.User.Id, Username = t1.User.Username, Avatar = t1.User.Avatar } }).ToList(),
                Attachments = t.Attachments.Select(f => new FileModel(){ Id = f.Id, 
                    RealFileName = f.RealFileName,
                    FileName = f.FileName, 
                    Extension = f.FileName.Substring(f.FileName.IndexOf(".")),
                    IsImage = f.FileName.Substring(f.FileName.IndexOf(".")) == ".jpg"}).ToList()
            };
        }

        protected override void UpdateEntry(Task dbEntity, TaskModel entity)
        {
            dbEntity.Id = entity.Id;
            dbEntity.Header = entity.Header;
            dbEntity.Content = entity.Content;
            dbEntity.Price = entity.Price;
            dbEntity.Executive = (entity.Executive == null) ? null : ctx.Users.First(u => u.Id == entity.Executive.Id);
            dbEntity.User = (entity.Creator == null) ? null : ctx.Users.First(u => u.Id == entity.Creator.Id);
            dbEntity.TaskSubcatId = entity.Subcategory;
            dbEntity.TaskCategoryId = entity.Category;
            dbEntity.Solved = entity.Solved;
            dbEntity.Hide = entity.Hide;
        }

        public List<TaskModel> GetUserTasks(int userid)
        {
            return ctx.Tasks.Where(t => t.UserId == userid).Select(GetConverter()).OrderByDescending(t => t.Id).ToList();

        }

        public List<TaskModel> GetUserSolvedTasks(int userid)
        {
            return ctx.Tasks.Where(t => t.UserId == userid && t.Solved == true).Select(GetConverter()).ToList();
        }

     

        public List<TaskCategoryModel> GetTaskCategories()
        {
            List<TaskCategoryModel> categories = new List<TaskCategoryModel>();
            foreach (var c in ctx.TaskCategories)
            {
                categories.Add(new TaskCategoryModel() { Id = c.Id, Name = c.Name});
            }
            return categories;
        }

        public List<TaskSubcatModel> GetSubcategories(int TaskCategoryId)
        {
            List<TaskSubcatModel> subcategories = new List<TaskSubcatModel>();
            foreach (var c in ctx.TaskSubcats)
            {
                if (c.TaskCategoryId == TaskCategoryId)
                {
                    subcategories.Add(new TaskSubcatModel() { Id = c.Id, Name = c.Name, TaskCategoryId = c.TaskCategoryId}); 
                }
            }
            return subcategories;
        }

        public List<TaskModel> GetUserSubscribedTasks(int userid)
        {
            User user = ctx.Users.First(u => u.Id == userid);

            List<TaskModel> subTasks = new List<TaskModel>();

            if (ctx.Tasks.Where(task => task.TaskSubscribers.Any()).
                Any(task => task.TaskSubscribers.Any(t1 => t1.UserId == userid)))
            {
                subTasks = ctx.Tasks.Where(task => task.TaskSubscribers.Any()).
                Where(task => task.TaskSubscribers.Any(t1 => t1.UserId == userid)).
                Select(GetConverter()).ToList();
            }

            return subTasks;
        }

        public List<TaskModel> GetUserExecutiveTasks(int userid)
        {
            List<TaskModel> tasks = new List<TaskModel>();
            tasks = ctx.Tasks.Where(t => t.ExecutiveId == userid).Select(GetConverter()).ToList();
            return tasks;
        }

        public void CheckTaskAsHidden(int taskid)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskid);

            if (task.Hide == null)
            {
                task.Hide = true;
            }
            else
            {
                if (task.Hide == true)
                {
                    task.Hide = false;
                }
                else
                {
                    task.Hide = true;
                }
            }

            ctx.SaveChanges();
        }

        public void CreateTask(TaskModel model)
        {
            Save(model);
        }

        public List<TaskCommentModel> GetNewTaskComments(int taskid, int? taskcommentid)
        {
            Task task = ctx.Tasks.First(p => p.Id == taskid);

            if (taskcommentid == null)
            {
                if (task.TaskComments.Any())
                {
                    List<TaskCommentModel> comments = task.TaskComments.Select(c => new TaskCommentModel()
                    {
                        Id = c.Id,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        TaskId = c.TaskId,
                        User = new UserShortModel() { Id = c.User.Id, Username = c.User.Username, Avatar = c.User.Avatar }
                    }).ToList();
                    return comments;
                }
                else
                {
                    return new List<TaskCommentModel>();
                }
            }
            else
            {

                TaskComment comment = task.TaskComments.First(c => c.Id == taskcommentid);

                List<TaskComment> comments =
                    task.TaskComments.Where(p => comment.Id < p.Id).ToList();

                if (comments.Any())
                {
                    return comments.Select(c => new TaskCommentModel()
                    {
                        Id = c.Id,
                        CreationTime = c.CreationTime,
                        Content = c.Content,
                        TaskId = c.TaskId,
                        User = new UserShortModel() { Id = c.User.Id, Username = c.User.Username, Avatar = c.User.Avatar }
                    }).ToList();
                }
                else
                {
                    return new List<TaskCommentModel>();
                }
            }
        } 

        public int LikeTask(UserModel userModel, int taskid)
        {
            User user = ctx.Users.First(u => u.Id == userModel.Id);

            int res = 0;

            if (ctx.TaskLikes.Any(l => l.UserId == user.Id && l.TaskId == taskid))
            {
                List<TaskLike> likes =
                    ctx.TaskLikes.Where(l => l.UserId == user.Id && l.TaskId == taskid).ToList();
                ctx.TaskLikes.RemoveRange(likes);
            }
            else
            {
                TaskLike like = new TaskLike();

                like.Task = ctx.Tasks.First(p => p.Id == taskid);
                like.TaskId = like.Task.Id;

                like.User = user;
                like.UserId = user.Id;

                ctx.TaskLikes.Add(like);

                if (like.Task.User.Id != user.Id)
                {
                    Notification notification = new Notification();
                    notification.Date = DateTime.Now;
                    notification.Type = NotificationType.TaskLike;
                    notification.User = like.Task.User;
                    notification.UserId = like.Task.User.Id;
                    notification.Sender = user;
                    notification.SenderId = user.Id;
                    notification.Link = "/Tasks/Id/" + taskid;
                    notification.Message = "@" + user.Username + " оцінив ваше завдання";

                    ctx.Notifications.Add(notification);
                }

                res = 1;
            }

            ctx.SaveChanges();

            return res;
        }

        public void WriteTaskComment(UserModel userModel, int taskid, string text)
        {
            User user = ctx.Users.First(u => u.Id == userModel.Id);

            TaskComment comment = new TaskComment();

            comment.Content = text;
            comment.TaskId = taskid;
            comment.Task = ctx.Tasks.First(p => p.Id == taskid);
            comment.User = user;
            comment.UserId = user.Id;
            comment.CreationTime = DateTime.Now;

            if (comment.Task.User.Id != user.Id)
            {
                Notification notification = new Notification();
                notification.Date = DateTime.Now;
                notification.Type = NotificationType.TaskComment;
                notification.User = comment.Task.User;
                notification.UserId = comment.Task.User.Id;
                notification.SenderId = user.Id;
                notification.Sender = user;
                notification.Link = "/Tasks/Id/" + taskid;
                notification.Message = "@" + user.Username + " коментував ваше завдання";

                ctx.Notifications.Add(notification);
            }

            ctx.TaskComments.Add(comment);
            ctx.SaveChanges();
        }

        public void CheckTaskAsSolved(int taskid)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskid);
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

                        Notification notificationAchievement = new Notification();
                        notificationAchievement.Date = DateTime.Now;
                        notificationAchievement.Type = NotificationType.Achievement;
                        notificationAchievement.User = task.Executive;
                        notificationAchievement.UserId = task.Executive.Id;
                        notificationAchievement.Sender = task.User;
                        notificationAchievement.SenderId = task.User.Id;
                        notificationAchievement.Link = "/Tasks/Id/" + task.Id;
                        notificationAchievement.Message = "Новий бейдж в категорії \"" + task.TaskCat.Name + "\"";

                        ctx.Notifications.Add(notificationAchievement);
                    }

                    Notification notificationSolved = new Notification();
                    notificationSolved.Date = DateTime.Now;
                    notificationSolved.Type = NotificationType.Solved;
                    notificationSolved.User = task.Executive;
                    notificationSolved.UserId = task.Executive.Id;
                    notificationSolved.Sender = task.User;
                    notificationSolved.SenderId = task.User.Id;
                    notificationSolved.Link = "/Tasks/Id/" + task.Id;
                    notificationSolved.Message = "Завдання виконане!";

                    ctx.Notifications.Add(notificationSolved);
                }
            }

            

            ctx.SaveChanges();
        }

        public void CheckAsTaskMainExecutive(int taskid, string username)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskid);
            User user = ctx.Users.First(u => u.Username == username);

            if (task.TaskSubscribers.Any(t => t.User == user))
            {
                task.TaskSubscribers.RemoveAll(t => t.User == user);
            }

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

            Notification notification = new Notification();
            notification.Date = DateTime.Now;
            notification.Type = NotificationType.CheckedAsTaskExecutive;
            notification.User = user;
            notification.UserId = user.Id;
            notification.Sender = task.User;
            notification.SenderId = task.User.Id;
            notification.Link = "/Tasks/Id/" + task.Id;
            notification.Message = "@" + task.User.Username + " поставив вас виконавцем завдання";

            ctx.Notifications.Add(notification);

            ctx.SaveChanges();
        }

        public void RemoveTaskExecutive(int taskid)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskid);

            if (task.TaskSubscribers.Any(t => t.UserId == task.ExecutiveId))
                task.TaskSubscribers.RemoveAll(t => t.UserId == task.ExecutiveId);

            task.ExecutiveId = null;
            task.Executive = null;

            ctx.SaveChanges();
        }

        public void SubscribeOnTheTask(int senderid, int taskid, int? taskprice)
        {
            User sender = ctx.Users.First(u => u.Id == senderid);

            Task task = ctx.Tasks.First(t => t.Id == taskid);

            if (task.Executive != null)
            {
                if (task.Executive.Id == sender.Id)
                    throw new SameUserException();
            }


            if (sender.Id == task.User.Id)
                throw new SameUserException();

            if (task.TaskSubscribers == null)
                task.TaskSubscribers = new List<TaskSubscriber>();
            else
            {
                if (task.TaskSubscribers.Any(t => t.UserId == senderid))
                    throw new SameUserException();
            }

            if (task.TaskSubscribers.Any(s => s.UserId == senderid))
                task.TaskSubscribers.RemoveAll(s => s.UserId == senderid);

            task.TaskSubscribers.Add(new TaskSubscriber() { User = sender, Price = taskprice });

            Notification notification = new Notification();
            notification.Date = DateTime.Now;
            notification.Type = NotificationType.TaskSubscribed;
            notification.User = task.User;
            notification.UserId = task.User.Id;
            notification.Sender = sender;
            notification.SenderId = sender.Id;
            notification.Link = "/Tasks/Id/" + task.Id;
            notification.Message = " підписався на виконання завдання";

            ctx.Notifications.Add(notification);

            ctx.SaveChanges();
        }

        public List<TaskModel> SearchTasks(string request, int? userid, int? category,
            int? subcategory, int? minprice, int? maxprice)
        {
            List<TaskModel> tasks = new List<TaskModel>();


            if (userid == null)
            {
                tasks = ctx.Tasks.Select(GetConverter()).ToList();
            }
            else
            {
                User user = ctx.Users.First(u => u.Id == userid.Value);

                tasks = ctx.Tasks.Where(t => t.UserId == userid).Select(GetConverter()).ToList();
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

            if (minprice != null && maxprice != null && minprice.Value < maxprice.Value)
            {
                tasks.RemoveAll(t => t.Price < minprice || t.Price > maxprice);
            }

            return tasks;
        }

        public void RemoveTask(int taskid)
        {
            ctx.Tasks.Remove(ctx.Tasks.First(t => t.Id == taskid));
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

            //try
            //{
            //    review.Executor = ctx.Users.First(u => u.Id == review.ExecutorId);
            //    review.Task = ctx.Tasks.First(t => t.Id == review.TaskId);
            //}
            //catch (InvalidOperationException e)
            //{
                
            //}

            float? sum = ctx.Reviews.Where(r => r.ExecutorId == review.ExecutorId).Sum(r => r.Rating);
            int length = ctx.Reviews.Count(r => r.ExecutorId == review.ExecutorId) + 1;
            review.Executor.Rating = (float)(Math.Round((((sum == null ? 0 : sum.Value) + review.Rating.Value) / length), 2));

            ctx.Reviews.Add(review);
            ctx.SaveChanges();
        }

        public SearchTaskModel UpdateSearchModel(string request, int? userid, int? category, int? subcategory,
            int? minprice, int? maxprice)
        {
            SearchTaskModel model = new SearchTaskModel
            {
                Request = request,
                CategoryId = category,
                SubcategoryId = subcategory,
                MinPrice = minprice,
                MaxPrice = maxprice
            };


            return model;
        }


        public TaskModel CreateTask(int userid, string header, string content, int price,
            int category, int subcategory, List<FileModel> attachments)
        {
            
            Task task = new Task();
            task.Header = header;
            task.Content = content;
            task.Price = price;
            task.Solved = false;
            task.UserId = userid;
            task.User = ctx.Users.First(u => u.Id == userid);
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

            return ctx.Tasks.Where(t => t.Id == ctx.Tasks.Max(t1=>t1.Id)).Select(GetConverter()).First();
        }

        public int? GetTaskExecutiveId(int taskid)
        {
            Task t = ctx.Tasks.First(t1 => t1.Id == taskid);

            if (t.Executive == null)
                return -1;
            else
                return t.Executive.Id;
        }
    }

    
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Kampus.Entities;
using Kampus.Models;
using Task = Kampus.Entities.Task;

namespace Kampus.DAL
{
    public class Db
    {
        private static KampusContext ctx = DbContextSingleton.Context;
        
        public static void RegisterUser(UserModel model)
        {
            int cityId = Convert.ToInt32(model.City);
           
            User user = new User();
            
            user.Username = model.Username;
            user.Password = model.Password;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Status = "";
            user.About = "";
            user.Avatar = model.Avatar;
            user.Fullname = model.FullName;
            user.Rating = 0;
            user.DateOfBirth = DateTime.Now;
            user.Tasks = null;
            user.Messages = null;
            user.City = Db.GetCities().First(c => c.Id == cityId);
            user.Role = Db.GetRoles().First();
            

            if (!model.IsNotStudent)
            {
                AddStudentDetails(model);
            }

            user.StudentDetailsId = ctx.StudentDetails.Max(d => d.Id);


            ctx.Users.Add(user);
            ctx.SaveChanges();
        }

        public static void AddStudentDetails(UserModel model)
        {
            int universityId = Convert.ToInt32(model.UniversityName);
            int facultyId = Convert.ToInt32(model.UniversityFaculty);

            StudentDetails details = new StudentDetails();

            
            details.UniversityId =  universityId;
            details.Course = model.UniversityCourse.Value;
            details.FacultyId = facultyId;

            ctx.StudentDetails.Add(details);
            ctx.SaveChanges();
        }

        public static SignInResult SignIn(string username, string password)
        {
            if (ctx.Users.Any(u => u.Username == username && u.Password == password))
                return SignInResult.Successful;
            else if (ctx.Users.Any(u=>u.Username==username))
                return SignInResult.WrongPassword;
            else 
                return SignInResult.Error;
           
        }

        public static List<City> GetCities()
        {
            List<City> cities = new List<City>();
            foreach (var r in ctx.Cities)
            {
                cities.Add(new City{Id = r.Id, Name = r.Name});
            }
            return cities;
        }

       

        public static List<UserRole> GetRoles()
        {
            List<UserRole> roles = new List<UserRole>();
            foreach (var r in ctx.UserRoles)
            {
                roles.Add(new UserRole(){Id = r.Id, Name = r.Name});
            }
            return roles;
        }

       

        public static List<University> GetUniversities()
        {
            List<University> universities = new List<University>();
            foreach (var u in ctx.Universities)
            {
                universities.Add(new University() { Id = u.Id, Name = u.Name, Faculties = u.Faculties});
            }
            return universities;
        }

         

        public static UniversityFaculty GetFaculty(int id)
        {
            UniversityFaculty f1 = ctx.Faculties.First(f => f.Id == id);
            return f1;
        }

        public static User GetUserByUsername(string username)
        {
            User u1 = ctx.Users.First(u => u.Username == username);
            return u1;
        }

       

        public static List<TaskCategory> GetTaskCategories()
        {
            List<TaskCategory> categories = new List<TaskCategory>();
            foreach (var c in ctx.TaskCategories)
            {
                categories.Add(new TaskCategory() {Id = c.Id, Name = c.Name});
            }
            return categories;
        }

        public static List<TaskSubcat> GetSubcategories(int categoryid)
        {
            List<TaskSubcat> subcategories = new List<TaskSubcat>();
            foreach (var c in ctx.TaskSubcats)
            {
                if (c.TaskCategoryId == categoryid)
                {
                    subcategories.Add(new TaskSubcat() { Id = c.Id, Name = c.Name, TaskCategoryId = c.TaskCategoryId});
                }
            }
            return subcategories;
        }

        public static void CreateTask(User user, TaskModel model)
        {
            Task task = new Task();
            task.Header = model.Header;
            task.Content = model.Content;
            task.Price = model.Price;

           
            task.TaskSubcat = ctx.TaskSubcats.First(c => c.Id == model.Subcategory.Value);
           
            task.User = user;
            task.UserId = user.Id;

            ctx.Tasks.Add(task);
            ctx.SaveChanges();

            int t1Id = ctx.Tasks.Max(t => t.Id);
            Task t1 = ctx.Tasks.First(t => t.Id == t1Id);

            User currentUser = ctx.Users.First(u => u.Id == user.Id);
            currentUser.Tasks.Add(t1);

            ctx.SaveChanges();
        }

        

        public static void LikeTask(User user, int taskid)
        {


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
                    notification.Link = "/Home/Tasks";
                    notification.Message = "User " + user.Username + " liked your task";

                    ctx.Notifications.Add(notification);
                }
            }

            ctx.SaveChanges();
        }

        public static void WriteTaskComment(User user, int taskid, string text)
        {
            TaskComment comment = new TaskComment();

            comment.Content = text;
            comment.TaskId = taskid;
            comment.Task = ctx.Tasks.First(p => p.Id == taskid);
            comment.User = user;
            comment.UserId = user.Id;

            if (comment.Task.User.Id != user.Id)
            {
                Notification notification = new Notification();
                notification.Date = DateTime.Now;
                notification.Type = NotificationType.TaskComment;
                notification.User = comment.Task.User;
                notification.UserId = comment.Task.User.Id;
                notification.Link = "/Home/Tasks";
                notification.Message = "User " + user.Username + " commented your task";

                ctx.Notifications.Add(notification);
            }

            ctx.TaskComments.Add(comment);
            ctx.SaveChanges();

           
        }

        public static void AddSubscriber(User user, User sender)
        {
            User u1 = ctx.Users.First(u => u.Id == user.Id);
            User s1 = ctx.Users.First(s => s.Id == sender.Id);

            s1.Subscribers.Add(u1);

           
            Notification notification = new Notification();
            notification.Date = DateTime.Now;
            notification.Type = NotificationType.Subscribed;
            notification.User = sender;
            notification.UserId = sender.Id;
            notification.Link = "/Home/Friends";
            notification.Message = "User " + sender.Username + " subscribed";

            ctx.Notifications.Add(notification);
            

            ctx.SaveChanges();
        }

        public static void AddFriend(int id, int userid)
        {
            User u1 = ctx.Users.First(u => u.Id == id);
            User u2 = ctx.Users.First(u => u.Id == userid);

            u1.Friends.Add(u2);
            u2.Friends.Add(u1);

            if (u1.Subscribers.Contains(u2))
                u1.Subscribers.Remove(u2);

            if (u2.Subscribers.Contains(u1))
                u2.Subscribers.Remove(u1);

            Notification notification = new Notification();
            notification.Date = DateTime.Now;
            notification.Type = NotificationType.Friendship;
            notification.User = u2;
            notification.UserId = u2.Id;
            notification.Link = "/Home/Friends";
            notification.Message = "User " + u1.Username + " added you as friend";

            ctx.Notifications.Add(notification);

            ctx.SaveChanges();
        }

        public static void RemoveFriend(int id, int friendid)
        {
            User u1 = ctx.Users.First(u => u.Id == id);
            User u2 = ctx.Users.First(u => u.Id == friendid);

            u1.Friends.Remove(u2);
            u1.Subscribers.Add(u2);

            u2.Friends.Remove(u1);

            if (u2.Subscribers.Contains(u1))
                u2.Subscribers.Remove(u1);

            ctx.SaveChanges();

        }

        public static void WriteMessage(int senderid, int receiverid, string text)
        {
            Message msg = new Message();

            User sender = ctx.Users.First(u => u.Id == senderid);
            User receiver = ctx.Users.First(u => u.Id == receiverid);

            msg.Sender = sender;
            msg.SenderId = senderid;

            msg.Receiver = receiver;
            msg.ReceiverId = receiverid;

            msg.Content = text;
            msg.CreationDate = DateTime.Now;

            sender.Messages.Add(msg);

            Notification notification = new Notification();
            notification.Date = DateTime.Now;
            notification.Type = NotificationType.Message;
            notification.User = receiver;
            notification.UserId = receiver.Id;
            notification.Link = "/";
            notification.Message = "User " + sender.Username + " ";

            ctx.Notifications.Add(notification);

            ctx.Messages.Add(msg);
            ctx.SaveChanges();
        }

        public static List<Task> GetUserSubscribedTasks(int userid)
        {
            User user = ctx.Users.First(u => u.Id == userid);

            List<Task> subTasks = new List<Task>();
            
            

            foreach (var task in ctx.Tasks)
            {
                if (task.TaskSubscribers != null)
                {
                    //if (task.TaskSubscribers.ContainsKey(user))
                    //{
                    //    subTasks.Add(task);
                    //}
                }
            }

            return subTasks;
        }

        public static List<Task> GetUserExecutiveTasks(int userid)
        { 
            List<Task> tasks = new List<Task>();
            tasks = ctx.Tasks.Where(t => t.ExecutiveId == userid).ToList();
            return tasks;
        }

        public static void RemoveTaskExecutive(int taskid)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskid);
            task.ExecutiveId = null;
            task.Executive = null;
            ctx.SaveChanges();
        }

        public static List<Message> GetUserMessages(int id)
        {
            List<Message> messages = ctx.Messages.Where(m => m.SenderId == id || m.ReceiverId == id).ToList();
            return messages;
        }

        public static User GetUserById(int id)
        {
            User user = ctx.Users.First(u => u.Id == id);
            return user;
        }

        public static List<Message> GetNewMessages(int senderid, int receiverid, int lastmsgid)
        {
            List<Message> messages = ctx.Messages.Where(m => 
                ((m.SenderId == senderid && m.ReceiverId == receiverid) ||
                (m.SenderId == receiverid && m.ReceiverId == senderid)) &&
                m.Id > lastmsgid).ToList();

            return messages;
        }

        public static void SubscriberTask(User sender, int taskid, int? taskprice)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskid);
            if (task.TaskSubscribers == null)
                task.TaskSubscribers = new List<TaskSubscriber>();
            task.TaskSubscribers.Add(new TaskSubscriber() {User = sender, Price = taskprice});

            Notification notification = new Notification();
            notification.Date = DateTime.Now;
            notification.Type = NotificationType.TaskSubscribed;
            notification.User = task.User;
            notification.UserId = task.User.Id;
            notification.Link = "/";
            notification.Message = "User " + sender.Username + " subscribed to your task";

            ctx.SaveChanges();
        }

        public static void CheckTaskAsSolved(int taskid)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskid);
            if (task.Solved == true)
            {
                task.Solved = false;
            }
            else
            {
                task.Solved = true;
            }
            ctx.SaveChanges();
        }

        public static void CheckAsTaskMainExecutive(int taskid, int userid)
        {
            Task task = ctx.Tasks.First(t => t.Id == taskid);
            User user = ctx.Users.First(u => u.Id == userid);

            if (task.ExecutiveId == userid)
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
            notification.Link = "/";
            notification.Message = "User " + task.User.Username + " checked you as main executive to the task";

            ctx.SaveChanges();
        }

        public static void CreateGroup(GroupModel model)
        {
            //Group group = new Group();

            //group.Name = model.Name;
            //group.Status = model.Status;
            //group.Avatar = model.Avatar;
            //group.Members = new List<User>();
            //group.Creator = ctx.Users.First(u => u.Id == model.Creator);
            //group.CreatorId = model.Creator;

            //group.Admins = new List<User>();
            //group.Admins.Add(group.Creator);
            
            //ctx.Groups.Add(group);

            //group.Creator.Groups.Add(group);

            //ctx.SaveChanges();


        }


        public static void CheckTaskAsHidden(int taskid)
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

        public static Group GetGroup(int groupid)
        {
            Group group = new Group();

            group = ctx.Groups.First(g => g.Id == groupid);

            return group;
        }

        public static void SubscribeForTheGroup(int userid, int groupid, int res)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            Group group = ctx.Groups.First(g => g.Id == groupid);

            if (res == 1)
            {
                group.Members.Add(user);
                user.Groups.Add(group);
            }
            else
            {
                group.Members.Remove(user);
                user.Groups.Remove(group);
            }

            ctx.SaveChanges();
        }

        public static void WriteGroupPost(int userid, int groupid, string content)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            Group group = ctx.Groups.First(g => g.Id == groupid);

            GroupPost post = new GroupPost();

            post.Content = content;
            post.User = user;
            post.UserId = userid;
            post.Group = group;
            post.GroupId = groupid;
            post.CreationTime = DateTime.Now;
            post.Comments = new List<GroupPostComment>();
            post.Likes = new List<GroupPostLike>();

            ctx.GroupPosts.Add(post);
            

            ctx.SaveChanges();
        }

        public static void WriteGroupPostComment(int userid, int postid, string content)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            GroupPost post = ctx.GroupPosts.First(p => p.Id == postid);

            GroupPostComment comment = new GroupPostComment();
            comment.Content = content;
            comment.GroupPost = post;
            comment.GroupPostId = postid;
            comment.User = user;
            comment.UserId = userid;

            ctx.GroupPostComments.Add(comment);
            ctx.SaveChanges();
        }

        public static void LikeGroupPost(int userid, int postid)
        {
            if (ctx.GroupPostLikes.Any(l => l.UserId == userid && l.Post.Id == postid))
            {
                List<GroupPostLike> likes =
                    ctx.GroupPostLikes.Where(l => l.UserId == userid && l.GroupPostId == postid).ToList();
                ctx.GroupPostLikes.RemoveRange(likes);
            }
            else
            {


                GroupPostLike like = new GroupPostLike();

                like.Post = ctx.GroupPosts.First(p => p.Id == postid);
                like.GroupPostId = like.Post.Id;

                like.User = ctx.Users.First(u => u.Id == userid);
                like.UserId = like.User.Id;

                ctx.GroupPostLikes.Add(like);
            }

            ctx.SaveChanges();
        }
    }
}

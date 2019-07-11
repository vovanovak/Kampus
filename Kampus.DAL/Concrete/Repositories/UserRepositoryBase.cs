using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;
using Newtonsoft.Json;
using Kampus.DAL.Abstract.Repositories;
using Kampus.DAL.Enums;
using Kampus.DAL.Exceptions;
using Kampus.DAL.Security;
using Kampus.Persistence.Entities.UserRelated;
using Kampus.Persistence.Enums;

namespace Kampus.DAL.Concrete.Repositories
{
    internal class UserRepositoryBase : RepositoryBase<UserModel, User>, IUserRepository
    {
        public UserRepositoryBase(KampusContext context) : base(context)
        {
        }

        protected override System.Data.Entity.DbSet<User> GetTable()
        {
            return ctx.Users;
        }

        protected override System.Linq.Expressions.Expression<Func<User, UserModel>> GetConverter()
        {
            return u => new UserModel()
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Password = u.Password,
                DateOfBirth = u.DateOfBirth,
                FullName = u.Fullname,

                Rating = u.Rating,

                City = u.City.Name,
                Avatar = u.Avatar,
                UserRole = u.Role.Name,

                Status = u.Status,

                Achievements = u.Achievements.Select(a => a.Name).ToList(),

                UniversityName = (u.StudentDetails != null) ? u.StudentDetails.University.Name : "",
                UniversityFaculty = ((u.StudentDetails != null) ? u.StudentDetails.Faculty.Name : ""),
                UniversityCourse = u.StudentDetails.Course
            };
        }

        protected override void UpdateEntry(User dbEntity, UserModel entity)
        {
            
        }

        public UserModel GetByUsername(string username)
        {
            return ctx.Users.Where(u => u.Username == username).Select(GetConverter()).First();
        }

        public void RegisterUser(UserModel model)
        {
            Save(model);
        }

        public SignInResult SignIn(string username, string password)
        {
            if (ctx.Users.Any(u => u.Username == username && u.Password == password))
                return SignInResult.Successful;
            else if (ctx.Users.Any(u => u.Username == username))
                return SignInResult.WrongPassword;
            else
                return SignInResult.Error;
        }

        public List<UserShortModel> GetUserSubscribers(int userId)
        {
            User user = GetTable().First(u => u.Id == userId);

            if (user.Subscribers == null)
            {
                user.Subscribers = new List<User>();
                return new List<UserShortModel>();
            }

            return user.Subscribers.Select(u => new UserShortModel() { Id = u.Id, Username = u.Username, Avatar = u.Avatar }).ToList();
        }

        public List<UserShortModel> GetUserFriends(int userId)
        {
            User user = GetTable().First(u => u.Id == userId);
            if (user.Friends == null)
            {
                user.Friends = new List<User>();
                return new List<UserShortModel>();
            }
            return user.Friends.Select(u => new UserShortModel() { Id = u.Id, Username = u.Username, Avatar = u.Avatar }).ToList();
        }

        public void AddFriend(int id, int userId)
        {
            if (id == userId)
                throw new SameUserException();

            User u1 = GetTable().First(u => u.Id == id);
            User u2 = GetTable().First(u => u.Id == userId);

            u1.Friends.Add(u2);
            u2.Friends.Add(u1);

            u1.Subscribers.Remove(u2);
            u2.Subscribers.Remove(u1);

            Notification notification = Notification.From(DateTime.Now, NotificationType.Friendship,
                u1, u2, "/Home/Friends", "@" + u1.Username + " додав вас як друга");

            ctx.Notifications.Add(notification);

            ctx.SaveChanges();
        }

        public void AddSubscriber(UserModel receiver, UserModel sender)
        {
            if (receiver.Id == sender.Id)
                throw new SameUserException();

            User dbReceiver = GetTable().First(u => u.Id == receiver.Id);
            User dbSender = GetTable().First(s => s.Id == sender.Id);

            if (dbReceiver.Friends.Contains(dbSender))
                throw new SubscribeOnFriendException();

            if (!dbReceiver.Subscribers.Contains(dbSender))
                dbReceiver.Subscribers.Add(dbSender);
            else
            {
                dbReceiver.Subscribers.Remove(dbSender);
            }

            Notification notification = Notification.From(DateTime.Now, NotificationType.Subscribed,
                dbSender, dbReceiver, "/Home/Subscribers", "@" + dbSender.Username + " підписався на ваші оновлення");

            ctx.Notifications.Add(notification);
            ctx.SaveChanges();
        }

        public bool ContainsUserWithSuchUsername(string username)
        {
            return GetTable().Any(u => u.Username == username);
        }

        public void RemoveFriend(int id, int friendId)
        {
            User u1 = GetTable().First(u => u.Id == id);
            User u2 = GetTable().First(u => u.Id == friendId);

            u1.Friends.Remove(u2);
            u1.Subscribers.Add(u2);

            u2.Friends.Remove(u1);
            u2.Subscribers.Remove(u1);

            ctx.SaveChanges();
        }

        public void SendRecoveryLetter(UserRecovery recovery, string path)
        {

            var fromAddress = new MailAddress("cinemaserverrivne@gmail.com", "Kampus.com");
            var toAddress = new MailAddress(recovery.User.Email, "");
            string fromPassword = "cinemaserverrivne1";
            string subject = "Відновлення пароля";
            string body = "Відновіть пароль перейшовши за цим посиланням: " + path + recovery.HashString;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

        }

        public void RecoverPassword(string username, string email, string path)
        {
            User user = GetTable().First(u => u.Username == username);

            if (user.Email == email)
            {
                UserRecovery recovery = new UserRecovery();
                recovery.User = user;
                recovery.UserId = user.Id;
                recovery.HashString = (DateTime.Now.Ticks.ToString() + username + email + path).GetEncodedHash();

                SendRecoveryLetter(recovery, path);

                ctx.Recoveries.Add(recovery);

                ctx.SaveChanges();
            }
        }

        public string ContainsRecoveryWithSuchHash(string str)
        {
            return ctx.Recoveries.FirstOrDefault(r => r.HashString == str)?.User.Username;
        }

        public void SetNewPassword(string username, string password)
        {
            User user = GetTable().First(u => u.Username == username);
            user.Password = password;
            ctx.SaveChanges();
        }

        public void SetAvatar(int userId, string path)
        {
            User user = GetTable().First(u => u.Id == userId);
            user.Avatar = path;
            ctx.SaveChanges();
        }

        public void ChangePassword(int userId, string oldPassword, string newPassword, string newPasswordConfirm)
        {
            User user = GetTable().First(u => u.Id == userId);
            if (user.Password == oldPassword)
            {
                if (newPassword == newPasswordConfirm)
                {
                    user.Password = newPasswordConfirm;
                    ctx.SaveChanges();
                }
            }
        }

        public void ChangeStatus(int userId, string status)
        {
            User user = GetTable().First(u => u.Id == userId);
            user.Status = status;
            ctx.SaveChanges();
        }

        public void ChangeStudentInfo(int userId, string university, string faculty, int course)
        {
            User user = GetTable().First(u => u.Id == userId);
            if (user.StudentDetails == null)
                user.StudentDetails = new StudentDetails();

            user.StudentDetails.University = ctx.Universities.First(u => u.Name == university);
            user.StudentDetails.Faculty = ctx.Faculties.First(f => f.Name == faculty);
            user.StudentDetails.Course = course;

            ctx.SaveChanges();
        }

        public void ChangeCity(int userId, string city)
        {
            User user = GetTable().First(u => u.Id == userId);
            user.City = ctx.Cities.First(c => c.Name == city);
            ctx.SaveChanges();
        }

        public int CalculateAge(DateTime bday)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - bday.Year;
            if (bday > today.AddYears(-age)) age--;
            return age;
        }

        

        public bool ContainsUserWithSuchEmail(string email)
        {
            return ctx.Users.Any(u => u.Email == email);
        }
    }
}

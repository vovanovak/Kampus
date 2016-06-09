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

namespace Kampus.DAL.Concrete
{
    public class UserRepositoryBase: RepositoryBase<UserModel, User>, IUserRepository
    {
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

                UniversityName = ((u.StudentDetails != null) ? u.StudentDetails.University.Name : ""),
                UniversityFaculty = ((u.StudentDetails != null) ? u.StudentDetails.Faculty.Name : ""),
                UniversityCourse = u.StudentDetails.Course
            };
        }

        protected override void UpdateEntry(User dbEntity, UserModel entity)
        {
            dbEntity.Username = entity.Username;
            dbEntity.Password = entity.Password;
            dbEntity.Email = entity.Email;
            dbEntity.Status = entity.Status;
            dbEntity.Avatar = entity.Avatar;
            dbEntity.Fullname = entity.FullName;
            dbEntity.Rating = 0;
            dbEntity.DateOfBirth = (entity.DateOfBirth >= (DateTime)SqlDateTime.MinValue) ? entity.DateOfBirth : (DateTime)DateTime.MinValue;
            dbEntity.NotificationsLastChecked = DateTime.Now;
            dbEntity.City = ctx.Cities.First(c => c.Name == entity.City);
            dbEntity.Role = ctx.UserRoles.First(r => r.Name == "User");

            if (entity.IsNotStudent)
                dbEntity.StudentDetails = null;
            else
            {
                if (dbEntity.StudentDetails == null)
                    dbEntity.StudentDetails = new StudentDetails();

                dbEntity.StudentDetails.Course = entity.UniversityCourse.Value;
                
                if (dbEntity.StudentDetails.University == null)
                    dbEntity.StudentDetails.University = new University();
                dbEntity.StudentDetails.University = ctx.Universities.First(u => u.Name == entity.UniversityName);


                if (dbEntity.StudentDetails.Faculty == null)
                    dbEntity.StudentDetails.Faculty = new UniversityFaculty();
                dbEntity.StudentDetails.Faculty = ctx.Faculties.First(u => u.Name == entity.UniversityFaculty && u.UniversityId == dbEntity.StudentDetails.University.Id);
            }
        }

        public UserModel GetByUsername(string username)
        {
            return ctx.Users.Include("Users").Where(u => u.Username == username).Select(GetConverter()).First();
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

        public List<UserShortModel> GetUserSubscribers(int userid)
        {
            User user = ctx.Users.First(u => u.Id == userid);

            if (user.Subscribers == null)
            {
                user.Subscribers = new List<User>();
                return new List<UserShortModel>();
            }

            return user.Subscribers.Select(u => new UserShortModel() {Id = u.Id, Username = u.Username, Avatar = u.Avatar}).ToList();
        }

        public List<UserShortModel> GetUserFriends(int userid)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            if (user.Friends == null)
            {
                user.Friends = new List<User>();
                return new List<UserShortModel>();
            }
            return user.Friends.Select(u => new UserShortModel() { Id = u.Id, Username = u.Username, Avatar = u.Avatar }).ToList();
        }

        public void AddFriend(int id, int userid)
        {
            if (id == userid)
                throw new SameUserException();
            
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
            notification.SenderId = u1.Id;
            notification.Sender = u1;
            notification.Link = "/Home/Friends";
            notification.Message = "@" + u1.Username + " додав вас як друга";

            ctx.Notifications.Add(notification);

            ctx.SaveChanges();
            
           
               
        }

        public void AddSubscriber(UserModel user, UserModel sender)
        {
            if (user.Id == sender.Id)
                throw new SameUserException();


            User u1 = ctx.Users.First(u => u.Id == user.Id);
            User s1 = ctx.Users.First(s => s.Id == sender.Id);



            if (u1.Friends.Contains(s1))
                throw new SubscribeOnFriendException();

            if (!u1.Subscribers.Contains(s1))
                u1.Subscribers.Add(s1);
            else
            {
                u1.Subscribers.Remove(s1);
            }

            Notification notification = new Notification();
            notification.Date = DateTime.Now;
            notification.Type = NotificationType.Subscribed;
            notification.User = u1;
            notification.UserId = u1.Id;
            notification.SenderId = s1.Id;
            notification.Sender = s1;
            notification.Link = "/Home/Subscribers";
            notification.Message = "@" + s1.Username + " підписався на ваші оновлення";

            ctx.Notifications.Add(notification);


            ctx.SaveChanges();
        }

        public bool ContainsUserWithSuchUsername(string username)
        {
            return ctx.Users.Any(u => u.Username == username);
        }

        public void RemoveFriend(int id, int friendid)
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

        public string GetEncodedHash(string path)
        {
            const string salt = "adhasdhasdhas";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(path + salt));
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            base64digest = base64digest.Replace("\\", "a");
            base64digest = base64digest.Replace("/", "a");
            base64digest = base64digest.Replace("+", "b");
            return base64digest.Substring(0, base64digest.Length - 2);
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
            User user = ctx.Users.First(u => u.Username == username);

            if (user.Email == email)
            {
                UserRecovery recovery = new UserRecovery();
                recovery.User = user;
                recovery.UserId = user.Id;
                recovery.HashString = GetEncodedHash(DateTime.Now.Ticks.ToString() + username + email + path);

                SendRecoveryLetter(recovery, path);

                ctx.Recoveries.Add(recovery);

                ctx.SaveChanges();
            }
            else
            {
                
            }

            
        }

        public string ContainsRecoveryWithSuchHash(string str)
        {
            if (ctx.Recoveries.Any(r => r.HashString == str))
            {
                return ctx.Recoveries.First(r => r.HashString == str).User.Username;
            }
            else
            {
                return null;
            }
        }

        public void SetNewPassword(string username, string password)
        {
            User user = ctx.Users.First(u => u.Username == username);
            user.Password = password;
            ctx.SaveChanges();
        }

        public void SetAvatar(int userid, string path)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            user.Avatar = path;
            ctx.SaveChanges();
        }

        public void ChangePassword(int userid, string oldpassword, string newpassword, string newpasswordconfirm)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            if (user.Password == oldpassword)
            {
                if (newpassword == newpasswordconfirm)
                {
                    user.Password = newpasswordconfirm;
                    ctx.SaveChanges();
                }
            }
        }

        public void ChangeStatus(int userid, string status)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            user.Status = status;
            ctx.SaveChanges();
        }

        public void ChangeStudentInfo(int userid, string university, string faculty, int course)
        {
            User user = ctx.Users.First(u => u.Id == userid);
            if (user.StudentDetails == null)
                user.StudentDetails = new StudentDetails();

            user.StudentDetails.University = ctx.Universities.First(u => u.Name == university);
            user.StudentDetails.Faculty = ctx.Faculties.First(f => f.Name == faculty);
            user.StudentDetails.Course = course;

            ctx.SaveChanges();
        }

        public void ChangeCity(int userid, string city)
        {
            User user = ctx.Users.First(u => u.Id == userid);
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

        public UserSearchModel UpdateUserSearch(string request, string university, string faculty,
            string city, int? course, int? minage, int? maxage, int? minrating, int? maxrating)
        {
            UserSearchModel model = new UserSearchModel
            {
                Request = request,
                University = university,
                Faculty = faculty,
                City = city,
                Course = course,
                MinAge = minage,
                MaxAge = maxage,
                MinRating = minrating,
                MaxRating = maxrating
            };

            return model;
        }

        public List<UserModel> SearchUsers(string request, string university, string faculty,
            string city, int? course, int? minage, int? maxage, int? minrating, int? maxrating)
        {
            List<User> users = ctx.Users.Select(p => p).ToList();

            if (!string.IsNullOrEmpty(request))
            {
                request = request.ToLower();

                users.RemoveAll(
                    u =>
                        (u.Status == null || !u.Status.ToLower().Contains(request)) && 
                        !u.Username.ToLower().Contains(request) &&
                        !u.Fullname.ToLower().Contains(request));

            }

            if (!string.IsNullOrEmpty(university))
            {
                users.RemoveAll(
                    u => ((u.StudentDetails == null) || !u.StudentDetails.University.Name.Contains(university)));
            }

            if (!string.IsNullOrEmpty(faculty))
            {
                users.RemoveAll(u => ((u.StudentDetails == null) || !u.StudentDetails.Faculty.Name.Contains(faculty)));
            }

            if (!string.IsNullOrEmpty(city))
            {
                users.RemoveAll(u => !u.City.Name.Contains(city));
            }
            if (course != null)
            {
                users.RemoveAll(u => ((u.StudentDetails == null) || u.StudentDetails.Course != course.Value));
            }
            if (minage != null && maxage != null && minage < maxage)
            {
                users.RemoveAll(u => CalculateAge(u.DateOfBirth) < minage || CalculateAge(u.DateOfBirth) > maxage);
            }
            else
            {
                if (minage == null && maxage != null)
                {
                    users.RemoveAll(u => CalculateAge(u.DateOfBirth) > maxage);
                }

                if (maxage == null && minage != null)
                {
                    users.RemoveAll(u => CalculateAge(u.DateOfBirth) < minage);
                }
            }
            if (minrating != null && maxrating != null && minrating < maxrating)
            {
                users.RemoveAll(u => u.Rating < minrating || u.Rating > maxrating);
            }
            else
            {
                if (minrating == null && maxrating != null)
                {
                    users.RemoveAll(u => u.Rating > maxrating);
                }

                if (maxrating == null && minrating != null)
                {
                    users.RemoveAll(u => u.Rating < minrating);
                }
            }

            return users.Any()
                ? users.Select(u => new UserModel()
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Password = u.Password,
                    DateOfBirth = u.DateOfBirth,
                    FullName = u.Fullname,

                    City = u.City.Name,
                    Avatar = u.Avatar,
                    UserRole = u.Role.Name,

                    Status = u.Status,
                    UniversityName = ((u.StudentDetails != null) ? u.StudentDetails.University.Name : ""),
                    UniversityFaculty = ((u.StudentDetails != null) ? u.StudentDetails.Faculty.Name : ""),
                    UniversityCourse = u.StudentDetails.Course
                }).ToList()
                : new List<UserModel>();

        }
    }

}

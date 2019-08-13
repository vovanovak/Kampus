using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.UniversityRelated;
using Kampus.Persistence.Entities.UserRelated;
using Kampus.Persistence.Enums;

namespace Kampus.Application.Services.Users.Impl
{
    internal class UserService : IUserService
    {
        private readonly KampusContext _context;
        private readonly IUserMapper _userMapper;

        public UserService(KampusContext ctx, IUserMapper userMapper)
        {
            _context = ctx;
            _userMapper = userMapper;
        }

        public List<UserModel> GetAll()
        {
            return _context.Users.Select(_userMapper.Map).ToList();
        }

        public UserModel GetById(int userId)
        {
            return _userMapper.Map(_context.Users.Single(u => u.UserId == userId));
        }

        public UserModel GetByUsername(string username)
        {
            return _context.Users.Where(u => u.Username == username).Select(_userMapper.Map).First();
        }

        public void RegisterUser(UserModel model)
        {
            var dbEntity = new User();

            dbEntity.Username = model.Username;
            dbEntity.Password = model.Password;
            dbEntity.Email = model.Email;
            dbEntity.Status = model.Status;
            dbEntity.Avatar = model.Avatar;
            dbEntity.Fullname = model.FullName;
            dbEntity.Rating = 0;
            dbEntity.DateOfBirth = model.DateOfBirth >= (DateTime)SqlDateTime.MinValue ? model.DateOfBirth : DateTime.MinValue;
            dbEntity.NotificationsLastChecked = DateTime.Now;
            dbEntity.City = _context.Cities.First(c => c.Name == model.City);
            dbEntity.Role = _context.UserRoles.First(r => r.Name == "User");

            if (model.IsNotStudent)
                dbEntity.StudentDetails = null;
            else
            {
                if (dbEntity.StudentDetails == null)
                    dbEntity.StudentDetails = new StudentDetails();

                dbEntity.StudentDetails.Course = model.UniversityCourse.Value;

                if (dbEntity.StudentDetails.University == null)
                    dbEntity.StudentDetails.University = new University();
                dbEntity.StudentDetails.University = _context.Universities.First(u => u.Name == model.UniversityName);


                if (dbEntity.StudentDetails.Faculty == null)
                    dbEntity.StudentDetails.Faculty = new Faculty();
                dbEntity.StudentDetails.Faculty = _context.Faculties.First(u => u.Name == model.UniversityFaculty && u.UniversityId == dbEntity.StudentDetails.University.UniversityId);
            }
        }

        public SignInResult SignIn(string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username && u.Password == password))
                return SignInResult.Successful;
            else if (_context.Users.Any(u => u.Username == username))
                return SignInResult.WrongPassword;
            else
                return SignInResult.Error;
        }

        public bool ContainsUserWithSuchUsername(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        public bool ContainsUserWithSuchEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public void SetNewPassword(string username, string password)
        {
            User user = _context.Users.First(u => u.Username == username);
            user.Password = password;
            _context.SaveChanges();
        }

        public void SetAvatar(int userId, string path)
        {
            User user = _context.Users.First(u => u.UserId == userId);
            user.Avatar = path;
            _context.SaveChanges();
        }

        public void ChangePassword(int userId, string oldPassword, string newPassword, string newPasswordConfirm)
        {
            User user = _context.Users.First(u => u.UserId == userId);
            if (user.Password == oldPassword)
            {
                if (newPassword == newPasswordConfirm)
                {
                    user.Password = newPasswordConfirm;
                    _context.SaveChanges();
                }
            }
        }

        public void ChangeStatus(int userId, string status)
        {
            User user = _context.Users.First(u => u.UserId == userId);
            user.Status = status;
            _context.SaveChanges();
        }

        public void ChangeStudentInfo(int userId, string university, string faculty, int course)
        {
            User user = _context.Users.First(u => u.UserId == userId);
            if (user.StudentDetails == null)
                user.StudentDetails = new StudentDetails();

            user.StudentDetails.University = _context.Universities.First(u => u.Name == university);
            user.StudentDetails.Faculty = _context.Faculties.First(f => f.Name == faculty);
            user.StudentDetails.Course = course;

            _context.SaveChanges();
        }

        public void ChangeCity(int userId, string city)
        {
            User user = _context.Users.First(u => u.UserId == userId);
            user.City = _context.Cities.First(c => c.Name == city);
            _context.SaveChanges();
        }
    }
}

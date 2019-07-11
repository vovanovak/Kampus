using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kampus.Models;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class UserMapper : IUserMapper
    {
        public UserModel Map(User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                DateOfBirth = user.DateOfBirth,
                FullName = user.Fullname,

                Rating = user.Rating,

                City = user.City.Name,
                Avatar = user.Avatar,
                UserRole = user.Role.Name,

                Status = user.Status,

                Achievements = user.Achievements.Select(a => a.Name).ToList(),

                UniversityName = (user.StudentDetails != null) ? user.StudentDetails.University.Name : "",
                UniversityFaculty = ((user.StudentDetails != null) ? user.StudentDetails.Faculty.Name : ""),
                UniversityCourse = user.StudentDetails.Course
            };
        }
    }
}

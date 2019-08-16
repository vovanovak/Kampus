using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.UserRelated;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kampus.Application.Services.Users.Impl
{
    internal class UserSearchService : IUserSearchService
    {
        private readonly KampusContext _context;

        public UserSearchService(KampusContext context)
        {
            _context = context;
        }

        // TODO: Refactor method
        public List<UserModel> SearchUsers(string request, string university, string faculty, string city, int? course, int? minAge, int? maxAge, int? minRating, int? maxRating)
        {
            List<User> users = _context.Users.Select(p => p).ToList();

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
                    u => u.StudentDetails == null || !u.StudentDetails.University.Name.Contains(university));
            }

            if (!string.IsNullOrEmpty(faculty))
            {
                users.RemoveAll(u => u.StudentDetails == null || !u.StudentDetails.Faculty.Name.Contains(faculty));
            }

            if (!string.IsNullOrEmpty(city))
            {
                users.RemoveAll(u => !u.City.Name.Contains(city));
            }
            if (course != null)
            {
                users.RemoveAll(u => u.StudentDetails == null || u.StudentDetails.Course != course.Value);
            }
            if (minAge != null && maxAge != null && minAge < maxAge)
            {
                users.RemoveAll(u => u.CalculateAge() < minAge || u.CalculateAge() > maxAge);
            }
            else
            {
                if (minAge == null && maxAge != null)
                {
                    users.RemoveAll(u => u.CalculateAge() > maxAge);
                }

                if (maxAge == null && minAge != null)
                {
                    users.RemoveAll(u => u.CalculateAge() < minAge);
                }
            }
            if (minRating != null && maxRating != null && minRating < maxRating)
            {
                users.RemoveAll(u => u.Rating < minRating || u.Rating > maxRating);
            }
            else
            {
                if (minRating == null && maxRating != null)
                {
                    users.RemoveAll(u => u.Rating > maxRating);
                }

                if (maxRating == null && minRating != null)
                {
                    users.RemoveAll(u => u.Rating < minRating);
                }
            }

            return users.Any()
                ? users.Select(u => new UserModel()
                {
                    Id = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Password = u.Password,
                    DateOfBirth = u.DateOfBirth,
                    FullName = u.Fullname,

                    City = u.City.Name,
                    Avatar = u.Avatar,
                    UserRole = u.Role.Name,

                    Status = u.Status,
                    UniversityName = u.StudentDetails != null ? u.StudentDetails.University.Name : "",
                    UniversityFaculty = u.StudentDetails != null ? u.StudentDetails.Faculty.Name : "",
                    UniversityCourse = u.StudentDetails.Course
                }).ToList() : new List<UserModel>();
        }

        public UserSearchModel UpdateUserSearch(string request, string university, string faculty, string city, int? course, int? minAge, int? maxAge, int? minRating, int? maxRating)
        {
            UserSearchModel model = new UserSearchModel
            {
                Request = request,
                University = university,
                Faculty = faculty,
                City = city,
                Course = course,
                MinAge = minAge,
                MaxAge = maxAge,
                MinRating = minRating,
                MaxRating = maxRating
            };

            return model;
        }
    }
}

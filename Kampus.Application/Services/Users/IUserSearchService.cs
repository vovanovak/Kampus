using Kampus.Models;
using System.Collections.Generic;

namespace Kampus.Application.Services.Users
{
    public interface IUserSearchService
    {
        UserSearchModel UpdateUserSearch(string request, string university, string faculty,
            string city, int? course, int? minAge, int? maxAge, int? minRating, int? maxRating);
        List<UserModel> SearchUsers(string request, string university, string faculty,
            string city, int? course, int? minAge, int? maxAge, int? minRating, int? maxRating);
    }
}

using Kampus.Models;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Extensions
{
    public static class UserExtensions
    {
        public static UserShortModel MapToUserShortModel(this User user)
        {
            return user == null ? null : new UserShortModel(user.UserId, user.Username, user.Avatar);
        }
    }
}
using Kampus.Models;
using System.Collections.Generic;

namespace Kampus.Application.Services.Users
{
    public interface IUserConnectionsService
    {
        List<UserShortModel> GetUserSubscribers(int userId);
        List<UserShortModel> GetUserFriends(int userId);
        void AddFriend(int id, int userId);
        void AddSubscriber(UserModel user, UserModel sender);
        void RemoveFriend(int id, int friendId);
    }
}

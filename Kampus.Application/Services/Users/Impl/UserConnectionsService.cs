using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Application.Exceptions;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.NotificationRelated;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Services.Users.Impl
{
    internal class UserConnectionsService : IUserConnectionsService
    {
        private readonly KampusContext _context;

        public UserConnectionsService(KampusContext context)
        {
            _context = context;
        }

        public void AddFriend(int id, int userId)
        {
            if (id == userId)
                throw new SameUserException();

            User u1 = _context.Users.First(u => u.Id == id);
            User u2 = _context.Users.First(u => u.Id == userId);

            u1.Friends.Add(u2);
            u2.Friends.Add(u1);

            u1.Subscribers.Remove(u2);
            u2.Subscribers.Remove(u1);

            Notification notification = Notification.From(DateTime.Now, NotificationType.Friendship,
                u1, u2, "/Home/Friends", "@" + u1.Username + " додав вас як друга");

            _context.Notifications.Add(notification);

            _context.SaveChanges();
        }

        public void AddSubscriber(UserModel receiver, UserModel sender)
        {
            if (receiver.Id == sender.Id)
                throw new SameUserException();

            User dbReceiver = _context.Users.First(u => u.Id == receiver.Id);
            User dbSender = _context.Users.First(s => s.Id == sender.Id);

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

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public List<UserShortModel> GetUserFriends(int userId)
        {
            User user = _context.Users.First(u => u.Id == userId);
            if (user.Friends == null)
            {
                user.Friends = new List<User>();
                return new List<UserShortModel>();
            }
            return user.Friends.Select(u => new UserShortModel() { Id = u.Id, Username = u.Username, Avatar = u.Avatar }).ToList();
        }

        public List<UserShortModel> GetUserSubscribers(int userId)
        {
            User user = _context.Users.First(u => u.Id == userId);

            if (user.Subscribers == null)
            {
                user.Subscribers = new List<User>();
                return new List<UserShortModel>();
            }

            return user.Subscribers.Select(u => new UserShortModel() { Id = u.Id, Username = u.Username, Avatar = u.Avatar }).ToList();
        }

        public void RemoveFriend(int id, int friendId)
        {
            User u1 = _context.Users.First(u => u.Id == id);
            User u2 = _context.Users.First(u => u.Id == friendId);

            u1.Friends.Remove(u2);
            u1.Subscribers.Add(u2);

            u2.Friends.Remove(u1);
            u2.Subscribers.Remove(u1);

            _context.SaveChanges();
        }
    }
}

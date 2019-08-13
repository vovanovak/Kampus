using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Application.Exceptions;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.NotificationRelated;
using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;

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

            var u1 = _context.Users.First(u => u.UserId == id);
            var u2 = _context.Users.First(u => u.UserId == userId);

            _context.Friends.Add(new Friend { User1Id = id, User2Id = userId });
            _context.Friends.Add(new Friend { User1Id = userId, User2Id = id });

            var subscribers = _context.Subscribers.Where(s => s.User1Id == id && s.User2Id == userId ||
                                                              s.User1Id == userId && s.User2Id == id).ToList();

            _context.Subscribers.RemoveRange(subscribers);

            var notification = Notification.From(DateTime.Now, NotificationType.Friendship,
                u1, u2, "/Home/Friends", "@" + u1.Username + " додав вас як друга");

            _context.Notifications.Add(notification);

            _context.SaveChanges();
        }

        public void AddSubscriber(UserModel receiver, UserModel sender)
        {
            if (receiver.Id == sender.Id)
                throw new SameUserException();

            var dbReceiver = _context.Users.First(u => u.UserId == receiver.Id);
            var dbSender = _context.Users.First(s => s.UserId == sender.Id);

            if (_context.Friends.Any(f => f.User1Id == sender.Id && f.User2Id == receiver.Id))
                throw new SubscribeOnFriendException();

            var subscribers = _context.Subscribers
                .Where(s => s.User1Id == sender.Id && s.User2Id == receiver.Id ||
                            s.User1Id == receiver.Id && s.User2Id == sender.Id)
                .ToList();

            if (subscribers.Count > 0)
            {
                _context.Subscribers.RemoveRange(subscribers);
            }
            else
            {
                _context.Subscribers.Add(new Subscriber {User1Id = sender.Id, User2Id = receiver.Id});
                _context.Subscribers.Add(new Subscriber {User1Id = receiver.Id, User2Id = sender.Id});
            }

            Notification notification = Notification.From(DateTime.Now, NotificationType.Subscribed,
                dbSender, dbReceiver, "/Home/Subscribers", "@" + dbSender.Username + " підписався на ваші оновлення");

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public List<UserShortModel> GetUserFriends(int userId)
        {
            return _context.Friends
                .Include(s => s.User1)
                .Include(s => s.User2)
                .Where(s => s.User1Id == userId)
                .Select(u => new UserShortModel(u.User2.UserId, u.User2.Username, u.User2.Avatar))
                .ToList();
        }

        public List<UserShortModel> GetUserSubscribers(int userId)
        {
            return _context.Subscribers
                .Include(s => s.User1)
                .Include(s => s.User2)
                .Where(s => s.User1Id == userId)
                .Select(u => new UserShortModel(u.User2.UserId, u.User2.Username, u.User2.Avatar))
                .ToList();
        }

        public void RemoveFriend(int id, int friendId)
        {
            var friendsToRemove = _context.Friends.Where(f => f.User1Id == id && f.User2Id == friendId ||
                                                              f.User1Id == friendId && f.User2Id == id)
                .ToList();

            var subscribersToRemove = _context.Subscribers.Where(s => s.User1Id == id && s.User2Id == friendId ||
                                                                      s.User1Id == friendId && s.User2Id == id)
                .ToList();

            _context.Friends.RemoveRange(friendsToRemove);
            _context.Subscribers.RemoveRange(subscribersToRemove);

            _context.SaveChanges();
        }
    }
}

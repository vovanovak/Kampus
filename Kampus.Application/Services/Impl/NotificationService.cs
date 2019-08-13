using System;
using System.Collections.Generic;
using System.Linq;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.NotificationRelated;
using Kampus.Persistence.Entities.UserRelated;

namespace Kampus.Application.Services.Impl
{
    internal class NotificationService : INotificationService
    {
        private readonly KampusContext _context;
        private readonly INotificationMapper _notificationMapper;

        public NotificationService(KampusContext context, INotificationMapper notificationMapper)
        {
            _context = context;
            _notificationMapper = notificationMapper;
        }

        public IReadOnlyList<NotificationModel> GetNewNotifications(int userId)
        {
            var user = _context.Users.First(u => u.UserId == userId);

            var notifications = _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .Select(_notificationMapper.Map)
                .ToList();

            var sec = TimeSpan.TicksPerSecond * 2;

            notifications.RemoveAll(n => n.SeenDate != null && DateTime.Now.Ticks - n.SeenDate.Value.Ticks >= sec);

            user.NotificationsLastChecked = DateTime.Now;

            return notifications;
        }

        public void SetNotificationSeen(int notificationId)
        {
            var notification = _context.Notifications.Single(n => n.NotificationId == notificationId);
            notification.Seen = true;
            notification.SeenDate = DateTime.Now;
            _context.SaveChanges();
        }

        public void ViewUnseenNotifications(int userId)
        {
            var notifications = _context.Notifications
                .Where(n => n.ReceiverId == userId &&
                            n.Seen == false &&
                            n.SeenDate == null)
                .ToList();

            foreach (var n in notifications)
            {
                n.Seen = true;
                n.SeenDate = DateTime.Now;
            }

            _context.SaveChanges();
        }
    }
}

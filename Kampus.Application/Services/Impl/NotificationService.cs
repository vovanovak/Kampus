using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kampus.Application.Mappers;
using Kampus.Models;
using Kampus.Persistence.Contexts;
using Kampus.Persistence.Entities.NotificationRelated;
using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IReadOnlyList<NotificationModel>> GetNewNotifications(int userId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);

            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .Select(n => _notificationMapper.Map(n))
                .ToListAsync();

            const long sec = TimeSpan.TicksPerSecond * 2;

            notifications.RemoveAll(n => n.SeenDate != null && DateTime.Now.Ticks - n.SeenDate.Value.Ticks >= sec);

            user.NotificationsLastChecked = DateTime.Now;

            return notifications;
        }

        public async Task SetNotificationSeen(int notificationId)
        {
            var notification = await _context.Notifications.SingleAsync(n => n.NotificationId == notificationId);

            notification.Seen = true;
            notification.SeenDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task ViewUnseenNotifications(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.ReceiverId == userId &&
                            n.Seen == false &&
                            n.SeenDate == null)
                .ToListAsync();

            foreach (var n in notifications)
            {
                n.Seen = true;
                n.SeenDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}

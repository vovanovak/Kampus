using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;

namespace Kampus.DAL.Concrete
{
    public class NotificationRepositoryBase: RepositoryBase<NotificationModel, Notification>, INotificationRepository
    {
        protected override DbSet<Notification> GetTable()
        {
            return ctx.Notifications;
        }

        protected override Expression<Func<Notification, NotificationModel>> GetConverter()
        {
            return n => new NotificationModel()
            {
                Id = n.Id,
                Date = n.Date,
                Link = n.Link,
                Message = n.Message,
                Type = n.Type,
                Seen = n.Seen,
                SeenDate = n.SeenDate,
                User = new UserShortModel()
                {
                    Id = n.User.Id,
                    Username = n.User.Username,
                    Avatar = n.User.Avatar
                },
                Sender = new UserShortModel()
                {
                    Id = n.Sender.Id,
                    Username = n.Sender.Username,
                    Avatar = n.Sender.Avatar
                }
            };
        }

        protected override void UpdateEntry(Notification dbEntity, NotificationModel entity)
        {
            dbEntity.Id = entity.Id;
            dbEntity.Date = entity.Date;
            dbEntity.Type = entity.Type;
            dbEntity.Link = entity.Link;
            dbEntity.Message = entity.Message;
            dbEntity.Seen = entity.Seen;
            dbEntity.User = ctx.Users.First(u => u.Id == entity.User.Id);
            dbEntity.Sender = ctx.Users.First(u => u.Id == entity.Sender.Id);
        }

        public List<NotificationModel> GetNewNotifications(int userid)
        {
            User user = ctx.Users.First(u => u.Id == userid);

            int count = ctx.Notifications.Count();
            long ticks = TimeSpan.TicksPerSecond * 5;

            List<NotificationModel> notifications =
                ctx.Notifications.Where(n => n.UserId == userid)
                    .Select(GetConverter())
                    .ToList();

            long sec = TimeSpan.TicksPerSecond * 2;

            
            notifications.RemoveAll(n => ((n.SeenDate != null)
                ? DateTime.Now.Ticks - n.SeenDate.Value.Ticks >= sec
                : false));

            user.NotificationsLastChecked = DateTime.Now;
            return notifications;
        }

        public void SetNotificationSeen(int notificationid)
        {
            Notification notification = ctx.Notifications.First(n => n.Id == notificationid);
            notification.Seen = true;
            notification.SeenDate = DateTime.Now;
            ctx.SaveChanges();
        }

        public void ViewUnseenNotifications(int userid)
        {
            List<Notification> notifications = ctx.Notifications.Where(n => n.UserId == userid &&
                n.Seen == false && n.SeenDate == null).ToList();

            foreach (var n in notifications)
            {
                n.Seen = true;
                n.SeenDate = DateTime.Now;
            }

            ctx.SaveChanges();
        }
    }
}
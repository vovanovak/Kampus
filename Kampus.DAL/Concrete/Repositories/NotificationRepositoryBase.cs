using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Kampus.DAL.Abstract;
using Kampus.Entities;
using Kampus.Models;
using Kampus.DAL.Abstract.Repositories;

namespace Kampus.DAL.Concrete.Repositories
{
    internal class NotificationRepositoryBase: RepositoryBase<NotificationModel, Notification>, INotificationRepository
    {
        public NotificationRepositoryBase(KampusContext context) : base(context)
        {
        }

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
                Receiver =  new UserShortModel() { Id = n.Receiver.Id, Username = n.Receiver.Username, Avatar = n.Receiver.Avatar },
                Sender = new UserShortModel() { Id = n.Sender.Id, Username = n.Sender.Username, Avatar = n.Sender.Avatar }
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
            dbEntity.Receiver = ctx.Users.First(u => u.Id == entity.Receiver.Id);
            dbEntity.Sender = ctx.Users.First(u => u.Id == entity.Sender.Id);
        }

        public List<NotificationModel> GetNewNotifications(int userId)
        {
            
        }

        public void SetNotificationSeen(int notificationId)
        {
            
        }

        public void ViewUnseenNotifications(int userId)
        {
        }

        
    }
}
using Kampus.Models;
using Kampus.Persistence.Entities.NotificationRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class NotificationMapper : INotificationMapper
    {
        public NotificationModel Map(Notification notification)
        {
            return new NotificationModel
            {
                Id = notification.NotificationId,
                Date = notification.Date,
                Link = notification.Link,
                Message = notification.Message,
                Type = (int)notification.Type,
                Seen = notification.Seen,
                SeenDate = notification.SeenDate,
                Receiver = new UserShortModel(notification.Receiver.UserId, notification.Receiver.Username, notification.Receiver.Avatar),
                Sender = new UserShortModel(notification.Sender.UserId, notification.Sender.Username, notification.Sender.Avatar)
            };
        }
    }
}

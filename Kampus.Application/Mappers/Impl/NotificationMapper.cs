using Kampus.Models;
using Kampus.Persistence.Entities.NotificationRelated;

namespace Kampus.Application.Mappers.Impl
{
    internal class NotificationMapper : INotificationMapper
    {
        public NotificationModel Map(Notification notification)
        {
            return new NotificationModel()
            {
                Id = notification.Id,
                Date = notification.Date,
                Link = notification.Link,
                Message = notification.Message,
                Type = (int)notification.Type,
                Seen = notification.Seen,
                SeenDate = notification.SeenDate,
                Receiver = new UserShortModel() { Id = notification.Receiver.Id, Username = notification.Receiver.Username, Avatar = notification.Receiver.Avatar },
                Sender = new UserShortModel() { Id = notification.Sender.Id, Username = notification.Sender.Username, Avatar = notification.Sender.Avatar }
            };
        }
    }
}

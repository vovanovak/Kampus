using Kampus.Models;
using Kampus.Persistence.Entities.NotificationRelated;

namespace Kampus.Application.Mappers
{
    public interface INotificationMapper
    {
        NotificationModel Map(Notification notification);
    }
}

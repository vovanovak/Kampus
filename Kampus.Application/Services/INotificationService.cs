using Kampus.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kampus.Application.Services
{
    public interface INotificationService
    {
        Task<IReadOnlyList<NotificationModel>> GetNewNotifications(int userId);
        Task SetNotificationSeen(int notificationId);
        Task ViewUnseenNotifications(int userId);
    }
}

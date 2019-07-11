using Kampus.Models;
using System.Collections.Generic;

namespace Kampus.Application.Services
{
    public interface INotificationService
    {
        IReadOnlyList<NotificationModel> GetNewNotifications(int userId);
        void SetNotificationSeen(int notificationId);
        void ViewUnseenNotifications(int userId);
    }
}

using System.Collections.Generic;
using Kampus.Models;

namespace Kampus.DAL.Abstract.Repositories
{
    public interface INotificationRepository: IRepository<NotificationModel>
    {
        List<NotificationModel> GetNewNotifications(int userId);
        void SetNotificationSeen(int notificationId);
        void ViewUnseenNotifications(int userId);
    }
}
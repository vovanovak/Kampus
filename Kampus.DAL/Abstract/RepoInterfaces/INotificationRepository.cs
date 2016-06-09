using System.Collections.Generic;
using Kampus.Models;

namespace Kampus.DAL.Abstract
{
    public interface INotificationRepository: IRepository<NotificationModel>
    {
        List<NotificationModel> GetNewNotifications(int userid);
        void SetNotificationSeen(int notificationid);
        void ViewUnseenNotifications(int userid);
    }
}
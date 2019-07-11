using Kampus.Models;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Kampus.Controllers
{
    public class NotificationController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public NotificationController()
        {
            _unitOfWork = UnitOfWorkResolver.UnitOfWork;
        }

        [HttpGet]
        public string GetNewNotifications()
        {
            int userId = Convert.ToInt32(Session["CurrentUserId"]);
            NotificationModel[] notifications = _unitOfWork.Notifications.GetNewNotifications(userId).ToArray();
            return JsonConvert.SerializeObject(notifications);
        }

        [HttpPost]
        public void ViewNotifications()
        {
            int userId = Convert.ToInt32(Session["CurrentUserId"]);
            _unitOfWork.Notifications.ViewUnseenNotifications(userId);
        }

    }
}

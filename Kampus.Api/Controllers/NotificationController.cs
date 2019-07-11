using Kampus.Api.Constants;
using Kampus.Api.Extensions;
using Kampus.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kampus.Api.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public string GetNewNotifications()
        {
            var userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            var notifications = _notificationService.GetNewNotifications(userId);
            return JsonConvert.SerializeObject(notifications);
        }

        [HttpPost]
        public void ViewNotifications()
        {
            var userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            _notificationService.ViewUnseenNotifications(userId);
        }
    }
}

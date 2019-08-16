using System.Threading.Tasks;
using Kampus.Application.Services;
using Kampus.Host.Constants;
using Kampus.Host.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Kampus.Host.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNewNotifications()
        {
            var userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            var notifications = await _notificationService.GetNewNotifications(userId);
            return Json(notifications);
        }

        [HttpPost]
        public async Task ViewNotifications()
        {
            var userId = HttpContext.Session.Get<int>(SessionKeyConstants.CurrentUserId);
            await _notificationService.ViewUnseenNotifications(userId);
        }
    }
}

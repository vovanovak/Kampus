using System.Threading.Tasks;
using Kampus.Application.Services.Users;
using Kampus.Host.Constants;
using Kampus.Host.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Kampus.Host.Controllers
{
    public class MainController : Controller
    {
        private readonly IUserService _userService;

        public MainController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> SignIn()
        {
            return View("SignIn");
        }

        [HttpPost]
        public async Task<string> SignIn(string username, string password)
        {
            var res = await _userService.SignIn(username, password);

            if (res == Persistence.Enums.SignInResult.Successful)
            {
                var user = await _userService.GetByUsername(username);
                HttpContext.Session.Add(SessionKeyConstants.CurrentUser, user);
                HttpContext.Session.Add(SessionKeyConstants.CurrentUserId, user.Id);
            }

            return res.ToString();
        }
    }
}

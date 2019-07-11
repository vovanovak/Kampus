using Kampus.Api.Constants;
using Kampus.Api.Extensions;
using Kampus.Application.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Kampus.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/

        private readonly IUserService _userService;

        public MainController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult SignIn()
        {
            return View("SignIn");
        }

        [HttpPost]
        public string SignIn(string username, string password)
        {
            var res = _userService.SignIn(username, password);

            if (res == Persistence.Enums.SignInResult.Successful)
            {
                var user = _userService.GetByUsername(username);
                HttpContext.Session.Add(SessionKeyConstants.CurrentUser, user);
                HttpContext.Session.Add(SessionKeyConstants.CurrentUserId, user.Id);
            }

            return res.ToString();
        }
    }
}

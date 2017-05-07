using Kampus.DAL;
using Kampus.DAL.Abstract;
using Kampus.DAL.Enums;
using System.Web.Mvc;

namespace Kampus.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/

        private IUnitOfWork _unitOfWork;

        public MainController()
        {
            _unitOfWork = UnitOfWorkResolver.UnitOfWork;
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
            SignInResult res = _unitOfWork.Users.SignIn(username, password);

            if (res == SignInResult.Successful)
            {
                var user = _unitOfWork.Users.GetByUsername(username);
                Session.Add("CurrentUser", user);
                Session.Add("CurrentUserId", user.Id);
            }

            return res.ToString();
        }
    }
}

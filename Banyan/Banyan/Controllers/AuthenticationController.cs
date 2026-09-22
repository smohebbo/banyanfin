using Banyan.Entities.viewModel;
using Microsoft.AspNetCore.Mvc;

namespace Banyan.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: AuthenticationController
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Authenticate(loginViewModel _logininfo)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", _logininfo);
            }

            // TODO: Validate UserId and password against your database.
            //
            // Example:
            // if (_logininfo.UserId == "admin" &&
            //     _logininfo.password == "123456")
            // {
            //     return RedirectToAction("Index", "Home");
            // }

            // Temporary example
            if (_logininfo.UserId == "admin" &&
                _logininfo.Password == "123456")
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid User ID or password.");
            return View("Login", _logininfo);
        }
    

    }
}

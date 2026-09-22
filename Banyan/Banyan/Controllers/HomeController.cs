using Banyan.Entities.viewModel;
using Banyan.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Banyan.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var loginViewModel = new loginViewModel();
            return View(loginViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

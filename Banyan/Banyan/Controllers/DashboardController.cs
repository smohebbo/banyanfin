using Banyan.Entities.viewModel;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Banyan.Controllers;

public class DashboardController : Controller
{
    private const string UserSessionKey = "_userSession";

    public IActionResult Index()
    {
        var serializedSession = HttpContext.Session.GetString(UserSessionKey);
        if (string.IsNullOrWhiteSpace(serializedSession))
        {
            return RedirectToAction("Index", "Authentication");
        }

        try
        {
            var userSession = JsonSerializer.Deserialize<UserSession>(serializedSession);
            if (userSession is null || string.IsNullOrWhiteSpace(userSession.Token))
            {
                HttpContext.Session.Remove(UserSessionKey);
                return RedirectToAction("Index", "Authentication");
            }

            ViewData["UserId"] = userSession.UserId;
            return View();
        }
        catch (JsonException)
        {
            HttpContext.Session.Remove(UserSessionKey);
            return RedirectToAction("Index", "Authentication");
        }
    }
}

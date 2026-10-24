using Banyan.Entities.viewModel;
using Banyan.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace Banyan.Controllers;

public class AuthenticationController : Controller
{
    private const string UserSessionKey = "_userSession";
    private readonly IAuthenticationService _authenticationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IAuthenticationService authenticationService,
        IConfiguration configuration,
        ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View("~/Views/Home/Index.cshtml", new loginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Authenticate(
        loginViewModel loginInfo,
        CancellationToken cancellationToken)
    {
        HttpContext.Session.Remove(UserSessionKey);
        if (TempData.Count > 0)
        {
            TempData.Clear();
        }
        if (!ModelState.IsValid)
        {
            return View("~/Views/Home/Index.cshtml", loginInfo);
        }

        var secretKey = _configuration["SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            _logger.LogError("Authentication cannot run because SecretKey configuration is missing.");
            TempData["AlertType"] = "danger";
            TempData["AlertMessage"] = "Sign-in is temporarily unavailable. Please try again later.";
            return View("~/Views/Home/Index.cshtml", loginInfo);
        }

        loginInfo.SecretKey = secretKey;

        try
        {
            var userSession = await _authenticationService.Authenticate(loginInfo, cancellationToken);
            if (userSession is null)
            {
                TempData["AlertType"] = "danger";
                TempData["AlertMessage"] = "Invalid user ID or password.";
                return View("~/Views/Home/Index.cshtml", loginInfo);
            }

            HttpContext.Session.SetString(UserSessionKey, JsonSerializer.Serialize(userSession));
            TempData["AlertType"] = "success";
            TempData["AlertMessage"] = "Sign-in successful.";
            return RedirectToAction("Index", "Dashboard");
        }
        catch (SqlException exception)
        {
            _logger.LogError(exception, "A database error occurred while authenticating a user.");
            TempData["AlertType"] = "danger";
            TempData["AlertMessage"] = "Sign-in is temporarily unavailable. Please try again later.";
            return View("~/Views/Home/Index.cshtml", loginInfo);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred while authenticating a user.");
            TempData["AlertType"] = "danger";
            TempData["AlertMessage"] = "Sign-in is temporarily unavailable. Please try again later.";
            return View("~/Views/Home/Index.cshtml", loginInfo);
        }
    }
}

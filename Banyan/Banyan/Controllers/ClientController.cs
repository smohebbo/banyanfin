using System.Text.Json;
using Banyan.Entities.viewModel;
using Banyan.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Banyan.Controllers
{
    public class ClientController : Controller
    {
        private const string UserSessionKey = "_userSession";
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(
            IClientService clientService,
            ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, CancellationToken cancellationToken)
        {
            var serializedSession = HttpContext.Session.GetString(UserSessionKey);
            if (string.IsNullOrWhiteSpace(serializedSession))
            {
                return RedirectToAction("Index", "Authentication");
            }

            UserSession? userSession;
            try
            {
                userSession = JsonSerializer.Deserialize<UserSession>(serializedSession);
            }
            catch (JsonException)
            {
                HttpContext.Session.Remove(UserSessionKey);
                return RedirectToAction("Index", "Authentication");
            }

            if (userSession is null
                || string.IsNullOrWhiteSpace(userSession.Token)
                || string.IsNullOrWhiteSpace(userSession.SecretKey)
                || userSession.SecretKey.Length > 100
                || userSession.ID <= 0)
            {
                HttpContext.Session.Remove(UserSessionKey);
                return RedirectToAction("Index", "Authentication");
            }

            byte[] token;
            try
            {
                token = Convert.FromBase64String(userSession.Token);
            }
            catch (FormatException)
            {
                HttpContext.Session.Remove(UserSessionKey);
                return RedirectToAction("Index", "Authentication");
            }

            if (token.Length is 0 or > 256)
            {
                HttpContext.Session.Remove(UserSessionKey);
                return RedirectToAction("Index", "Authentication");
            }

            var model = new ClientIndexViewModel
            {
                SearchTerm = searchTerm?.Trim()
            };
            ViewData["UserId"] = userSession.UserId;

            try
            {
                model = new ClientIndexViewModel
                {
                    SearchTerm = searchTerm?.Trim(),
                    Clients = await _clientService.GetClientsAsync(
                        token,
                        userSession.SecretKey,
                        userSession.ID,
                        searchTerm,
                        cancellationToken)
                };
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (SqlException exception)
            {
                _logger.LogError(exception, "A database error occurred while retrieving the client directory.");
                TempData["AlertType"] = "danger";
                TempData["AlertMessage"] = "The client list is temporarily unavailable. Please try again later.";
            }

            return View(model);
        }
    }
}

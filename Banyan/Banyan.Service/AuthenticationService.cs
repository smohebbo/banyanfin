using Banyan.Entities.Models;
using Banyan.Entities.viewModel;
using Banyan.Repository.Models;

namespace Banyan.Service;

public interface IAuthenticationService
{
    Task<UserSession?> Authenticate(
        loginViewModel loginViewModel,
        CancellationToken cancellationToken = default);
}

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;

    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public async Task<UserSession?> Authenticate(
        loginViewModel loginViewModel,
        CancellationToken cancellationToken = default)
    {
        var token = await _authenticationRepository.Authenticate(
            loginViewModel.UserId,
            loginViewModel.Password,
            loginViewModel.SecretKey,
            cancellationToken);

        if (token is not { Length: > 0 })
        {
            return null;
        }

        return new UserSession
        {
            ID = 1,
            UserId = loginViewModel.UserId,
            Token = Convert.ToBase64String(token),
            SecretKey = loginViewModel.SecretKey,
            RolePrivileges = null
        };
    }
}

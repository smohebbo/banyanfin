using Banyan.Data.Models;
using Banyan.Entities.Models;
using Microsoft.Data.SqlClient;

namespace Banyan.Repository.Models;

public interface IAuthenticationRepository
{
    Task<byte[]?> Authenticate(
        string userName,
        string password,
        string secretKey,
        CancellationToken cancellationToken = default);
}

public sealed class AuthenticationRepository : IAuthenticationRepository
{
    private readonly BanyanFinContext _context;

    public AuthenticationRepository(BanyanFinContext context)
    {
        _context = context;
    }

    public async Task<byte[]?> Authenticate(
        string userName,
        string password,
        string secretKey,
        CancellationToken cancellationToken = default)
    {
        var token = new OutputParameter<byte[]>();
        var returnValue = new OutputParameter<int>();

        try
        {
            await _context.Procedures.AuthenticateUserAsync(
                employeeID: null,
                userName: userName,
                password: password,
                secretKey: secretKey,
                token: token,
                returnValue: returnValue,
                cancellationToken: cancellationToken);
        }
        catch (SqlException exception) when (exception.Number == 50001)
        {
            return null;
        }

        var tokenValue = token.Value;
        return returnValue.Value == 0 && tokenValue is { Length: > 0 }
            ? tokenValue
            : null;
    }
}

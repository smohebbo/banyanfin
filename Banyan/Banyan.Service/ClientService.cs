using System.Globalization;
using Banyan.Entities.Models.clSchema;
using Banyan.Repository.Models;

namespace Banyan.Service;

public interface IClientService
{
    Task<IReadOnlyList<Client_GetResult>> GetClientsAsync(
        byte[] token,
        string secretKey,
        int sessionId,
        string? searchTerm,
        CancellationToken cancellationToken = default);
}

public sealed class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<IReadOnlyList<Client_GetResult>> GetClientsAsync(
        byte[] token,
        string secretKey,
        int sessionId,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var clients = await _clientRepository.GetAllAsync(
            token,
            secretKey,
            sessionId,
            cancellationToken);

        var filter = searchTerm?.Trim();
        if (string.IsNullOrEmpty(filter))
        {
            return clients
                .OrderBy(client => client.ClientName)
                .ThenBy(client => client.ClientLastName)
                .ThenBy(client => client.ID)
                .ToArray();
        }

        return clients
            .Where(client => Matches(client, filter))
            .OrderBy(client => client.ClientName)
            .ThenBy(client => client.ClientLastName)
            .ThenBy(client => client.ID)
            .ToArray();
    }

    private static bool Matches(Client_GetResult client, string filter)
    {
        return Contains(client.ClientName, filter)
            || Contains(client.ClientNameBangla, filter)
            || Contains(client.ClientLastName, filter)
            || Contains(client.MobileNo, filter)
            || Contains(client.EmailID, filter)
            || Contains(client.ID.ToString(CultureInfo.InvariantCulture), filter)
            || Contains(client.MemberNo?.ToString(CultureInfo.InvariantCulture), filter);
    }

    private static bool Contains(string? value, string filter)
    {
        return value?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true;
    }
}

using Banyan.Entities.Models.clSchema;

namespace Banyan.Entities.viewModel;

public sealed class ClientIndexViewModel
{
    public string? SearchTerm { get; init; }

    public IReadOnlyList<Client_GetResult> Clients { get; init; } = Array.Empty<Client_GetResult>();
}

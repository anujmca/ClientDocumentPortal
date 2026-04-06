using ClientDocumentPortal.Domain.Entities;

namespace ClientDocumentPortal.Application.Interfaces;

public interface IClientService
{
    Task<List<Client>> GetClientsAsync(CancellationToken cancellationToken = default);
    Task<Client?> GetClientByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Client> CreateClientAsync(string name, string email, string? phone, string? notes, CancellationToken cancellationToken = default);
    Task UpdateClientAsync(Guid id, string name, string email, string? phone, string? notes, CancellationToken cancellationToken = default);
    Task DeleteClientAsync(Guid id, CancellationToken cancellationToken = default);
}

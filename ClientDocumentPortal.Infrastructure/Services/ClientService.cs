using ClientDocumentPortal.Application.Interfaces;
using ClientDocumentPortal.Domain.Entities;
using ClientDocumentPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClientDocumentPortal.Infrastructure.Services;

public class ClientService : IClientService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantProvider _tenantProvider;

    public ClientService(ApplicationDbContext context, ITenantProvider tenantProvider)
    {
        _context = context;
        _tenantProvider = tenantProvider;
    }

    public Task<List<Client>> GetClientsAsync(CancellationToken cancellationToken = default)
    {
        return _context.Clients.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public Task<Client?> GetClientByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Client> CreateClientAsync(string name, string email, string? phone, string? notes, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetTenantId() ?? throw new InvalidOperationException("Tenant context required");

        var client = new Client
        {
            TenantId = tenantId,
            Name = name,
            Email = email,
            Phone = phone,
            Notes = notes
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync(cancellationToken);

        return client;
    }

    public async Task UpdateClientAsync(Guid id, string name, string email, string? phone, string? notes, CancellationToken cancellationToken = default)
    {
        var client = await GetClientByIdAsync(id, cancellationToken);
        if (client == null) return;

        client.Name = name;
        client.Email = email;
        client.Phone = phone;
        client.Notes = notes;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteClientAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var client = await GetClientByIdAsync(id, cancellationToken);
        if (client == null) return;

        client.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

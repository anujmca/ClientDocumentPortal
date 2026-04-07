using ClientDocumentPortal.Domain.Entities;

namespace ClientDocumentPortal.Application.Interfaces;

public interface IUserService
{
    Task<(bool success, string error)> RegisterTenantAsync(string businessName, string urlSlug, string adminEmail, string password);
    Task<(bool success, string error)> CreateClientUserAsync(Guid clientId, string email, string password);
    Task<List<ApplicationUser>> GetUsersByTenantAsync();
}

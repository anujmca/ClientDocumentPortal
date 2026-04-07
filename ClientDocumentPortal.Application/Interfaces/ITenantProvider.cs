namespace ClientDocumentPortal.Application.Interfaces;

public interface ITenantProvider
{
    Guid? GetTenantId();
    void SetTenantId(Guid tenantId);
    Task<Guid?> GetTenantIdBySlugAsync(string slug);
    string? GetCurrentTenantSlug();
}

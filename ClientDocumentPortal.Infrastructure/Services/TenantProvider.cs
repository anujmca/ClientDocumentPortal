using ClientDocumentPortal.Application.Interfaces;

namespace ClientDocumentPortal.Infrastructure.Services;

public class TenantProvider : ITenantProvider
{
    private Guid? _tenantId;

    public Guid? GetTenantId()
    {
        return _tenantId;
    }

    public void SetTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
    }

    public Task<Guid?> GetTenantIdBySlugAsync(string slug) => Task.FromResult<Guid?>(null);
    public string? GetCurrentTenantSlug() => null;
}

using ClientDocumentPortal.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClientDocumentPortal.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=ClientDocs;Username=postgres;Password=postgres")
                      .UseSnakeCaseNamingConvention();

        return new ApplicationDbContext(optionsBuilder.Options, new DummyTenantProvider());
    }

    private class DummyTenantProvider : ITenantProvider
    {
        public Guid? GetTenantId() => null;
        public void SetTenantId(Guid tenantId) { }
        public Task<Guid?> GetTenantIdBySlugAsync(string slug) => Task.FromResult<Guid?>(null);
        public string? GetCurrentTenantSlug() => null;
    }
}

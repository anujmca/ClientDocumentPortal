using ClientDocumentPortal.Application.Interfaces;
using ClientDocumentPortal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClientDocumentPortal.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly ITenantProvider _tenantProvider;
    private Guid? CurrentTenantId => _tenantProvider.GetTenantId();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
         _tenantProvider = tenantProvider;
    }

    public DbSet<Tenant> Tenants { get; set; } = default!;
    public DbSet<Client> Clients { get; set; } = default!;
    public DbSet<DocumentRequest> DocumentRequests { get; set; } = default!;
    public DbSet<DocumentItem> DocumentItems { get; set; } = default!;
    public DbSet<ActivityLog> ActivityLogs { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure UUID mapping if needed (Npgsql does it by default mostly)
        // Unique Slug
        builder.Entity<Tenant>()
            .HasIndex(t => t.UrlSlug)
            .IsUnique();

        // Identity table names to snake_case equivalent or just leave default and let the naming convention take over
        
        // Apply Query Filters
        builder.Entity<Tenant>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ApplicationUser>().HasQueryFilter(e => !e.Tenant.IsDeleted);

        builder.Entity<Client>().HasQueryFilter(e => !e.IsDeleted && (!CurrentTenantId.HasValue || e.TenantId == CurrentTenantId));
        builder.Entity<DocumentRequest>().HasQueryFilter(e => !e.IsDeleted && (!CurrentTenantId.HasValue || e.TenantId == CurrentTenantId));
        builder.Entity<DocumentItem>().HasQueryFilter(e => !e.IsDeleted && (!CurrentTenantId.HasValue || e.TenantId == CurrentTenantId));
        builder.Entity<ActivityLog>().HasQueryFilter(e => !e.IsDeleted && (!CurrentTenantId.HasValue || e.TenantId == CurrentTenantId));

        // Relationships
        builder.Entity<Tenant>()
            .HasMany(t => t.Clients)
            .WithOne(c => c.Tenant)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Tenant>()
            .HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Client>()
            .HasMany(c => c.DocumentRequests)
            .WithOne(dr => dr.Client)
            .HasForeignKey(dr => dr.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DocumentRequest>()
            .HasMany(dr => dr.Items)
            .WithOne(di => di.DocumentRequest)
            .HasForeignKey(di => di.DocumentRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    public override int SaveChanges()
    {
        SetAuditingFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetAuditingFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditingFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

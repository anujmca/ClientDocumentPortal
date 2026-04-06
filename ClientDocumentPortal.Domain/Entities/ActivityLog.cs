namespace ClientDocumentPortal.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public string Action { get; set; } = default!;
    public string? Details { get; set; }
    
    // To support JSONB we use a string or a JsonDocument but a simple string is easier without EF core npgsql json mapping complexity if we want simplicity
    public string? MetadataJson { get; set; } 
    
    // Multi-tenant
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; } = default!;
    
    // User who did it (if any)
    public Guid? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }
    
    // Client (if the client did it via public link)
    public Guid? ClientId { get; set; }
    public virtual Client? Client { get; set; }
}

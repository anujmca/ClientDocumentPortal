namespace ClientDocumentPortal.Domain.Entities;

public class DocumentRequest : BaseEntity
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    
    // Secure token for client access
    public string SecureToken { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime? ExpiresAt { get; set; }
    
    // Multi-tenant
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; } = default!;
    
    // Client relation
    public Guid ClientId { get; set; }
    public virtual Client Client { get; set; } = default!;
    
    // Navigation
    public virtual ICollection<DocumentItem> Items { get; set; } = new List<DocumentItem>();
}

namespace ClientDocumentPortal.Domain.Entities;

public class Client : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    
    // Multi-tenant
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; } = default!;
    
    // Navigation
    public virtual ICollection<DocumentRequest> DocumentRequests { get; set; } = new List<DocumentRequest>();
}

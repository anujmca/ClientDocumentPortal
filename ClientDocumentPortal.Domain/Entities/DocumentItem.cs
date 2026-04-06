using ClientDocumentPortal.Domain.Enums;

namespace ClientDocumentPortal.Domain.Entities;

public class DocumentItem : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    public string? Remarks { get; set; }
    
    // File upload info
    public string? FileKey { get; set; } // Path/Key in S3
    public string? OriginalFileName { get; set; }
    public string? ContentType { get; set; }
    public long? FileSizeBytes { get; set; }
    
    // Multi-tenant
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; } = default!;
    
    public Guid DocumentRequestId { get; set; }
    public virtual DocumentRequest DocumentRequest { get; set; } = default!;
}

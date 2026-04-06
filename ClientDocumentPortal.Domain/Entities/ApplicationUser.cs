using Microsoft.AspNetCore.Identity;

namespace ClientDocumentPortal.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    
    // Auditing keys
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Tenant reference
    public Guid TenantId { get; set; }
    public virtual Tenant Tenant { get; set; } = default!;
}

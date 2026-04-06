namespace ClientDocumentPortal.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Limits
    public int MaxClients { get; set; } = 100;
    public long MaxStorageBytes { get; set; } = 10L * 1024 * 1024 * 1024; // 10GB default
    
    // Navigation
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}

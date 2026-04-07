using ClientDocumentPortal.Application.Interfaces;
using ClientDocumentPortal.Domain.Entities;
using ClientDocumentPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClientDocumentPortal.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ITenantProvider _tenantProvider;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ApplicationDbContext context,
        ITenantProvider tenantProvider)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _tenantProvider = tenantProvider;
    }

    public async Task<(bool success, string error)> RegisterTenantAsync(string businessName, string urlSlug, string adminEmail, string password)
    {
        // Check if slug already exists
        if (await _context.Tenants.AnyAsync(t => t.UrlSlug == urlSlug))
        {
            return (false, "This URL slug is already taken.");
        }

        // 1. Create Tenant
        var tenant = new Tenant { Name = businessName, UrlSlug = urlSlug, IsActive = true };
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        // 2. Create Admin Role if not exists
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
        }

        // 3. Create Admin User
        var user = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            TenantId = tenant.Id,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, "Admin");

        return (true, "");
    }

    public async Task<(bool success, string error)> CreateClientUserAsync(Guid clientId, string email, string password)
    {
        var tenantId = _tenantProvider.GetTenantId() ?? throw new InvalidOperationException("Tenant context required");

        // Ensure Client role exists
        if (!await _roleManager.RoleExistsAsync("Client"))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Client"));
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "Client",
            LastName = "User",
            TenantId = tenantId,
            ClientId = clientId,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, "Client");

        return (true, "");
    }

    public async Task<List<ApplicationUser>> GetUsersByTenantAsync()
    {
        var tenantId = _tenantProvider.GetTenantId();
        if (!tenantId.HasValue) return new List<ApplicationUser>();

        return await _userManager.Users
            .Where(u => u.TenantId == tenantId.Value)
            .ToListAsync();
    }
}

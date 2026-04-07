using ClientDocumentPortal.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace ClientDocumentPortal.Web.Services;

public class WebTenantProvider : ITenantProvider
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly NavigationManager _navigationManager;
    private Guid? _cachedTenantId;
    private string? _cachedTenantSlug;

    public WebTenantProvider(
        AuthenticationStateProvider authStateProvider, 
        IServiceScopeFactory scopeFactory,
        NavigationManager navigationManager)
    {
        _authStateProvider = authStateProvider;
        _scopeFactory = scopeFactory;
        _navigationManager = navigationManager;
    }

    public Guid? GetTenantId()
    {
        if (_cachedTenantId.HasValue) 
            return _cachedTenantId;

        // Try to get from URL slug first
        var slug = GetCurrentTenantSlug();
        if (!string.IsNullOrEmpty(slug))
        {
            // We use GetResult() here because it's called synchronously in the pipeline
            _cachedTenantId = GetTenantIdBySlugAsync(slug).GetAwaiter().GetResult();
            if (_cachedTenantId.HasValue) return _cachedTenantId;
        }

        try
        {
            // In Blazor Server, GetAuthenticationStateAsync() is typically completed 
            // once the circuit is established and the user is logged in.
            var authStateTask = _authStateProvider.GetAuthenticationStateAsync();
            
            // We use .GetAwaiter().GetResult() only if the task is already completed or 
            // within a context where it won't deadlock (common in Blazor Server scoped services).
            var authState = authStateTask.GetAwaiter().GetResult();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var claimValue = user.FindFirst("tenant_id")?.Value;
                if (Guid.TryParse(claimValue, out var tenantId))
                {
                    _cachedTenantId = tenantId;
                    return tenantId;
                }
            }
        }
        catch
        {
            // Fallback or log if needed
        }

        return _cachedTenantId;
    }

    public void SetTenantId(Guid tenantId)
    {
        _cachedTenantId = tenantId;
    }

    public async Task<Guid?> GetTenantIdBySlugAsync(string slug)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ClientDocumentPortal.Infrastructure.Data.ApplicationDbContext>();
        
        var tenant = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .FirstOrDefaultAsync(context.Tenants, t => t.UrlSlug == slug);
            
        return tenant?.Id;
    }

    public string? GetCurrentTenantSlug()
    {
        if (!string.IsNullOrEmpty(_cachedTenantSlug)) return _cachedTenantSlug;

        try
        {
            var uri = new Uri(_navigationManager.Uri);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0 && segments[0] != "register" && segments[0] != "login")
            {
                _cachedTenantSlug = segments[0];
                return _cachedTenantSlug;
            }
        }
        catch { }

        return null;
    }
}

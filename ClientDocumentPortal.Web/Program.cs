using ClientDocumentPortal.Web.Components;
using ClientDocumentPortal.Application;
using ClientDocumentPortal.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Clean Architecture Layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Override ITenantProvider with a Blazor-aware implementation
builder.Services.AddScoped<ClientDocumentPortal.Application.Interfaces.ITenantProvider, ClientDocumentPortal.Web.Services.WebTenantProvider>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

var app = builder.Build();

// Tenant PathBase Middleware
app.Use((context, next) =>
{
    var pathStr = context.Request.Path.Value;
    if (string.IsNullOrEmpty(pathStr)) return next();

    var segments = pathStr.Split('/', StringSplitOptions.RemoveEmptyEntries);
    
    // Ignore global or framework paths
    string[] ignoredRoots = { "_blazor", "_framework", "css", "bootstrap", "app.css", "favicon.png", "favicon.ico", "clientdocumentportal.web.styles.css", "login", "register", "logout" };
    
    if (segments.Length > 0 && !ignoredRoots.Contains(segments[0].ToLowerInvariant()))
    {
        var slug = segments[0];
        
        // Exact match e.g., "/cafirm1" -> redirect to "/cafirm1/"
        if (pathStr.Equals($"/{slug}", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect($"/{slug}/");
            return Task.CompletedTask;
        }

        if (pathStr.StartsWith($"/{slug}/", StringComparison.OrdinalIgnoreCase))
        {
            context.Request.PathBase = new PathString($"/{slug}");
            context.Request.Path = new PathString(pathStr.Substring(slug.Length + 1));
        }
    }

    return next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/logout", async (SignInManager<ClientDocumentPortal.Domain.Entities.ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

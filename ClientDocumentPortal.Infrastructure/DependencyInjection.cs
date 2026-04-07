using Amazon.S3;
using ClientDocumentPortal.Application.Interfaces;
using ClientDocumentPortal.Domain.Entities;
using ClientDocumentPortal.Infrastructure.Data;
using ClientDocumentPortal.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientDocumentPortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoles<IdentityRole<Guid>>()
            .AddClaimsPrincipalFactory<TenantClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();

        // AWS S3 / R2 Configuration
        var s3Config = new AmazonS3Config
        {
            ServiceURL = configuration["Storage:ServiceUrl"],
            AuthenticationRegion = "auto"
        };
        var s3Client = new AmazonS3Client(configuration["Storage:AccessKey"], configuration["Storage:SecretKey"], s3Config);
        services.AddSingleton<IAmazonS3>(s3Client);

        services.AddScoped<IFileStorageService, R2StorageService>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IDocumentRequestService, DocumentRequestService>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddHostedService<ReminderBackgroundService>();

        return services;
    }
}

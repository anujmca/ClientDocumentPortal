using Microsoft.Extensions.DependencyInjection;

namespace ClientDocumentPortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add MediatR if using, or standard services
        // services.AddMediatR(Assembly.GetExecutingAssembly());
        
        return services;
    }
}

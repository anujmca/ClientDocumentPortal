using ClientDocumentPortal.Domain.Enums;
using ClientDocumentPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClientDocumentPortal.Infrastructure.Services;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderBackgroundService> _logger;

    public ReminderBackgroundService(IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Find items that are pending
                var pendingItems = await dbContext.DocumentItems
                    .Include(i => i.DocumentRequest)
                    .ThenInclude(r => r.Client)
                    .Where(i => i.Status == DocumentStatus.Pending && !i.IsDeleted)
                    .ToListAsync(stoppingToken);

                if (pendingItems.Any())
                {
                    _logger.LogInformation($"Found {pendingItems.Count} pending items to send reminders for.");
                    // Dummy logic to send email
                    // foreach (var item in pendingItems) { SendEmail(item.DocumentRequest.Client.Email, ...); }
                }

                // Wait 24 hours between runs (Using 1 hour for demo/debug purposes instead or simple fixed delay)
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ReminderBackgroundService.");
                // Retry later
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}

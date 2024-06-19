using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;
using Microsoft.Extensions.Hosting;


public class StartupHostedService:IHostedService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StartupHostedService> _logger;

    public StartupHostedService(IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider, ILogger<StartupHostedService> logger)
    {
        _recurringJobManager = recurringJobManager;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting StartupHostedService");
        
        using (var scope = _serviceProvider.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IInactiveUserNotificationService>();
            _recurringJobManager.AddOrUpdate(
                "NotifyInactiveUsers",
                () => service.NotifyInactiveUsersAsync(),
                Cron.MinuteInterval(1));// Cron.Daily
        }
        
        _logger.LogInformation("Scheduled NotifyInactiveUsers job");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

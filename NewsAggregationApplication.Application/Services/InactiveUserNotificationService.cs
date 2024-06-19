using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.CQS.Commands.InactiveUserNotification;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class InactiveUserNotificationService:IInactiveUserNotificationService
{
   
    private readonly ILogger<InactiveUserNotificationService> _logger;
    private readonly IMediator _mediator;

    public InactiveUserNotificationService(ILogger<InactiveUserNotificationService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task NotifyInactiveUsersAsync()
    {
        try
        {
            await _mediator.Send(new NotifyInactiveUsersCommand());
            _logger.LogInformation("Inactive users have been notified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error notifying inactive users");
        }
       
    }
}
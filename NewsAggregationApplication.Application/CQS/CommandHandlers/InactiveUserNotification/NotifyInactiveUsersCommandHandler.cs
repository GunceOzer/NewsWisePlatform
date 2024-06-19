using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.CQS.Commands.InactiveUserNotification;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.InactiveUserNotification;

public class NotifyInactiveUsersCommandHandler:IRequestHandler<NotifyInactiveUsersCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotifyInactiveUsersCommandHandler> _logger;

    public NotifyInactiveUsersCommandHandler(UserManager<User> userManager, IEmailService emailService, ILogger<NotifyInactiveUsersCommandHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(NotifyInactiveUsersCommand request, CancellationToken cancellationToken)
    {
        var tenDaysAgo = DateTime.UtcNow.AddDays(-10);
        var users = _userManager.Users
            .Where(u => u.LastLoginDate < tenDaysAgo && (u.LastNotifiedDate == null || u.LastNotifiedDate < tenDaysAgo))
            .ToList();


        _logger.LogInformation($"Found {users.Count} inactive users.");

        foreach (var user in users)
        {
            try
            {
                _logger.LogInformation($"Attempting to send email to {user.Email}.");
                await _emailService.SendEmailAsync(user.Email, "We Miss You!",
                    "It looks like you haven't been active for a while. We miss you! Please log in to check out what's new.");
                user.LastNotifiedDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation($"Email sent to {user.Email}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {user.Email}: {ex.Message}");
            }
        }
    }
}
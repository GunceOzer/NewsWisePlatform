namespace NewsAggregationApplication.UI.Interfaces;

public interface IInactiveUserNotificationService
{
    public Task NotifyInactiveUsersAsync();
}
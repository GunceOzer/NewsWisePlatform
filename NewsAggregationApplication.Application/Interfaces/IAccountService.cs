namespace NewsAggregationApplication.UI.Interfaces;

public interface IAccountService
{
    public Task CreateRoles(IServiceProvider serviceProvider);
    //public Task EnsureRolesAsync();
    //public Task EnsureAdminUserAsync();
}
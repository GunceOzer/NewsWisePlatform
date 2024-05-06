using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands.Bookmark;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Bookmark;

public class RemoveBookmarkCommandHandler:IRequestHandler<RemoveBookmarkCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<RemoveBookmarkCommandHandler> _logger;

    public RemoveBookmarkCommandHandler(NewsDbContext dbContext, ILogger<RemoveBookmarkCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Handle(RemoveBookmarkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var bookmark = await _dbContext.Bookmarks.FirstOrDefaultAsync(b => b.ArticleId == request.ArticleId && b.UserId == request.UserId);
            if (bookmark == null)
            {
                _logger.LogWarning($"Attempt to remove non-existent bookmark: ArticleID={request.ArticleId}, UserID={request.UserId}");
                return false;
            }

            _dbContext.Remove(bookmark);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Bookmark removed successfully: ArticleID={request.ArticleId}, UserID={request.UserId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing bookmark: ArticleID={request.ArticleId}, UserID={request.UserId}");
            return false;
        }
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands.Bookmark;

namespace NewsAggregationApplication.UI.Queries.Article;

public class IsArticleBookmarkedByUserQueryHandler:IRequestHandler<IsArticleBookmarkedByUserQuery,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<IsArticleBookmarkedByUserQueryHandler> _logger;

    public IsArticleBookmarkedByUserQueryHandler(NewsDbContext dbContext, ILogger<IsArticleBookmarkedByUserQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Handle(IsArticleBookmarkedByUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var isBookmarked = await _dbContext.Bookmarks.AnyAsync(b => b.ArticleId == request.ArticleId && b.UserId == request.UserId);
            _logger.LogInformation($"Checked bookmark status: ArticleID={request.ArticleId}, UserID={request.UserId}, IsBookmarked={isBookmarked}");
            return isBookmarked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking bookmark status for: ArticleID={request.ArticleId}, UserID={request.UserId}");
            return false;
        }
    }
}
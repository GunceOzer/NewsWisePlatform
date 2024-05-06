using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands.Bookmark;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Bookmark;

public class BookmarkArticlesCommandHandler: IRequestHandler<BookmarkArticlesCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<BookmarkArticlesCommandHandler> _logger;

    public BookmarkArticlesCommandHandler(NewsDbContext dbContext, ILogger<BookmarkArticlesCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Handle(BookmarkArticlesCommand request, CancellationToken cancellationToken)
    {
        
            try
            {
                if (_dbContext.Bookmarks.Any(b => b.ArticleId == request.ArticleId && b.UserId == request.UserId))
                {
                    _logger.LogInformation("Attempt to bookmark already bookmarked article: ArticleID={request.ArticleId}, UserID={request.UserId}");
                    return false;
                }

                var bookmark = new Data.Entities.Bookmark
                {
                    Id = Guid.NewGuid(),
                    ArticleId = request.ArticleId,
                    UserId = request.UserId
                };
                _dbContext.Bookmarks.Add(bookmark);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Article bookmarked successfully: ArticleID={request.ArticleId}, UserID={request.UserId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bookmarking article: ArticleID={request.ArticleId}, UserID={request.UserId}");
                return false;
            }

        

    }
}
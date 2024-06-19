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
            if (_dbContext.Bookmarks.Any(b =>
                    b.ArticleId == request.BookmarkDto.ArticleId && b.UserId == request.BookmarkDto.UserId))
            {
                _logger.LogInformation(
                    "Attempt to bookmark already bookmarked article: ArticleID={request.ArticleId}, UserID={request.UserId}");
                return false;
            }

            var bookmark = new Data.Entities.Bookmark
            {
                Id = request.BookmarkDto.Id,
                UserId = request.BookmarkDto.UserId,
                ArticleId = request.BookmarkDto.ArticleId,
            };

            _dbContext.Bookmarks.Add(bookmark);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation(
                $"Article bookmarked successfully: ArticleID={request.BookmarkDto.ArticleId}, UserID={request.BookmarkDto.UserId}");
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error bookmarking article with ID: {request.BookmarkDto.ArticleId}");
            return false;


        }
    }
}
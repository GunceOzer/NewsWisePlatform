using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.CQS.CommandHandlers.Bookmark;
using NewsAggregationApplication.UI.CQS.Commands.Bookmark;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.Services;

public class BookmarkService:IBookmarkService
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<BookmarkService> _logger;
    private readonly IMediator _mediator;


    public BookmarkService(NewsDbContext dbContext, ILogger<BookmarkService> logger, IMediator mediator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task<bool> BookmarkArticleAsync(Guid articleId, Guid userId)
    {
        try
        {
            await _mediator.Send(new BookmarkArticlesCommand { ArticleId = articleId, UserId = userId });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"Error bookmarking article with the id{articleId}");
            return false;
        }
        

    }

    public async Task<bool> RemoveBookmarkAsync(Guid articleId, Guid userId)
    {
        try
        {
            await _mediator.Send(new RemoveBookmarkCommand { ArticleId = articleId, UserId = userId });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"Error removing the bookmark with article id{articleId}");
            return false;
        }
    }

    
    public async Task<IEnumerable<ArticleDto>> GetBookmarkArticlesAsync(Guid userId)
    {
        try
        {
            // Create and send the query
            var query = new GetBookmarkedArticlesQuery { UserId = userId };
            var bookmarkedArticles = await _mediator.Send(query);

            _logger.LogInformation($"Retrieved {bookmarkedArticles.Count()} bookmarked articles for UserID={userId}");
            return bookmarkedArticles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving bookmarked articles for UserID={userId}");
            return new List<ArticleDto>();
        }
        
    }

    /*public async Task<IEnumerable<Guid>> GetBookmarkedArticleIdsByUser(Guid userId)
    {

        try
        {
            var query = new GetBookmarkedArticleIdsByUserQuery { UserId = userId };

            var bookmarkedArticlesByUser = await _mediator.Send(query);
            _logger.LogInformation(
                $"Retrieved {bookmarkedArticlesByUser.Count()} bookmarked articles for UserID={userId}");
            return bookmarkedArticlesByUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving bookmarked articles for UserID={userId}");
            return new List<Guid>();
        }
    }*/
    /*try
    {
        var bookmarkedArticleIds = await _dbContext.Bookmarks
            .Where(b => b.UserId == userId)
            .Select(b => b.ArticleId)  // Select only the ArticleId
            .ToListAsync();

        _logger.LogInformation($"Retrieved bookmarked article IDs for UserID={userId}");
        return bookmarkedArticleIds;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error retrieving bookmarked article IDs for UserID={userId}");
        return new List<Guid>();
    }*/
    

    

    public async Task<bool> IsArticleBookmarkedByUser(Guid articleId, Guid userId)
    { 
        try
        { 
            //creating and sending the query
            var query = new IsArticleBookmarkedByUserQuery() { ArticleId = articleId,UserId = userId };
            var isBookmarkedByUser = await _mediator.Send(query);
            
            return isBookmarkedByUser;
        }
        catch (Exception ex)
        {
            return false;
        }
        
    }
}
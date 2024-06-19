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
    private readonly ILogger<BookmarkService> _logger;
    private readonly IMediator _mediator;


    public BookmarkService(ILogger<BookmarkService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
   

    public async Task<bool> BookmarkArticleAsync(BookmarkDto bookmartDto)
    {
        try
        {
            var command = new BookmarkArticlesCommand{BookmarkDto = bookmartDto};
            return await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bookmarking the article.");
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
    
    public async Task<bool> IsArticleBookmarkedByUser(Guid articleId, Guid userId)
    { 
        try
        { 
            
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
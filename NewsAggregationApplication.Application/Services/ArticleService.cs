using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Commands;
using NewsAggregationApplication.UI.Commands.Article;
using NewsAggregationApplication.UI.CQS.Commands.Article;
using NewsAggregationApplication.UI.CQS.Queries;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;
namespace NewsAggregationApplication.UI.Services;

public class ArticleService:IArticleService
{
    
    private readonly ILogger<ArticleService> _logger;
    private readonly IMediator _mediator;
    
    public ArticleService( ILogger<ArticleService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
       
    }
    
    public async Task<IEnumerable<ArticleDto>> GetArticlesAsync(Guid? userId)
    {
        var allArticlesQuery = new GetArticlesQuery{UserId = userId};
        try
        {
            var allArticles = await _mediator.Send(allArticlesQuery);
            _logger.LogInformation($"Retrieved {allArticles.Count()} articles.");
            return allArticles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve articles.");
            return new List<ArticleDto>(); 
        }
    }

    public async Task<ArticleDto?> GetArticlesByIdAsync(Guid id, Guid userId)
    {
        try
        {
            var getArticlesById = new GetArticleByIdQuery { ArticleId = id, UserId = userId };
            return await _mediator.Send(getArticlesById);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"Error occured while getting the article");
            return null;
        }
    }

    public async Task AggregateFromSourceAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new AggregateArticlesCommand(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while aggregating articles from source.");
        }
        
        
    }
   


    public async Task<bool> DeleteArticleAsync(Guid id)
    {
        try
        {
            var command = new DeleteArticleCommand { ArticleId = id }; 
            await _mediator.Send(command);
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting article with ID: {id}.");
            return false;
        }

       
    }

    public async Task<IEnumerable<ArticleDto>> GetSortedArticlesByPositivityAsync(Guid? userId, bool sortByPositive)
    {
        try
        {
            var query = new GetSortedArticlesByPositivityQuery { UserId = userId, SortByPositive = sortByPositive };
            return await _mediator.Send(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting the sorted articles by positivity");
            return Enumerable.Empty<ArticleDto>();

        }
    }

    public async Task<IEnumerable<ArticleDto>> GetSortedArticlesByDate(Guid? userId, bool sortByNewest)
    {
        try
        {
            var query = new GetNewestArticlesQuery() { UserId = userId, SortByNewest = sortByNewest };
            return await _mediator.Send(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error getting the sorted articles by date");
            return Enumerable.Empty<ArticleDto>();
        }
    }
    
}
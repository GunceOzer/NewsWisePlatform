using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Commands;
using NewsAggregationApplication.UI.Commands.Article;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;


namespace NewsAggregationApplication.UI.Services;

public class ArticleService:IArticleService
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<ArticleService> _logger;
    private readonly ArticleMapper _mapper;
    private readonly IContentScraper _contentScraper;
    private readonly IImageExtractor _imageExtractor;
    private readonly IBookmarkService _bookmarkService;
    private readonly IMediator _mediator;


    public ArticleService(NewsDbContext dbContext, ILogger<ArticleService> logger, ArticleMapper mapper, IImageExtractor imageExtractor, IContentScraper contentScraper, IBookmarkService bookmarkService, IMediator mediator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
        _imageExtractor = imageExtractor;
        _contentScraper = contentScraper;
        _bookmarkService = bookmarkService;
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
            return new List<ArticleDto>(); // Return an empty list on failure
        }
        
       
    }

    public async Task<ArticleDto?> GetArticlesByIdAsync(Guid id, Guid userId)
    {
        try
        {
            _logger.LogInformation($"Fetching article with ID: {id}");
            var article = await _dbContext.Articles
                .Include(a => a.Likes)
                .Include(a => a.Comments)
                .ThenInclude(c => c.User)
                .SingleOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                _logger.LogWarning($"Article with ID: {id} not found.");
                return null;
            }

            _logger.LogInformation($"Article with ID: {id} retrieved successfully.");
            //working after this line is added new
            //return _mapper.ArticleToArticleDto(article);
            var isBookmarked = await _dbContext.Bookmarks.AnyAsync(b => b.ArticleId == id && b.UserId == userId);

            var articleDto = _mapper.ArticleToArticleDto(article);
            articleDto.IsBookmarked = isBookmarked;
            _logger.LogInformation($"Article with ID: {id} retrieved successfully.");
            return articleDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while retrieving article with ID: {id}.");
            return null;
        }
    }

    public async Task AggregateFromSourceAsync(IEnumerable<string> rssLinks,CancellationToken cancellationToken)
    {
       
        foreach (var rssLink in rssLinks)
        {
            try
            {
                var feed = await _mediator.Send(new FetchRssFeedQuery { Url = rssLink }, cancellationToken);
                await _mediator.Send(new InitializeArticlesByRssCommand { RssData = feed.Items }, cancellationToken);
                
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing RSS link {rssLink}");
            }
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

        /*try
        {
            var article = await _dbContext.Articles.FindAsync(id);
            if (article == null)
            {
                _logger.LogWarning($"Article with ID: {id} not found for deletion.");
                return false;
            }

            _dbContext.Articles.Remove(article);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Article with ID: {id} successfully deleted.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while deleting article with ID: {id}.");
            return false;
        }*/
    }
    
    
    
}
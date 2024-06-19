using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.QueryHandlers.Article;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.QueryHandlers.Article;

public class GetArticleByIdQueryHandler: IRequestHandler<GetArticleByIdQuery, ArticleDto>
{
    private readonly NewsDbContext _dbContext;
    private readonly ArticleMapper _mapper;
    private readonly ILogger<GetArticleByIdQueryHandler> _logger;

    public GetArticleByIdQueryHandler(NewsDbContext dbContext, ArticleMapper mapper, ILogger<GetArticleByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ArticleDto> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation($"Fetching article with ID: {request.ArticleId}");
            var article = await _dbContext.Articles
                .Include(a => a.Likes)
                .Include(a => a.Comments)
                .ThenInclude(c => c.User)
                .SingleOrDefaultAsync(a => a.Id == request.ArticleId);

            if (article == null)
            {
                _logger.LogWarning($"Article with ID: {request.ArticleId} not found.");
                return null;
            }

            _logger.LogInformation($"Article with ID: {request.ArticleId} retrieved successfully.");
            var isBookmarked = await _dbContext.Bookmarks.AnyAsync(b => b.ArticleId == request.ArticleId && b.UserId == request.UserId);

            var articleDto = _mapper.ArticleToArticleDto(article);
            articleDto.IsBookmarked = isBookmarked;
            _logger.LogInformation($"Article with ID: {request.ArticleId} retrieved successfully.");
            return articleDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error while retrieving article with ID: {request.ArticleId}.");
            return null;
        }
    }
}
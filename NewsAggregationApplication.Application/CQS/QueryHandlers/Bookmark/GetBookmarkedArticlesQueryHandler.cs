using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.QueryHandlers.Article;

public class GetBookmarkedArticlesQueryHandler: IRequestHandler<GetBookmarkedArticlesQuery,IEnumerable<ArticleDto>>
{
    private readonly NewsDbContext _dbContext;
    private readonly ArticleMapper _mapper;
    private readonly ILogger<GetBookmarkedArticlesQuery> _logger;

    public GetBookmarkedArticlesQueryHandler(NewsDbContext dbContext, ArticleMapper mapper, ILogger<GetBookmarkedArticlesQuery> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ArticleDto>> Handle(GetBookmarkedArticlesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var bookmarks = await _dbContext.Bookmarks
                .Include(b => b.Article)
                .ThenInclude(a => a.Likes)
                .Where(b => b.UserId == request.UserId)
                .Select(b => b.Article)
                .ToListAsync();

            var bookmarkDtos = bookmarks.Select(_mapper.ArticleToArticleDto).ToList();
            _logger.LogInformation($"Retrieved bookmarked articles for UserID={request.UserId}");
            return bookmarkDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving bookmarked articles for UserID={request.UserId}");
            return new List<ArticleDto>();
        }
    }
       
}
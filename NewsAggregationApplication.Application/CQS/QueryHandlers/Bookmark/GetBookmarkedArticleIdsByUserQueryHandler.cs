using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.QueryHandlers.Article;

public class GetBookmarkedArticleIdsByUserQueryHandler:IRequestHandler<GetBookmarkedArticleIdsByUserQuery,IEnumerable<Guid>>
{
    private readonly NewsDbContext _dbContext;
    private readonly ArticleMapper _mapper;
    private readonly ILogger<GetBookmarkedArticleIdsByUserQueryHandler> _logger;

    public GetBookmarkedArticleIdsByUserQueryHandler(NewsDbContext dbContext, ArticleMapper mapper, ILogger<GetBookmarkedArticleIdsByUserQueryHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<Guid>> Handle(GetBookmarkedArticleIdsByUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var bookmarkedArticleIds = await _dbContext.Bookmarks
                .Where(b => b.UserId == request.UserId)
                .Select(b => b.ArticleId) 
                .ToListAsync();

            _logger.LogInformation($"Retrieved bookmarked article IDs for UserID={request.UserId}");
            return bookmarkedArticleIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving bookmarked article IDs for UserID={request.UserId}");
            return new List<Guid>();
        }
    }
}
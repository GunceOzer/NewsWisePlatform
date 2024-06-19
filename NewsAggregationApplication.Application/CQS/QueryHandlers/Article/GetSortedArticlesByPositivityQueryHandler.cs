using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.CQS.QueryHandlers.Article;

public class GetSortedArticlesByPositivityQueryHandler:IRequestHandler<GetSortedArticlesByPositivityQuery,IEnumerable<ArticleDto>>
{
    public readonly NewsDbContext _dbContext;
    private readonly ArticleMapper _articleMapper;

    public GetSortedArticlesByPositivityQueryHandler(NewsDbContext dbContext, ArticleMapper articleMapper)
    {
        _dbContext = dbContext;
        _articleMapper = articleMapper;
    }

    public async Task<IEnumerable<ArticleDto>> Handle(GetSortedArticlesByPositivityQuery request, CancellationToken cancellationToken)
    {
        var articleQuery = _dbContext.Articles.AsQueryable();

        if (request.UserId.HasValue)
        {
            
        }

        var sortedArticles = request.SortByPositive
            ? await articleQuery.OrderByDescending(a => a.PositivityScore).ToListAsync(cancellationToken)
            : await articleQuery.OrderBy(a => a.PositivityScore).ToListAsync(cancellationToken);

        return sortedArticles.Select(a => _articleMapper.ArticleToArticleDto(a));
    }
}
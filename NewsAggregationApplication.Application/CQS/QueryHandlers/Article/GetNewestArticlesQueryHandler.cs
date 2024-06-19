using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.CQS.QueryHandlers.Article;

public class GetNewestArticlesQueryHandler:IRequestHandler<GetNewestArticlesQuery,IEnumerable<ArticleDto>>
{
    
    public readonly NewsDbContext _dbContext;
    private readonly ArticleMapper _articleMapper;

    public GetNewestArticlesQueryHandler(NewsDbContext dbContext, ArticleMapper articleMapper)
    {
        _dbContext = dbContext;
        _articleMapper = articleMapper;
    }

    public async Task<IEnumerable<ArticleDto>> Handle(GetNewestArticlesQuery request, CancellationToken cancellationToken)
    {
        var articleQuery = _dbContext.Articles.AsQueryable();
        var sortedArticles = request.SortByNewest
            ? await articleQuery.OrderByDescending(a => a.PublishedDate).ToListAsync(cancellationToken)
            : await articleQuery.OrderBy(a => a.PublishedDate).ToListAsync(cancellationToken);
        return sortedArticles.Select(a => _articleMapper.ArticleToArticleDto(a));
    }
}
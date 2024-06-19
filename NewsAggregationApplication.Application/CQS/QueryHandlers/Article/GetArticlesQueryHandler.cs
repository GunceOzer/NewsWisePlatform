using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.CQS.QueryHandlers.Article;

public class GetArticlesQueryHandler:IRequestHandler<GetArticlesQuery,IEnumerable<ArticleDto>>
{
    private readonly NewsDbContext _dbContext;
    private readonly ArticleMapper _mapper;
    private readonly ILogger<GetArticlesQueryHandler> _logger;

    public GetArticlesQueryHandler(NewsDbContext dbContext, ArticleMapper mapper, ILogger<GetArticlesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ArticleDto>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        var articles = await _dbContext.Articles.Include(a => a.Likes).Include(a => a.Bookmarks).ToListAsync(cancellationToken);
        return articles.Select(_mapper.ArticleToArticleDto);
        
    }
}
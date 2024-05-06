/*using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Queries.Article;

namespace NewsAggregationApplication.UI.QueryHandlers.Article;

public class GetArticleByIdQueryHandler: IRequestHandler<GetArticleByIdQuery, ArticleDto>
{
    private readonly NewsDbContext _dbContext;
    private readonly ArticleMapper _mapper;

    public GetArticleByIdQueryHandler(NewsDbContext dbContext, ArticleMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ArticleDto> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        var article = await _dbContext.Articles
            .Include(a => a.Likes)
            .Include(a => a.Comments)
            .SingleOrDefaultAsync(a => a.Id == request.ArticleId, cancellationToken);

        if (article == null)
        {
            return null;
        }

        return _mapper.ArticleToArticleDto(article);
    }
}*/
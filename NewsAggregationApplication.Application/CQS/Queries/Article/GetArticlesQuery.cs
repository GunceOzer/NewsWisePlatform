using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Queries.Article;

public class GetArticlesQuery:IRequest<IEnumerable<ArticleDto>>
{
    public Guid? UserId { get; set; }
}
using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Queries.Article;

public class GetNewestArticlesQuery:IRequest<IEnumerable<ArticleDto>>
{
    public Guid? UserId { get; set; }
    public bool SortByNewest { get; set; }
}
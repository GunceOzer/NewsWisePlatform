using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Queries.Article;

public class GetSortedArticlesByPositivityQuery:IRequest<IEnumerable<ArticleDto>>
{
    public Guid? UserId { get; set; }
    public bool SortByPositive { get; set; }
}
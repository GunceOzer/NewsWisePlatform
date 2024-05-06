using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Queries.Article;

public class GetBookmarkedArticleIdsByUserQuery:IRequest<IEnumerable<Guid>>
{
    public Guid UserId { get; set; }
}
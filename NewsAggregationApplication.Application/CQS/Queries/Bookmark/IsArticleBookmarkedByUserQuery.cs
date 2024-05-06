using MediatR;

namespace NewsAggregationApplication.UI.CQS.Commands.Bookmark;

public class IsArticleBookmarkedByUserQuery:IRequest<bool>
{
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
}
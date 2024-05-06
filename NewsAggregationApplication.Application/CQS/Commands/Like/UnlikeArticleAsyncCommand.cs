using MediatR;

namespace NewsAggregationApplication.UI.CQS.Commands.Like;

public class UnlikeArticleAsyncCommand:IRequest<bool>
{
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
}
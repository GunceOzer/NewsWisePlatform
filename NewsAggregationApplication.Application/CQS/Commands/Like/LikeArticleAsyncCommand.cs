using MediatR;

namespace NewsAggregationApplication.UI.CQS.Commands.Like;

public class LikeArticleAsyncCommand:IRequest<bool>
{
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
    
}
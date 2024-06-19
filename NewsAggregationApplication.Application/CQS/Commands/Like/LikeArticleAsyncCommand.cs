using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.CQS.Commands.Like;

public class LikeArticleAsyncCommand:IRequest<bool>
{
    
    public LikeDto LikeDto { get; set; }

}
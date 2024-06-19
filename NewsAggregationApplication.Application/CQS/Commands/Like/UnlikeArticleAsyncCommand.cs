using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.CQS.Commands.Like;

public class UnlikeArticleAsyncCommand:IRequest<bool>
{
    
    public LikeDto LikeDto;
}
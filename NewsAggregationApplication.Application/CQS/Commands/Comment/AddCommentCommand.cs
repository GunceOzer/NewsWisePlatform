using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.CQS.Commands;

public class AddCommentCommand:IRequest<bool>
{
   
    public CommentDto CommentDto { get; set; }
}
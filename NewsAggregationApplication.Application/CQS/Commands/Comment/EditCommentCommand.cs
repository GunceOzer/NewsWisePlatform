using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.CQS.Commands;

public class EditCommentCommand:IRequest<bool>
{
   
    public CommentDto CommentDto { get; set; }
}
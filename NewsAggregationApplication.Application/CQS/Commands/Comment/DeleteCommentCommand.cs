using MediatR;

namespace NewsAggregationApplication.UI.CQS.Commands;

public class DeleteCommentCommand:IRequest<bool>
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public bool IsAdmin { get; set; }
}
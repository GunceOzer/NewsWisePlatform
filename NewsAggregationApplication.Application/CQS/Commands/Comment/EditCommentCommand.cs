using MediatR;

namespace NewsAggregationApplication.UI.CQS.Commands;

public class EditCommentCommand:IRequest<bool>
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public bool IsAdmin { get; set; }
}
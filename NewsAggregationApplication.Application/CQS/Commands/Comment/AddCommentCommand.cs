using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.CQS.Commands;

public class AddCommentCommand:IRequest<CommentDto>
{
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
}
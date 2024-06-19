using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.CQS.Queries.Comment;

public class GetCommentsByArticleIdAsyncQuery:IRequest<IEnumerable<CommentDto>>
{
    public Guid ArticleId { get; set; }
}
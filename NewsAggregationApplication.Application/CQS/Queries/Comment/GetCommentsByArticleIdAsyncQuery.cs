using MediatR;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.CQS.Queries.Comment;

public class GetCommentsByArticleIdAsyncQuery:IRequest<List<CommentDto>>
{
    public Guid ArticleId { get; set; }
}
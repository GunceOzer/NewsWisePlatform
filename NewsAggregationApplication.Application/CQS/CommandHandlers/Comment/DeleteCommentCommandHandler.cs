using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Comment;

public class DeleteCommentCommandHandler:IRequestHandler<DeleteCommentCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<DeleteCommentCommandHandler> _logger;

    public DeleteCommentCommandHandler(NewsDbContext dbContext, ILogger<DeleteCommentCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _dbContext.Comments.FindAsync(request.CommentId);
            if (comment == null || (comment.UserId != request.UserId && !request.IsAdmin))
            {
                _logger.LogWarning($"Attempt to delete non-existent or unauthorized comment: CommentID={request.CommentId}, UserID={request.UserId}, IsAdmin={request.IsAdmin}");
                return false;
            }

            _dbContext.Comments.Remove(comment);
            var result = await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Comment deleted successfully: CommentID={request.CommentId}");
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete comment: CommentID={request.CommentId}");
            return false;
        }
    }
}
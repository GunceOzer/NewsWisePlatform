using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Comment;

public class EditCommentCommandHandler:IRequestHandler<EditCommentCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<EditCommentCommandHandler> _logger;

    public EditCommentCommandHandler(ILogger<EditCommentCommandHandler> logger, NewsDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {   try
        {
            var comment = await _dbContext.Comments.FindAsync(new object[] {request.CommentId},cancellationToken);
            if (comment == null || (comment.UserId != request.UserId && !request.IsAdmin))
            {
                _logger.LogWarning($"Attempt to edit non-existent or unauthorized comment: CommentID={request.CommentId}, UserID={request.UserId}, IsAdmin={request.IsAdmin}");
                return false;
            }

            comment.Content = request.Content;
            _dbContext.Comments.Update(comment);
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation($"Comment edited successfully: CommentID={
                request.CommentId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to edit comment: CommentID={request.CommentId}");
            return false;
        }
    }
}
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
            var comment = await _dbContext.Comments.FindAsync(new object[] { request.CommentId }, cancellationToken);
            if (comment == null)
            {
                return false;
            }

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment.");
            return false;
        }
    }

   
}
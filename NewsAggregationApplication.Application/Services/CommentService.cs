using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.CQS.Commands;
using NewsAggregationApplication.UI.CQS.Queries.Comment;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class CommentService:ICommentService
{
   
    private readonly ILogger<CommentService> _logger;
    private readonly IMediator _mediator;

    public CommentService(ILogger<CommentService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

   
    
    public async Task<bool> AddCommentAsync(CommentDto commentDto)
    {
        try
        {
            var command = new AddCommentCommand{CommentDto = commentDto};
            return await _mediator.Send(command);
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment.");
            return false;
        }
    }
    

   public async Task<IEnumerable<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId)
    {

        try
        {
            return await _mediator.Send(new GetCommentsByArticleIdAsyncQuery { ArticleId = articleId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"Error occured while trying to get comments");
            return new List<CommentDto>();
        }
    }
   

    public async Task<bool> EditCommentAsync(CommentDto commentDto)
    {
        try
        {
            var command = new EditCommentCommand { CommentDto = commentDto };
            return await _mediator.Send(command);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error editing comment.");
            return false;
        }
    }

   
   
    public async Task<bool> DeleteCommentAsync(Guid commentId)
    {
        try
        {
            var command = new DeleteCommentCommand { CommentId = commentId };
            return await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment.");
            return false;
        }
    }
   
}
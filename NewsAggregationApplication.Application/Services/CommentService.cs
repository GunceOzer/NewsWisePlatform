using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.CQS.Commands;
using NewsAggregationApplication.UI.CQS.Queries.Comment;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.UI.Services;

public class CommentService:ICommentService
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<CommentService> _logger;
    private readonly CommentMapper _mapper;
    private readonly IMediator _mediator;

    public CommentService(NewsDbContext dbContext, ILogger<CommentService> logger, CommentMapper mapper, IMediator mediator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
        _mediator = mediator;
    }

   
    public async Task<CommentDto> AddCommentAsync(Guid articleId, Guid userId, string content,CancellationToken cancellationToken)
    {
        try
        {
            var addCommentCommand = new AddCommentCommand { ArticleId = articleId, UserId = userId, Content = content };
            return await _mediator.Send(addCommentCommand, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding comment");
            return null;
        }
       
    }

    public async Task<List<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId)
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

    
    

  
    public async Task<bool> EditCommentAsync(Guid commentId, Guid userId, string content, bool isAdmin)
    {
        try
        {
            var edit = new EditCommentCommand
                { CommentId = commentId, UserId = userId, Content = content, IsAdmin = isAdmin };
            return await _mediator.Send(edit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"Error occured while editing comment : CommentId:{commentId}");
            return false;
        }
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId, Guid userId, bool isAdmin)
    {

        try
        {
            var delete = new DeleteCommentCommand { CommentId = commentId, UserId = userId, IsAdmin = isAdmin };
            return await _mediator.Send(delete);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,$"Error occured while deleting the comment, commentID : {commentId}");
            return false;
        }
        
    }
   

   
}
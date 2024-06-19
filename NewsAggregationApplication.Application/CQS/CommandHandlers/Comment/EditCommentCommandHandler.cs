using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Comment;

public class EditCommentCommandHandler:IRequestHandler<EditCommentCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<EditCommentCommandHandler> _logger;
    private readonly CommentMapper _commentMapper;
    

    public EditCommentCommandHandler(ILogger<EditCommentCommandHandler> logger, NewsDbContext dbContext, CommentMapper commentMapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _commentMapper = commentMapper;
    }
    

  
    public async Task<bool> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _dbContext.Comments.FindAsync(new object[]
                { request.CommentDto.Id }, cancellationToken);
            if (comment == null)
            {
                return false;
            }

            _commentMapper.UpdateCommentFromDto(request.CommentDto, comment);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error editing comment");
            return false;
        }
    }
}
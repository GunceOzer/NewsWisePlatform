using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Comment;

public class AddCommentCommandHandler:IRequestHandler<AddCommentCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<AddCommentCommandHandler> _logger;
    private readonly CommentMapper _mapper;

    public AddCommentCommandHandler(NewsDbContext dbContext, ILogger<AddCommentCommandHandler> logger, CommentMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
    }

    


    public async Task<bool> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var comment = _mapper.CommentDtoToComment(request.CommentDto);
            await _dbContext.Comments.AddAsync(comment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add comment.");
            return false;
        }
    }
}
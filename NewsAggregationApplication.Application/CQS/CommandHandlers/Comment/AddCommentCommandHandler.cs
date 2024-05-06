using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Comment;

public class AddCommentCommandHandler:IRequestHandler<AddCommentCommand,CommentDto>
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

    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
       try
       {
           var user = await _dbContext.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
           if (user == null)
           {
               _logger.LogWarning("User not found for comment: UserID={UserId}", request.UserId);
               throw new InvalidOperationException("User not found.");
           }

           var comment = new Data.Entities.Comment
           {
               Id = Guid.NewGuid(),
               Content = request.Content,
               ArticleId = request.ArticleId,
               UserId = request.UserId,
               CreatedAt = DateTime.UtcNow,
               User = user
           };

           _dbContext.Comments.Add(comment);
           await _dbContext.SaveChangesAsync(cancellationToken);
           _logger.LogInformation("Comment added successfully: CommentID={CommentId}, ArticleID={ArticleId}, UserID={UserId}", comment.Id, request.ArticleId, request.UserId);

           return _mapper.CommentModelToCommentDto(comment);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Failed to add comment: ArticleID={ArticleId}, UserID={UserId}", request.ArticleId, request.UserId);
           throw;  
       }
    }
    
    
}
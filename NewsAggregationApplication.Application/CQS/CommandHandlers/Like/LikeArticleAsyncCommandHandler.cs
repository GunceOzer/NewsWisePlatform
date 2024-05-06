using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands.Like;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Like;

public class LikeArticleAsyncCommandHandler:IRequestHandler<LikeArticleAsyncCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<LikeArticleAsyncCommandHandler> _logger;

    public LikeArticleAsyncCommandHandler(NewsDbContext dbContext, ILogger<LikeArticleAsyncCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Handle(LikeArticleAsyncCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (await _dbContext.Likes.AnyAsync(l => l.ArticleId == request.ArticleId && l.UserId == request.UserId))
            {
                _logger.LogInformation("Attempt to like an already liked article: ArticleID={articleId}, UserID={userId}", request.ArticleId,request.UserId);
                return false;
            }
        
            var like = new Data.Entities.Like
            {
                Id = Guid.NewGuid(),
                ArticleId = request.ArticleId,
                UserId = request.UserId,
            };

            _dbContext.Likes.Add(like);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Article liked successfully: ArticleID={articleId}, UserID={userId}", request.ArticleId,request.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to like article: ArticleID={articleId}, UserID={userId}");
            return false;
        }
    }
}
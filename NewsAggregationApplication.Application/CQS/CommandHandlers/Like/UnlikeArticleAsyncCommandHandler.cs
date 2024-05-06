using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands.Like;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Like;

public class UnlikeArticleAsyncCommandHandler:IRequestHandler<UnlikeArticleAsyncCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<UnlikeArticleAsyncCommandHandler> _logger;

    public UnlikeArticleAsyncCommandHandler(NewsDbContext dbContext, ILogger<UnlikeArticleAsyncCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Handle(UnlikeArticleAsyncCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var like = await _dbContext.Likes.FirstOrDefaultAsync(l => l.ArticleId == request.ArticleId && l.UserId == request.UserId);
            if (like == null)
            {
                _logger.LogInformation("Attempt to unlike an article that was not previously liked: ArticleID={articleId}, UserID={userId}",request.ArticleId,request.UserId);
                return false;
            }

            _dbContext.Likes.Remove(like);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Article unliked successfully: ArticleID={articleId}, UserID={userId}",request.ArticleId,request.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unlike article: ArticleID={articleId}, UserID={userId}",request.ArticleId,request.UserId);
            return false;
        }
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.UI.CQS.Commands.Like;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.UI.CQS.CommandHandlers.Like;

public class LikeArticleAsyncCommandHandler:IRequestHandler<LikeArticleAsyncCommand,bool>
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<LikeArticleAsyncCommandHandler> _logger;
    private readonly LikeMapper _mapper;

    public LikeArticleAsyncCommandHandler(NewsDbContext dbContext, ILogger<LikeArticleAsyncCommandHandler> logger, LikeMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
    }

    
    public async Task<bool> Handle(LikeArticleAsyncCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingLike = await _dbContext.Likes.FirstOrDefaultAsync(l =>
                l.ArticleId == request.LikeDto.ArticleId && l.UserId == request.LikeDto.UserId);
            if (existingLike != null)
            {
                _logger.LogInformation(
                    "Attempt to like an already liked article: ArticleID={articleId}, UserID={userId}",
                    request.LikeDto.ArticleId, request.LikeDto.UserId);
                return false;
            }

            var like = _mapper.LikeDtoToLike(request.LikeDto);
            _dbContext.Likes.Add(like);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liking the article.");
            return false;
        }
    }
}
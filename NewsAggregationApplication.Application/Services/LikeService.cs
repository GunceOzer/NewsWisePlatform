using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.CQS.Commands.Like;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.UI.Services;

public class LikeService:ILikeService
{
    
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<LikeService> _logger;
    private readonly LikeMapper _likeMapper;
    private readonly IMediator _mediator;

    public LikeService(NewsDbContext dbContext, ILogger<LikeService> logger, LikeMapper likeMapper, IMediator mediator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _likeMapper = likeMapper;
        _mediator = mediator;
    }

    public async Task<bool> LikeArticleAsync(Guid articleId, Guid userId)
    {
        try
        {
            var like = new LikeArticleAsyncCommand { ArticleId = articleId, UserId = userId };
            return await _mediator.Send(like);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"Error occured while like the article , articleId :{articleId}");
            return false;
        }
       
    }
    /*public async Task<bool> LikeArticleAsync(LikeDto likeDto)
    {
        try
        {
            if (_dbContext.Likes.Any(l => l.ArticleId == likeDto.ArticleId && l.UserId == likeDto.UserId))
            {
                _logger.LogInformation("Attempt to like an already liked article: ArticleID={ArticleId}, UserID={UserId}", likeDto.ArticleId, likeDto.UserId);
                return false;
            }

            var like = _likeMapper.LikeDtoToLikeModel(likeDto); // Assuming you need to convert DTO to entity model here

            _dbContext.Likes.Add(like);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Article liked successfully: ArticleID={ArticleId}, UserID={UserId}", likeDto.ArticleId, likeDto.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to like article: ArticleID={ArticleId}, UserID={UserId}", likeDto.ArticleId, likeDto.UserId);
            return false;
        }
    }*/


    public async Task<bool> UnlikeArticleAsync(Guid articleId, Guid userId)
    {
        try
        {
            var unlike = new UnlikeArticleAsyncCommand { ArticleId = articleId, UserId = userId };
            return await _mediator.Send(unlike);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"Error occured while unlike the article: {articleId}");
            return false;
        }
    }
}
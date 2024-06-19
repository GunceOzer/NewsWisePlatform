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
    
    private readonly ILogger<LikeService> _logger;
    private readonly IMediator _mediator;

    public LikeService(ILogger<LikeService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    

    public async Task<bool> LikeArticleAsync(LikeDto likeDto)
    {
        try
        {
            var like = new LikeArticleAsyncCommand { LikeDto = likeDto };
            return await _mediator.Send(like);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error liking article.");
            return false;
        }
    }

    public async Task<bool> UnlikeArticleAsync(LikeDto likeDto)
    {
        try
        {
            var unlike = new UnlikeArticleAsyncCommand {LikeDto = likeDto };
            return await _mediator.Send(unlike);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,$"Error occured while unlike the article: {likeDto.ArticleId} by user: {likeDto.UserId}");
            return false;
        }
    }
}
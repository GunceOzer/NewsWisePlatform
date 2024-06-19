using Castle.Core.Logging;
using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.CQS.Commands.Like;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Services;
using NSubstitute;

namespace NewsAggregationApplication.Application.Tests;

public class LikeServiceTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ILogger<LikeService> _logger = Substitute.For<ILogger<LikeService>>();
    private readonly ILikeService _service;

    public LikeServiceTests()
    {
        _service = new LikeService(_logger, _mediator);
    }
    
    [Fact]
    public async Task LikeArticleAsync_ReturnsTrue_WhenSuccessful()
    {
        //Arrange
        var likeDto = new LikeDto{ ArticleId = Guid.NewGuid(), UserId = Guid.NewGuid()};
        _mediator.Send(Arg.Any<LikeArticleAsyncCommand>()).Returns(true);
        
        //Act
        var result = await _service.LikeArticleAsync(likeDto);
        
        //Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task LikeArticleAsync_ReturnsFalse_WhenAlreadyLiked()
    {
        var likeDto = new LikeDto{ArticleId = Guid.NewGuid(), UserId = Guid.NewGuid()}; 
        _mediator.Send(Arg.Any<LikeArticleAsyncCommand>()).Returns(false);

        var result = await _service.LikeArticleAsync(likeDto);

        Assert.False(result);
    }
    
    [Fact]
    public async Task LikeArticleAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        var likeDto = new LikeDto{ArticleId = Guid.NewGuid(), UserId = Guid.NewGuid()}; 
        _mediator.Send(Arg.Any<LikeArticleAsyncCommand>())
            .Returns(Task.FromException<bool>(new Exception("Simulated error")));

        var result = await _service.LikeArticleAsync(likeDto);

        Assert.False(result);
    }

    [Fact]
    public async Task UnlikeArticleAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var likeDto = new LikeDto{ArticleId = Guid.NewGuid(), UserId = Guid.NewGuid()}; 
        _mediator.Send(Arg.Any<UnlikeArticleAsyncCommand>()).Returns(true);

        // Act
        var result = await _service.UnlikeArticleAsync(likeDto);

        // Assert
        Assert.True(result);
       
    }

    [Fact]
    public async Task UnlikeArticleAsync_ReturnsFalse_WhenNotLikedPreviously()
    {
        var likeDto = new LikeDto{ArticleId = Guid.NewGuid(), UserId = Guid.NewGuid()}; 
        _mediator.Send(Arg.Any<UnlikeArticleAsyncCommand>()).Returns(false);

        var result = await _service.UnlikeArticleAsync(likeDto);

        Assert.False(result);
    }

    [Fact]
    public async Task UnlikeArticleAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        var likeDto = new LikeDto{ArticleId = Guid.NewGuid(), UserId = Guid.NewGuid()}; 
        _mediator.Send(Arg.Any<UnlikeArticleAsyncCommand>())
            .Returns(Task.FromException<bool>(new Exception("Simulated error")));

        var result = await _service.UnlikeArticleAsync(likeDto);

        Assert.False(result);
    }

}
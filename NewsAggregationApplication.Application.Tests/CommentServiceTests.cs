using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.Commands.Article;
using NewsAggregationApplication.UI.CQS.Commands;
using NewsAggregationApplication.UI.CQS.Queries.Comment;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Services;
using NSubstitute;

namespace NewsAggregationApplication.Application.Tests;

public class CommentServiceTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ILogger<CommentService> _logger = Substitute.For<ILogger<CommentService>>();
    private readonly ICommentService _service;

    public CommentServiceTests()
    {
        _service = new CommentService(_logger, _mediator);
    }


    [Fact]
    public async Task AddComment_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var commentDto = new CommentDto { Id = Guid.NewGuid(), Content = "Nice article" };
        _mediator.Send(Arg.Any<AddCommentCommand>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _service.AddCommentAsync(commentDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddCommentAsync_ReturnsNull_WhenExceptionOccurs()
    {
        //Arrange
        var commentDto = new CommentDto { Id = Guid.NewGuid(), Content = "Nice article" };
        _mediator.Send(Arg.Any<AddCommentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("simulated error")));
        
        //Act
        var result = await _service.AddCommentAsync(commentDto);

        //Assert
        Assert.False(result);
    }

    /*[Fact]
    public async Task GetCommentsByArticleIdAsync_ReturnsComments_WhenSuccessful()
    {
        //Arrange 
        var articleId = Guid.NewGuid();
        var comments = new List<CommentDto>
        {
            new CommentDto
            {
                Id = Guid.NewGuid(),
                Content = "nice article"
            }
        };
        _mediator.Send(Arg.Any<GetCommentsByArticleIdAsyncQuery>()).Returns(comments);

        //Act
        var result = await _service.GetCommentsByArticleIdAsync(articleId);
        //Assert
        Assert.NotEmpty(result);
        Assert.Equal(comments.Count, result.Count);
    }*/
    [Fact]
    public async Task GetCommentsByArticleIdAsync_ReturnsComments_WhenSuccessful()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        var comments = new List<CommentDto>
        {
            new CommentDto { Id = Guid.NewGuid(), Content = "Interesting article" }
        };
        _mediator.Send(Arg.Any<GetCommentsByArticleIdAsyncQuery>()).Returns(comments);

        // Act
        var result = await _service.GetCommentsByArticleIdAsync(articleId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(comments.Count, ((List<CommentDto>)result).Count);
    }

    [Fact]
    public async Task GetCommentsByArticleIdAsync_ReturnsEmpty_WhenExceptionOccurs()
    {
        //Arrange
        var articleId = Guid.NewGuid();
        _mediator.Send(Arg.Any<GetCommentsByArticleIdAsyncQuery>())
            .Returns(Task.FromException<IEnumerable<CommentDto>>(new Exception("Simulated error!")));
        
        //Act
        var result = await _service.GetCommentsByArticleIdAsync(articleId);
        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task EditCommentAsync_ReturnsTrue_WhenSuccessful()
    {
        //Arrange
        var commentDto = new CommentDto { Id = Guid.NewGuid(), Content = "updated comment" };
        _mediator.Send(Arg.Any<EditCommentCommand>())
            .Returns(true);

        //Act
        var result = await _service.EditCommentAsync(commentDto);
        //Assert
        Assert.True(result);
        
    }

    [Fact]
    public async Task EditCommentAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        //Arrange
        var commentDto = new CommentDto { Id = Guid.NewGuid(), Content = "updated comment" };

        _mediator.Send(Arg.Any<EditCommentCommand>())
            .Returns(Task.FromException<bool>(new Exception("Simulated error!")));
        
        //Act
        var result = await _service.EditCommentAsync(commentDto);
        
        //Assert
        Assert.False(result);

    }

    [Fact]
    public async Task DeleteCommentAsync_ReturnsTrue_WhenUserDeletesOwnComment()
    {
        var commentId = Guid.NewGuid();
        
        _mediator.Send(Arg.Any<DeleteCommentCommand>())
            .Returns(true);

        var result = await _service.DeleteCommentAsync(commentId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCommentAsync_ReturnsFalse_WhenUserTriesToDeleteOthersComment()
    {
        var commentId = Guid.NewGuid();
        
        _mediator.Send(Arg.Any<DeleteCommentCommand>())
            .Returns(false);

        var result = await _service.DeleteCommentAsync(commentId);

        Assert.False(result);
    }
    
    [Fact]
    public async Task DeleteCommentAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        var commentId = Guid.NewGuid();
        
        _mediator.Send(Arg.Any<DeleteCommentCommand>())
            .Returns(Task.FromException<bool>(new Exception("Simulated error")));

        var result = await _service.DeleteCommentAsync(commentId);

        Assert.False(result);
    }

}
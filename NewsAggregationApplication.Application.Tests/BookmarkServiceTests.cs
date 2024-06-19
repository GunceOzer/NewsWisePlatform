using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.CQS.Commands.Bookmark;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Queries.Article;
using NewsAggregationApplication.UI.Services;
using NSubstitute;

namespace NewsAggregationApplication.Application.Tests;

public class BookmarkServiceTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ILogger<BookmarkService> _logger = Substitute.For<ILogger<BookmarkService>>();
    private readonly IBookmarkService _service;

    public BookmarkServiceTests()
    {
        _service = new BookmarkService( _logger, _mediator);
    }

    
    [Fact]
    public async Task BookmarkArticleAsync_ReturnsTrue_WhenSuccessful()
    {
        var bookmarkDto = new BookmarkDto()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ArticleId = Guid.NewGuid()
        };
        _mediator.Send(Arg.Any<BookmarkArticlesCommand>()).Returns(true);
        
        var result = await _service.BookmarkArticleAsync(bookmarkDto);

        Assert.True(result);
    }


    [Fact]
    public async Task BookmarkArticleAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        //Arrange
        var bookmarkDto = new BookmarkDto()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ArticleId = Guid.NewGuid()
        };
        _mediator.Send(Arg.Any<BookmarkArticlesCommand>())
            .Returns(Task.FromException<bool>(new Exception("Simulated error")));
        
        //Act
        var result = await _service.BookmarkArticleAsync(bookmarkDto);
        
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveBookmarkAsync_ReturnsTrue_WhenSuccessful()
    {
        //Arrange 
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _mediator.Send(Arg.Any<RemoveBookmarkCommand>()).Returns(true);
        
        //Act
        var result = await _service.RemoveBookmarkAsync(articleId, userId);
        
        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveBookmarkAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        //Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _mediator.Send(Arg.Any<RemoveBookmarkCommand>())
            .Returns(Task.FromException<bool>(new Exception("Similation error")));
        
        //Act
        var result = await _service.RemoveBookmarkAsync(articleId, userId);
        
        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetBookmarkArticlesAsync_ReturnsArticles_WhenSuccessful()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var expectedArticles = new List<ArticleDto> { new ArticleDto() { Id = Guid.NewGuid() } };
        _mediator.Send(Arg.Any<GetBookmarkedArticlesQuery>())
            .Returns(expectedArticles);
        //Act
        var result = await _service.GetBookmarkArticlesAsync(userId);
        
        //Assert
        Assert.Equal(expectedArticles.Count,result.Count());

    }

    [Fact]
    public async Task GetBookmarkArticlesAsync_ReturnsEmpty_WhenExceptionOccurs()
    {
        //Arrange
        var userId = Guid.NewGuid();
        _mediator.Send(Arg.Any<GetBookmarkedArticlesQuery>())
            .Returns(Task.FromException<IEnumerable<ArticleDto>>(new Exception("Simulation error")));
        //Act
        var result = await _service.GetBookmarkArticlesAsync(userId);
        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task IsArticleBookmarkedByUser_ReturnsTrue_IfBookmarked()
    {
        //Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _mediator.Send(Arg.Any<IsArticleBookmarkedByUserQuery>())
            .Returns(true);
        //Act
        var result = await _service.IsArticleBookmarkedByUser(articleId, userId);
        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsArticleBookmarkedByUser_ReturnsFalse_IfNotBookmarked()
    {
        //Arrange
        var articleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _mediator.Send(Arg.Any<IsArticleBookmarkedByUserQuery>()).Returns(false);
        //Act
        var result = await _service.IsArticleBookmarkedByUser(articleId, userId);
        //Assert
        Assert.False(result);
    }
}
using MediatR;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.UI.Commands.Article;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Queries.Article;
using NewsAggregationApplication.UI.Services;

namespace NewsAggregationApplication.Application.Tests;

using Xunit;
using NSubstitute;

public class ArticleServiceTests
{
    private readonly IArticleService _service;
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly ILogger<ArticleService> _logger = Substitute.For<ILogger<ArticleService>>();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ArticleServiceTests()
    {
        _service = new ArticleService(_logger, _mediator);
    }

    
    [Fact]
    public async Task DeleteArticleAsync_ReturnsFalse_WhenCommandFails()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        _mediator.Send(Arg.Any<DeleteArticleCommand>())
            .Returns(Task.FromException<bool>(new Exception("Simulated exception")));

        // Act
        var result = await _service.DeleteArticleAsync(articleId);

        // Assert
        Assert.False(result);
        _logger.Received().LogError(Arg.Any<Exception>(), $"Error deleting article with ID: {articleId}.");
    }

    [Fact]
    public async Task DeleteArticleAsync_ReturnsTrue_WhenArticleDeleted()
    {
        // Arrange
        var articleId = Guid.NewGuid();
        _mediator.Send(Arg.Is<DeleteArticleCommand>(cmd => cmd.ArticleId == articleId)).Returns(true);

        // Act
        var result = await _service.DeleteArticleAsync(articleId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetArticlesAsync_ReturnsArticles_WhenSuccessful()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedArticles = new List<ArticleDto>
        {
            new ArticleDto { Id = Guid.NewGuid(), Title = "Article 1" },
            new ArticleDto { Id = Guid.NewGuid(), Title = "Article 2" }
        };

        _mediator.Send(Arg.Is<GetArticlesQuery>(q => q.UserId == userId))
            .Returns(expectedArticles);

        // Act
        var articles = await _service.GetArticlesAsync(userId);

        // Assert
        Assert.NotNull(articles);
        Assert.Equal(expectedArticles.Count, articles.Count());
        _logger.Received().LogInformation($"Retrieved {expectedArticles.Count()} articles.");
    }

    [Fact]
    public async Task GetArticlesAsync_ReturnsEmptyList_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mediator.Send(Arg.Any<GetArticlesQuery>())
            .Returns(Task.FromException<IEnumerable<ArticleDto>>(new Exception("Simulated database failure")));

        // Act
        var result = await _service.GetArticlesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _logger.Received().LogError(Arg.Any<Exception>(), "Failed to retrieve articles.");
    }
}

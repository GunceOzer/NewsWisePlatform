using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArticlesController : Controller
{
    
    private readonly IArticleService _articleService;
    private readonly ArticleMapper _articleMapper;
    private readonly CommentMapper _commentMapper;
    private readonly ILogger<ArticlesController> _logger;
    private readonly ICommentService _commentService;


    public ArticlesController(IArticleService articleService, ArticleMapper articleMapper,ILogger<ArticlesController> logger, CommentMapper commentMapper, ICommentService commentService)
    {
        _articleService = articleService;
        _articleMapper = articleMapper;
        _logger = logger;
        _commentMapper = commentMapper;
        _commentService = commentService;
    }
    
    
    /// <summary>
    /// Get all articles
    /// </summary>
    /// <returns>A list of articles</returns>
    /// <response code="200">Returns the list of articles.</response>
    /// <response code="404">If no articles are found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticles()
    {
        var userId = User.Identity.IsAuthenticated ? Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value) : (Guid?)null;
        var articles = await _articleService.GetArticlesAsync(userId);

        if (articles == null)
        {
            return NotFound();
        }

        return Ok(articles);
    }

    /// <summary>
    /// Get an article by id
    /// </summary>
    /// /// <param name="id">The ID of the article.</param>
    /// <returns>The article details.</returns>
    /// <response code="200">Returns the article details.</response>
    /// <response code="404">If the article is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticleById(Guid id)
    {
        var userId = User.Identity.IsAuthenticated ? Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value) : (Guid?)null;
        var article = await _articleService.GetArticlesByIdAsync(id, userId ?? Guid.Empty);

        if (article == null)
        {
            return NotFound();
        }

        return Ok(article);
    }
    
    /// <summary>
    /// Aggregates articles from RSS feeds. Only accessible to Admin users.
    /// </summary>
    /// <returns>A message indicating the result of the aggregation.</returns>
    /// <response code="200">If the aggregation is successful.</response>
    /// <response code="500">If an error occurs during aggregation.</response>
    [HttpPost("aggregate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AggregateArticles()
    {
        _logger.LogInformation("AggregateArticles endpoint hit.");
        try
        {
            await _articleService.AggregateFromSourceAsync(CancellationToken.None);
            return Ok(new { Message = "RSS feeds successfully aggregated." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to aggregate RSS feeds.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to aggregate RSS feeds. Check logs for details.");
        }
    }
    
    /// <summary>
    /// Deletes a specific article by its ID. Only accessible to Admin users.
    /// </summary>
    /// <param name="id">The ID of the article to delete.</param>
    /// <returns>A message indicating the result of the deletion.</returns>
    /// <response code="200">If the deletion is successful.</response>
    /// <response code="404">If the article is not found.</response>
    /// <response code="500">If an error occurs during deletion.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteArticle(Guid id)
    {
        try
        {
            var deleted = await _articleService.DeleteArticleAsync(id);
            if (!deleted)
            {
                _logger.LogWarning($"Article with ID: {id} not found.");
                return NotFound();
            }
            _logger.LogInformation($"Article with ID: {id} deleted successfully.");
            return Ok(new { Message = $"Article with ID: {id} deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting article with ID: {id}.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting article.");
        }
    }
    
    /// <summary>
    /// Retrieves the details of an article along with its comments.
    /// </summary>
    /// <param name="id">The ID of the article.</param>
    /// <returns>The article details and comments.</returns>
    /// <response code="200">Returns the article details and comments.</response>
    /// <response code="404">If the article is not found.</response>
    /// <response code="500">If an error occurs while retrieving the article details.</response>
    [HttpGet("{id}/details")]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetArticleDetails(Guid id)
    {
        try
        {
            Guid userId = User.Identity.IsAuthenticated ? Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value) : Guid.Empty;

            var articleDto = await _articleService.GetArticlesByIdAsync(id, userId);
            if (articleDto == null)
            {
                _logger.LogWarning($"Article with ID: {id} not found.");
                return NotFound();
            }

            var article = _articleMapper.ArticleDtoToArticleModel(articleDto);
            var comments = await _commentService.GetCommentsByArticleIdAsync(id);
            article.Comments = comments.Select(_commentMapper.CommentDtoToCommentModel).ToList();

            return Ok(article);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to retrieve details for article with ID: {id}.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve article details.");
        }
    }
    
    /// <summary>
    /// Retrieves articles sorted by their positivity score only authorized users can see .
    /// </summary>
    /// <param name="sortByPositive">If true, sorts by positive sentiment; otherwise, sorts by negative sentiment.</param>
    /// <returns>A list of sorted articles by their positivity.</returns>
    /// <response code="200">Returns the list of sorted articles.</response>
    /// <response code="404">If no articles are found.</response>
    [Authorize]
    [HttpGet("sortedByPositivity")]
    [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSortedArticlesByPositivity([FromQuery] bool sortByPositive = true)
    {
        var userId = User.Identity.IsAuthenticated
            ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
            : (Guid?)null;
        var articles = await _articleService.GetSortedArticlesByPositivityAsync(userId, sortByPositive);
        if (articles == null || !articles.Any())
        {
            return NotFound();
        }

        return Ok(articles);
        
    }
    /// <summary>
    /// Retrieves articles sorted by their publication date.
    /// </summary>
    /// <param name="sortByNewest">If true, sorts by newest first; otherwise, sorts by oldest first.</param>
    /// <returns>A list of sorted articles by their publication date.</returns>
    /// <response code="200">Returns the list of sorted articles.</response>
    /// <response code="404">If no articles are found.</response>
    [HttpGet("sortedByNewest")]
    [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSortedArticlesByNewest([FromQuery] bool sortByNewest = true)
    {
        var userId = User.Identity.IsAuthenticated
            ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
            : (Guid?)null;
        var articles = await _articleService.GetSortedArticlesByDate(userId, sortByNewest);
        if (articles == null || !articles.Any())
        {
            return NotFound();
        }

        return Ok(articles);
        
    }

}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookmarksController : Controller
{
    
    private readonly IBookmarkService _bookmarkService;
    private readonly ILogger<BookmarksController> _logger;
    private readonly BookmarkMapper _bookmarkMapper;
    

    public BookmarksController(IBookmarkService bookmarkService, ILogger<BookmarksController> logger, BookmarkMapper bookmarkMapper)
    {
        _bookmarkService = bookmarkService;
        _logger = logger;
        _bookmarkMapper = bookmarkMapper;
    }
    
    /// <summary>
    /// Get all bookmarked articles for the logged-in user
    /// </summary>
    /// <returns>A list of bookmarked articles.</returns>
    /// <response code="200">Returns a list of bookmarked articles.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBookmarks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User ID is null in GetBookmarks method of BookmarkController.");
            return Unauthorized(new { Message = "User is not authorized" });
        }

        try
        {
            var articleDtos = await _bookmarkService.GetBookmarkArticlesAsync(Guid.Parse(userId));
            return Ok(articleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch bookmarked articles.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to fetch bookmarked articles" });
        }
    }
    /// <summary>
    /// Bookmark an article
    /// </summary>
    /// <param name="articleId">The ID of the article to bookmark.</param>
    /// <returns>Result of the bookmark operation.</returns>
    /// <response code="200">If the article is bookmarked successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpPost("{articleId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BookmarkArticle(Guid articleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "User is not authorized" });

        var bookmarkDto = new BookmarkDto
        {
            Id = Guid.NewGuid(),
            ArticleId = articleId,
            UserId = Guid.Parse(userId),
        };
        try
        {
            //new part 
            var bookmark = _bookmarkMapper.BookmarkDtoToBookmark(bookmarkDto);
            var result = await _bookmarkService.BookmarkArticleAsync(bookmarkDto);
            if (result)
            {
                _logger.LogInformation($"Article {articleId} bookmarked by user {userId}.");
                return Ok(new { Message = "Article bookmarked successfully" });
            }

            return BadRequest(new { Message = "Failed to bookmark article" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error bookmarking article {articleId} by user {userId}.");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred while bookmarking the article" });
        }
    }

        /// <summary>
        /// Remove bookmark from an article
        /// </summary>
        /// <param name="articleId">The ID of the article to remove the bookmark from.</param>
        /// <returns>Result of the remove bookmark operation.</returns>
        /// <response code="200">If the bookmark is removed successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpDelete("{articleId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveBookmark(Guid articleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User is not authorized" });
            }

            try
            {
                var result = await _bookmarkService.RemoveBookmarkAsync(articleId, Guid.Parse(userId));
                if (result)
                {
                    _logger.LogInformation($"Bookmark removed for article {articleId} by user {userId}.");
                    return Ok(new { Message = "Bookmark removed successfully" });
                }
                return BadRequest(new { Message = "Failed to remove bookmark" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing bookmark for article {articleId} by user {userId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while removing the bookmark" });
            }
        }

        /// <summary>
        /// Toggle bookmark for an article
        /// </summary>
        /// <param name="articleId">The ID of the article to toggle the bookmark for.</param>
        /// <returns>Result of the toggle bookmark operation.</returns>
        /// <response code="200">If the bookmark is toggled successfully.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPost("toggle/{articleId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleBookmark(Guid articleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User is not authorized" });
            }

            try
            {
                var isBookmarked = await _bookmarkService.IsArticleBookmarkedByUser(articleId, Guid.Parse(userId));
                if (isBookmarked)
                {
                    await _bookmarkService.RemoveBookmarkAsync(articleId, Guid.Parse(userId));
                    _logger.LogInformation($"Bookmark removed for article {articleId}.");
                    return Ok(new { Message = "Bookmark removed" });
                }
                else
                {
                   // await _bookmarkService.BookmarkArticleAsync(articleId, Guid.Parse(userId));
                   var bookmarkDto = new BookmarkDto()
                   {
                       Id = Guid.NewGuid(),
                       UserId = Guid.Parse(userId),
                       ArticleId = articleId,
                       IsBookmarked = isBookmarked
                   };
                   await _bookmarkService.BookmarkArticleAsync(bookmarkDto);
                    _logger.LogInformation($"Article {articleId} bookmarked.");
                    return Ok(new { Message = "Article bookmarked" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling bookmark for article {articleId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while toggling the bookmark" });
            }
        }

    
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Models;
using NewsAggregationApplication.UI.Services;

namespace NewsAggregationApplication.UI.Controllers;

public class BookmarkController : Controller
{
    private readonly IBookmarkService _bookmarkService;
    private readonly ILogger<BookmarkService> _logger;
    private readonly BookmarkMapper _bookmarkMapper;
    private readonly ArticleMapper _articleMapper;
    

    public BookmarkController(IBookmarkService bookmarkService, ILogger<BookmarkService> logger, BookmarkMapper bookmarkMapper, ArticleMapper articleMapper)
    {
        _bookmarkService = bookmarkService;
        _logger = logger;
        _bookmarkMapper = bookmarkMapper;
        _articleMapper = articleMapper;
    }
    
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("User ID is null in Index method of BookmarkController.");
            return Unauthorized();
        }

        try
        {
            var articleDtos = await _bookmarkService.GetBookmarkArticlesAsync(Guid.Parse(userId));
            var articleModels = articleDtos.Select(adto => _articleMapper.ArticleDtoToArticleModel(adto)).ToList();
            return View(articleModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch bookmarked articles.");
            return View(new List<ArticleModel>()); // Return an empty view in case of failure
        }
    }

    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Bookmark(Guid articleId)
    {
       
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _bookmarkService.BookmarkArticleAsync(articleId, Guid.Parse(userId));
            if (result)
            {
                _logger.LogInformation($"Article {articleId} bookmarked by user {userId}.");
            }
            else
            {
                _logger.LogWarning($"Article {articleId} already bookmarked by user {userId}.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error bookmarking article {articleId} by user {userId}.");
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveBookmark(Guid articleId)
    {
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _bookmarkService.RemoveBookmarkAsync(articleId, Guid.Parse(userId));
            if (result)
            {
                _logger.LogInformation($"Bookmark removed for article {articleId} by user {userId}.");
            }
            else
            {
                _logger.LogWarning($"Failed to remove bookmark for article {articleId} by user {userId}.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error removing bookmark for article {articleId} by user {userId}.");
        }

        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ToggleBookmark(Guid articleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var isBookmarked = await _bookmarkService.IsArticleBookmarkedByUser(articleId, Guid.Parse(userId));
            if (isBookmarked)
            {
                await _bookmarkService.RemoveBookmarkAsync(articleId, Guid.Parse(userId));
                _logger.LogInformation($"Bookmark removed for article {articleId}.");
            }
            else
            {
                await _bookmarkService.BookmarkArticleAsync(articleId, Guid.Parse(userId));
                _logger.LogInformation($"Article {articleId} bookmarked.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error toggling bookmark for article {articleId}.");
        }

        return RedirectToAction("Index", "Article"); 
    }
    
    
}
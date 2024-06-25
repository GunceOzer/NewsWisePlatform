using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.DTOs;
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
            TempData["ErrorMessage"] = "Failed to fetch bookmarked articles.";
            return View(new List<ArticleModel>()); // Return an empty view in case of failure
        }
    }

    
   
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ToggleBookmark(Guid articleId, string returnUrl)
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
                TempData["SuccessMessage"] = "Bookmark removed successfully.";

            }
            else
            {
                var bookmarkDto = new BookmarkDto
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userId),
                    ArticleId = articleId,
                    IsBookmarked = isBookmarked
                };
                await _bookmarkService.BookmarkArticleAsync(bookmarkDto);
                _logger.LogInformation($"Article {articleId} bookmarked.");
               

            }

            return Redirect(returnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error toggling bookmark for article {articleId}.");
            TempData["ErrorMessage"] = "An error occurred while toggling the bookmark.";
            return Redirect(returnUrl);
        }
    }

   
}
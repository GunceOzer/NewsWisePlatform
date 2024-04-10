using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Models;
using NewsAggregationApplication.UI.Services;

namespace NewsAggregationApplication.UI.Controllers;

public class BookmarkController : Controller
{
    private readonly IBookmarkService _bookmarkService;
    private readonly ILogger<BookmarkService> _logger;

    public BookmarkController(IBookmarkService bookmarkService, ILogger<BookmarkService> logger)
    {
        _bookmarkService = bookmarkService;
        _logger = logger;
    }

    // GET
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogError("User ID is null in Index method of BookmarkController.");
            return Unauthorized();
        }

        var articles = await _bookmarkService.GetBookmarkArticlesAsync(Guid.Parse(userId));
        if (articles == null)
        {
            _logger.LogError($"GetBookmarkArticlesAsync returned null for user ID {userId}.");
            return View(new List<ArticleModel>());
        }
        
        var articleModels = articles.Select(article => new ArticleModel
        {
            Id = article.Id,
            Title = article.Title,
            Description = article.Description,
            Content = article.Content,
            PublicationDate = article.PublishedDate,
            SourceUrl = article.SourceUrl,
            LikesCount = article.Likes?.Count?? 0,
            IsBookmarked = true,
            Comments = article.Comments.Select(c => new CommentViewModel
            {
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                //Username = c.User.UserName

            }).ToList()
        }).ToList();
        
        return View(articleModels);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Bookmark(Guid articleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        await _bookmarkService.BookmarkArticleAsync(articleId, Guid.Parse(userId));
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveBookmark(Guid articleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        await _bookmarkService.RemoveBookmarkAsync(articleId, Guid.Parse(userId));
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ToggleBookmark(Guid articleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }
    
        // if the article is already bookmarked by the user
        var isBookmarked = await _bookmarkService.IsArticleBookmarkedByUser(articleId, Guid.Parse(userId));

        if (isBookmarked)
        {
            await _bookmarkService.RemoveBookmarkAsync(articleId, Guid.Parse(userId));
        }
        else
        {
            await _bookmarkService.BookmarkArticleAsync(articleId, Guid.Parse(userId));
        }

        return RedirectToAction("Index", "Article");//, new { id = articleId }
    }
    
}
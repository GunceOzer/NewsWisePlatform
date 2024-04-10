using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.UI.Controllers;

public class ArticleController : Controller
{
    private readonly IArticleService _articleService;
    private readonly IBookmarkService _bookmarkService;

    public ArticleController(IArticleService articleService, IBookmarkService bookmarkService)
    {
        _articleService = articleService;
        _bookmarkService = bookmarkService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var articles = (await _articleService.GetArticlesAsync()).Select(article => new ArticleModel()
        {
            Id = article.Id,
            Description = article.Description,
            Content = article.Content,
            SourceUrl = article.SourceUrl,
            PublicationDate = article.PublishedDate,
            Title = article.Title,
            LikesCount = article.Likes?.Count

        }).ToList();
        return View(articles);
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Aggregate()
    {
       
        var rssLink = @"https://www.pcgamesn.com/mainrss.xml";
        await _articleService.AggregateFromSourceAsync(rssLink); 
        return RedirectToAction("Index");
        
    }

    [HttpPost]
    public async Task<IActionResult> DeleteArticle(Guid id)
    {
        bool deleted = await _articleService.DeleteArticleAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var article = await _articleService.GetArticlesByIdAsync(id);
        if (article == null)
        {
            return NotFound();
        }

        var model = new ArticleModel
        {
            Id = article.Id,
            Title = article.Title,
            Description = article.Description,
            Content = article.Content,
            LikesCount = article.Likes?.Count??0,
            SourceUrl = article.SourceUrl,
            PublicationDate = article.PublishedDate,
            Comments = article.Comments.Select(c => new CommentViewModel
            {
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                Username = c.User.UserName // Ensure the user data is included and accessible
            }).ToList()
        };

        return View(model);
    }
    

    // [HttpPost]
    // public async Task<IActionResult> Like(Guid articleId)
    // {
    //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //getting user id from claims
    //     await _articleService.LikeArticleAsync(articleId, new Guid(userId));
    //     return RedirectToAction("Index");
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> Unlike(Guid articleId)
    // {
    //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     await _articleService.UnlikeArticleAsync(articleId, new Guid(userId));
    //     return RedirectToAction("Index");
    // }
    
    
    /*[Authorize]
    [HttpPost]
    public async Task<IActionResult> Like(LikeModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        model.UserId = new Guid(userId);
        await _articleService.LikeArticleAsync(model.ArticleId, model.UserId);
        
        return RedirectToAction(nameof(Index));
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Unlike(LikeModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        model.UserId = new Guid(userId);
        await _articleService.UnlikeArticleAsync(model.ArticleId, model.UserId);
        
        
        return RedirectToAction(nameof(Index));
    }*/
    
    /*[Authorize]
    [HttpPost]
    public async Task<IActionResult> Bookmark(Guid articleId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        await _articleService.BookmarkArticleAsync(articleId, Guid.Parse(userId));
        return RedirectToAction(nameof(Index));
    }*/

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment(CommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var commentDto = new CommentDTO
        {
            Content = model.Content,
            ArticleId = model.ArticleId
        };

        await _articleService.AddCommentAsync(commentDto, Guid.Parse(userId));

        return RedirectToAction("Details", "Article", new { id = model.ArticleId });
    }
    
}
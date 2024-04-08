using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.UI.Controllers;

public class ArticleController : Controller
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
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
    public async Task<IActionResult> Aggregate()
    {
       
        var rssLink = @"https://www.pcgamesn.com/mainrss.xml";
        await _articleService.AggregateFromSourceAsync(rssLink); 
        return RedirectToAction("Index");
        
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteArticle(Guid id)
    {
        bool deleted = await _articleService.DeleteArticleAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
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
    
    
    [Authorize]
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

        await _articleService.BookmarkArticleAsync(articleId, Guid.Parse(userId));
        return RedirectToAction(nameof(Index));
    }
    
}
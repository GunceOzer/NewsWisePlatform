using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.UI.Controllers;

public class LikeController : Controller
{
    
    private readonly ILikeService _likeService;

    public LikeController(ILikeService likeService)
    {
        _likeService = likeService;
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Like(LikeModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        model.UserId = new Guid(userId);
        await _likeService.LikeArticleAsync(model.ArticleId, model.UserId);

        return RedirectToAction("Index", "Article");
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Unlike(LikeModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        model.UserId = new Guid(userId);
        await _likeService.UnlikeArticleAsync(model.ArticleId, model.UserId);
        
        return RedirectToAction("Index","Article");
    }
}
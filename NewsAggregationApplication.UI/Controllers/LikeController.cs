using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.UI.Controllers;

public class LikeController : Controller
{
    
    private readonly ILikeService _likeService;
    private readonly ILogger<LikeController> _logger;


    public LikeController(ILikeService likeService, ILogger<LikeController> logger)
    {
        _likeService = likeService;
        _logger = logger;
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
        try
        {
            var result = await _likeService.LikeArticleAsync(model.ArticleId, new Guid(userId));
            if (result)
            {
                _logger.LogInformation("Article liked successfully.");
                return RedirectToAction("Index", "Article");
            }
            else
            {
                _logger.LogWarning("Article already liked.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liking article.");
        }
        return RedirectToAction("Index", "Article");
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Unlike(LikeModel model)
    {
       var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var result = await _likeService.UnlikeArticleAsync(model.ArticleId, new Guid(userId));
            if (result)
            {
                _logger.LogInformation("Article unliked successfully.");
                return RedirectToAction("Index", "Article");
            }
            else
            {
                _logger.LogWarning("Article had not been liked.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unliking article.");
        }
        return RedirectToAction("Index", "Article");
           
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.UI.Controllers;

public class LikeController : Controller
{
    
    private readonly ILikeService _likeService;
    private readonly ILogger<LikeController> _logger;
    private readonly LikeMapper _likeMapper;


    public LikeController(ILikeService likeService, ILogger<LikeController> logger, LikeMapper likeMapper)
    {
        _likeService = likeService;
        _logger = logger;
        _likeMapper = likeMapper;
    }
    
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Like(LikeModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            model.UserId = Guid.Parse(userId);

            var likeDto = _likeMapper.LikeModelToLikeDto(model);

            var result = await _likeService.LikeArticleAsync(likeDto);
            if (result)
            {
                _logger.LogInformation("Article liked.");
                return RedirectToAction("Details", "Article", new { id = model.ArticleId });
            }

            return BadRequest("Unable to like article.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to like article.");
            return RedirectToAction("Details", "Article", new { id = model.ArticleId, error = "Failed to like article" });
        }
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Unlike(LikeModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            model.UserId = Guid.Parse(userId);
            var likeDto = _likeMapper.LikeModelToLikeDto(model);

            var result = await _likeService.UnlikeArticleAsync(likeDto);
            if (result)
            {
                _logger.LogInformation("Article unliked.");
                return RedirectToAction("Details", "Article", new { id = model.ArticleId });
            }

            return BadRequest("Unable to unlike article.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unlike article.");
            return RedirectToAction("Details", "Article", new { id = model.ArticleId, error = "Failed to unlike article" });
        }
    }
}
    
   

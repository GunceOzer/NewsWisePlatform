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
    public async Task<IActionResult> Like(LikeModel model, string returnUrl)
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
                TempData["SuccessMessage"] = "Article liked successfully.";

                _logger.LogInformation("Article liked.");
                return Redirect(returnUrl);
               
            }
            TempData["ErrorMessage"] = "You already liked the article.";
            return Redirect(returnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to like article.");
            TempData["ErrorMessage"] = "An error occurred while liking the article.";
            return Redirect(returnUrl);
        }
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Dislike(LikeModel model, string returnUrl)
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

            var result = await _likeService.DislikeArticleAsync(likeDto);
            if (result)
            {
                _logger.LogInformation("Article disliked.");
                return Redirect(returnUrl);
                
            }
            TempData["ErrorMessage"] = "Unable to dislike because you already disliked the article.";
            return Redirect(returnUrl);

           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dislike article.");
            TempData["ErrorMessage"] = "An error occurred while disliking the article.";
            return Redirect(returnUrl);
            
        }
    }
}
    
   

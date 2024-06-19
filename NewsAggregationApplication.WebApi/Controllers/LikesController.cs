using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LikesController : Controller
{
    private readonly ILikeService _likeService;
    private readonly ILogger<LikesController> _logger;

    public LikesController(ILikeService likeService, ILogger<LikesController> logger)
    {
        _likeService = likeService;
        _logger = logger;
    }

   
    /// <summary>
    /// Likes an article.
    /// </summary>
    /// <param name="likeDto">The like DTO containing article and user information.</param>
    /// <returns>Result of the like operation.</returns>
    /// <response code="200">If the article is liked successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpPost("like")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LikeArticle([FromBody] LikeDto likeDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new{Message= "User is not authoried"}); 
        }

        try
        {
            likeDto.UserId = Guid.Parse(userId);
            var result = await _likeService.LikeArticleAsync(likeDto);
            if (result)
            {
                _logger.LogInformation("Article liked successfully.");
                return Ok(new{ Message = "Article liked sucessfuly"});
            }
            return BadRequest(new { Message = "Unable to like article" });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to like article.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to like article" });
        }
    }


    /// <summary>
    /// Removes a like from an article.
    /// </summary>
    /// <param name="likeDto">The like DTO containing article and user information.</param>
    /// <returns>Result of the unlike operation.</returns>
    /// <response code="200">If the like is removed successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the user is not authorized.</response>
    /// <response code="500">If there is an internal server error.</response>
    [Authorize]
    [HttpPost("unlike")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Unlike([FromBody] LikeDto likeDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new {Message= "User is not authorized"});
        }

        try
        {
            likeDto.UserId = Guid.Parse(userId);
            var result = await _likeService.UnlikeArticleAsync(likeDto);
            if (result)
            {
                _logger.LogInformation("Article unliked successfully.");
                return Ok(new { Message = "Article unliked successfully" });
            }
            else
            {
                _logger.LogWarning("Article had not been unliked.");
                return BadRequest(new { Message = "Article had not been unliked." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unliking article");
            return StatusCode(StatusCodes.Status500InternalServerError, new{Message= " Error unliking article"});
        }
    }
}
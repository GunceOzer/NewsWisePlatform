using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.UI.Controllers;


public class CommentController : Controller
{

    private readonly ICommentService _commentService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CommentController> _logger;
    private readonly CommentMapper _commentMapper;

    public CommentController(ICommentService commentService, UserManager<User> userManager, ILogger<CommentController> logger, CommentMapper commentMapper)
    {
        _commentService = commentService;
        _userManager = userManager;
        _logger = logger;
        _commentMapper = commentMapper;
    }


    // GET
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddComment(Guid articleId, CommentViewModel model)
    {
       
        
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == null)
        {
            return Unauthorized();
        }
        try
        {
            var commentDto = await _commentService.AddCommentAsync(articleId, userId, model.Content,CancellationToken.None);
            _logger.LogInformation("Comment added.");
            if (commentDto == null)
            {
                return BadRequest("Unable to add comment.");
            }
            return RedirectToAction("Details", "Article", new { id = articleId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add comment.");
            return RedirectToAction("Details", "Article", new { id = articleId, error = "Failed to add comment" });
        }
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> EditComment(Guid articleId, EditCommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "There was a problem with your submission.";
            return RedirectToAction("Details", "Article", new { id = articleId });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool isAdmin = User.IsInRole("Admin");

        var success = await _commentService.EditCommentAsync(model.Id, new Guid(userId), model.Content, isAdmin);

        try{
            if (success)
            {
                _logger.LogInformation("Comment edited successfully.");
                return RedirectToAction("Details", "Article", new { id = articleId });
            }
            else
            {
                TempData["Error"] = "Comment could not be edited.";
                _logger.LogWarning("Failed to edit comment.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing comment.");
            TempData["Error"] = "An error occurred while editing the comment.";
        }
        return RedirectToAction("Details", "Article", new { id = articleId });
    }

   
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid articleId, Guid commentId)
    {
        var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
        bool isAdmin = User.IsInRole("Admin");
        try
        {
            var success = await _commentService.DeleteCommentAsync(commentId, userId, isAdmin);
            if (success)
            {
                _logger.LogInformation("Comment successfully deleted.");
            }
            else
            {
                _logger.LogWarning("Failed to delete comment.");
                TempData["Error"] = "Comment could not be deleted.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment.");
            TempData["Error"] = "An error occurred while deleting the comment.";
        }
        return RedirectToAction("Details", "Article", new { id = articleId });
    
    }
}
    

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Models;

namespace NewsAggregationApplication.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : Controller
{
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;
        private readonly CommentMapper _commentMapper;


        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger, CommentMapper commentMapper)
        {
            _commentService = commentService;
            _logger = logger;
            _commentMapper = commentMapper;
        }
        
        /// <summary>
        /// Adds a new comment to an article.
        /// </summary>
        /// <param name="articleId">The ID of the article to add a comment to.</param>
        /// <param name="model">The comment view model containing comment details.</param>
        /// <returns>Result of the add comment operation.</returns>
        /// <response code="200">If the comment is added successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPost("add")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddComment(Guid articleId, [FromBody] CommentViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User is not authorized" });
            }

            try
            {
                var commentDto = _commentMapper.CommentModelToCommentDto(model);
                commentDto.ArticleId = articleId;
                commentDto.UserId = Guid.Parse(userId);
                commentDto.CreatedAt = DateTime.UtcNow;

                var result = await _commentService.AddCommentAsync(commentDto);
                if (result)
                {
                    _logger.LogInformation("Comment added.");
                    return Ok(new { Message = "Comment added successfully", Comment = commentDto });
                }

                return BadRequest(new { Message = "Unable to add comment" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add comment.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Failed to add comment" });
            }
        }

        /// <summary>
        /// Edits an existing comment.
        /// </summary>
        /// <param name="articleId">The ID of the article associated with the comment.</param>
        /// <param name="model">The edit comment view model containing updated comment details.</param>
        /// <returns>Result of the edit comment operation.</returns>
        /// <response code="200">If the comment is edited successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpPut("edit")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditComment(Guid articleId, [FromBody] EditCommentViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User is not authorized" });
            }

            try
            {
                var commentDto = _commentMapper.EditCommentViewModelToCommentDto(model);
                commentDto.UserId = Guid.Parse(userId);

                var success = await _commentService.EditCommentAsync(commentDto);
                if (success)
                {
                    _logger.LogInformation("Comment edited successfully.");
                    return Ok(new { Message = "Comment edited successfully" });
                }

                _logger.LogWarning("Failed to edit comment.");
                return BadRequest(new { Message = "Comment could not be edited" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing comment.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while editing the comment" });
            }
        }

        /// <summary>
        /// Delete a comment.
        /// </summary>
        /// <param name="articleId">The ID of the article associated with the comment.</param>
        /// <param name="commentId">The ID of the comment to delete.</param>
        /// <returns>Result of the delete comment operation.</returns>
        /// <response code="200">If the comment is deleted successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If there is an internal server error.</response>
        [HttpDelete("delete")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteComment(Guid articleId, Guid commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User is not authorized" });
            }

            try
            {
                //var success = await _commentService.DeleteCommentAsync(commentId, Guid.Parse(userId), isAdmin);
                var success = await _commentService.DeleteCommentAsync(commentId);

                if (success)
                {
                    _logger.LogInformation("Comment successfully deleted.");
                    return Ok(new { Message = "Comment deleted successfully" });
                }
                else
                {
                    _logger.LogWarning("Failed to delete comment.");
                    return BadRequest(new { Message = "Comment could not be deleted" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while deleting the comment" });
            }
        }
    
}
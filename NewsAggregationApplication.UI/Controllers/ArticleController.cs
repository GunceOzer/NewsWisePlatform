using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;

namespace NewsAggregationApplication.UI.Controllers;

public class ArticleController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICommentService _commentService;
    private readonly ArticleMapper _articleMapper;
    private readonly CommentMapper _commentMapper;
    private readonly ILogger<ArticleController> _logger;

    public ArticleController(IArticleService articleService, ICommentService commentService, ArticleMapper articleMapper, CommentMapper commentMapper, ILogger<ArticleController> logger)
    {
        _articleService = articleService;
        _commentService = commentService;
        _articleMapper = articleMapper;
        _commentMapper = commentMapper;
        _logger = logger;
    }

    
    public async Task<IActionResult> Index()
    {
        ViewData["IsAdmin"] = User.IsInRole("Admin");
        Guid userId = User.Identity.IsAuthenticated ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : Guid.Empty;
        var articles = await _articleService.GetArticlesAsync(userId);
        var model = articles.Select(a => _articleMapper.ArticleDtoToArticleModel(a)).ToList();
        return View(model);
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Aggregate()
    {
        try
        {
            await _articleService.AggregateFromSourceAsync(CancellationToken.None);
            TempData["SuccessMessage"] = "RSS feeds successfully aggregated.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to aggregate RSS feeds.");
            TempData["ErrorMessage"] = "Failed to aggregate RSS feeds. Check logs for details.";
            return RedirectToAction("Index");
        }
       
        
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> DeleteArticle(Guid id)
    {
        try
        {
            var deleted = await _articleService.DeleteArticleAsync(id);
            if (!deleted)
            {
                _logger.LogWarning($"Article with ID: {id} not found.");
                return NotFound();
            }
            _logger.LogInformation($"Article with ID: {id} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting article with ID: {id}.");
            return NotFound();
        }
    }
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            Guid userId = User.Identity.IsAuthenticated ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : Guid.Empty;

            var articleDto = await _articleService.GetArticlesByIdAsync(id,userId);
            if (articleDto == null)
            {
                _logger.LogWarning($"Article with ID: {id} not found.");
                return NotFound();
            }

            var article = _articleMapper.ArticleDtoToArticleModel(articleDto);
            var comments = await _commentService.GetCommentsByArticleIdAsync(id);
            article.Comments = comments.Select(_commentMapper.CommentDtoToCommentModel).ToList();

            return View(article);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to retrieve details for article with ID: {id}.");
            return NotFound();
        }
    }
    
    [Authorize]
    public async Task<IActionResult> SortByPositivity(bool sortByPositive)
    {
        ViewData["IsAdmin"] = User.IsInRole("Admin");
        Guid userId = User.Identity.IsAuthenticated ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : Guid.Empty;
        var articles = await _articleService.GetSortedArticlesByPositivityAsync(userId, sortByPositive);
        var model = articles.Select(a => _articleMapper.ArticleDtoToArticleModel(a)).ToList();
        return View("Index", model);
    }

    [Authorize]
    public async Task<IActionResult> SortByDate(bool sortByNewest)
    {
        ViewData["IsAdmin"] = User.IsInRole("Admin");
        Guid userId = User.Identity.IsAuthenticated
            ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            : Guid.Empty;
        var articles = await _articleService.GetSortedArticlesByDate(userId, sortByNewest);
        var model = articles.Select(a => _articleMapper.ArticleDtoToArticleModel(a)).ToList();
        return View("Index", model);
    }
   

    
}
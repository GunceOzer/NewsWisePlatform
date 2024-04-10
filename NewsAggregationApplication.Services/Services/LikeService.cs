using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class LikeService:ILikeService
{
    
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<LikeService> _logger;

    public LikeService(NewsDbContext dbContext, ILogger<LikeService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> LikeArticleAsync(Guid articleId, Guid userId)
    {
        if (_dbContext.Likes.Any(l => l.ArticleId == articleId && l.UserId == userId))
        {
            Console.WriteLine("You already liked this news");
            return false;
        }
        
        var like = new Like
        {
            Id = Guid.NewGuid(),
            ArticleId = articleId,
            UserId = userId,
        };
        _dbContext.Likes.Add(like);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlikeArticleAsync(Guid articleId, Guid userId)
    {
        var like = await _dbContext.Likes.FirstOrDefaultAsync(l => l.ArticleId == articleId && l.UserId == userId);
        if (like == null)
        {
            Console.WriteLine("You cannot unlike this news because you already liked it");
            return false;
        }

        _dbContext.Likes.Remove(like);
        await _dbContext.SaveChangesAsync();
        return true;

    }
}
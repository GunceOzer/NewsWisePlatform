using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;

namespace NewsAggregationApplication.UI.Services;

public class BookmarkService:IBookmarkService
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<BookmarkService> _logger;

    public BookmarkService(NewsDbContext dbContext, ILogger<BookmarkService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    public async Task<bool> BookmarkArticleAsync(Guid articleId, Guid userId)
    {
        if (_dbContext.Bookmarks.Any(b => b.ArticleId == articleId && b.UserId == userId))
        {
            Console.WriteLine("You've already bookmarked the news");
            return false;
        }

        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid(),
            ArticleId = articleId,
            UserId = userId
        };
        _dbContext.Bookmarks.Add(bookmark);
        await _dbContext.SaveChangesAsync();
        return true;

    }

    public async Task<bool> RemoveBookmarkAsync(Guid articleId, Guid userId)
    {
        var bookmark =
            await _dbContext.Bookmarks.FirstOrDefaultAsync(b => b.ArticleId == articleId && b.UserId == userId);
        if (bookmark == null)
        {
            Console.WriteLine("This book is not bookmarked before so you can't remove bookmark");
            return false;
        }

        _dbContext.Remove(bookmark);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Article>> GetBookmarkArticlesAsync(Guid userId)
    {
       var bookmarks = await _dbContext.Bookmarks
            .Include(b=> b.Article)
            .ThenInclude(a=>a.Likes)
            .Where(b=> b.UserId ==userId)
            .Select(b=>b.Article)
            .ToListAsync();

       return bookmarks ?? new List<Article>();
    }

    public async Task<bool> IsArticleBookmarkedByUser(Guid articleId, Guid userId)
    {
        return await _dbContext.Bookmarks.AnyAsync(b => b.ArticleId == articleId && b.UserId == userId);
    }
}
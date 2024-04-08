using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;


namespace NewsAggregationApplication.UI.Services;

public class ArticleService:IArticleService
{
    private readonly NewsDbContext _dbContext;
    private readonly ILogger<ArticleService> _logger;


    public ArticleService(NewsDbContext dbContext, ILogger<ArticleService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Article>> GetArticlesAsync()
    {
        return await _dbContext.Articles.Include(a=> a.Likes).ToListAsync();
    }

    public async Task<Article?> GetArticlesByIdAsync(Guid id)
    {
        return await _dbContext.Articles.SingleOrDefaultAsync(a => a.Id.Equals(id));
    }

    public async Task AggregateFromSourceAsync(string rssLink)
    {
        try
        {
            var reader = XmlReader.Create(rssLink);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            // finding rss url in database
            var source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.Url == rssLink);

            // if the source doesn't exist, create a new source
            if (source == null)
            {
                source = new Source
                    { Id = Guid.NewGuid(), Url = rssLink, Name = rssLink }; 
                _dbContext.Sources.Add(source);
                
            }

            var newArticles = feed.Items
                .Where(item => !_dbContext.Articles.Any(a => a.SourceUrl == item.Links[0].Uri.ToString()))
                .Select(item => new Article
                {
                    Id = Guid.NewGuid(),
                    Title = item.Title.Text,
                    Description = item.Summary.Text,
                    PublishedDate = item.PublishDate.UtcDateTime,
                    SourceUrl = item.Links[0].Uri.ToString(),
                    SourceId = source.Id 
                }).ToList();

            if (newArticles.Count > 0)
            {
                await _dbContext.Articles.AddRangeAsync(newArticles);
                await _dbContext.SaveChangesAsync(); 
                _logger.LogDebug("Articles were added to DB successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occurred while aggregating articles from source."); 
            throw; 
        }
    }

    public async Task<bool> DeleteArticleAsync(Guid id)
    {
        var article = await _dbContext.Articles.FindAsync(id);
        if (article == null) return false;

        _dbContext.Articles.Remove(article);
        await _dbContext.SaveChangesAsync();
        return true;
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
        return await _dbContext.Bookmarks
            .Where(b => b.UserId == userId)
            .Select(b => b.Article)
            .Include(a => a.Bookmarks)
            .ToListAsync();
    }

    public async Task AddCommentAsync(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
    }
}
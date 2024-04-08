using NewsAggregationApplication.Data.Entities;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IArticleService
{
    public Task<IEnumerable<Article>> GetArticlesAsync();
    public Task<Article?> GetArticlesByIdAsync(Guid id);

    public Task AggregateFromSourceAsync(string rssLink);
    public Task<bool> DeleteArticleAsync(Guid id);

    public Task<bool> LikeArticleAsync(Guid articleId, Guid userId);
    public Task<bool> UnlikeArticleAsync(Guid articleId, Guid userId);

    public Task<bool> BookmarkArticleAsync(Guid articleId, Guid userId);
    public Task<bool> RemoveBookmarkAsync(Guid articleId, Guid userId);
    public Task<IEnumerable<Article>> GetBookmarkArticlesAsync(Guid userId);
    public Task AddCommentAsync(Comment comment);
   // public Task<List<CommentViewModel>> GetCommentsByArticleIdAsync(Guid articleId);
}
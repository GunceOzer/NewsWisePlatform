using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IArticleService
{
    public Task<IEnumerable<Article>> GetArticlesAsync();
    public Task<Article?> GetArticlesByIdAsync(Guid id);

    public Task AggregateFromSourceAsync(string rssLink);
    public Task<bool> DeleteArticleAsync(Guid id);

   // public Task<bool> LikeArticleAsync(Guid articleId, Guid userId);
    //public Task<bool> UnlikeArticleAsync(Guid articleId, Guid userId);

    //public Task<bool> BookmarkArticleAsync(Guid articleId, Guid userId);
    //public Task<bool> RemoveBookmarkAsync(Guid articleId, Guid userId);
    // Task<IEnumerable<Article>> GetBookmarkArticlesAsync(Guid userId);
    public Task AddCommentAsync(CommentDTO commentDTO, Guid userID);
    public Task<List<CommentDTO>> GetCommentsByArticleIdAsync(Guid articleId);
}
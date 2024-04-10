using NewsAggregationApplication.Data.Entities;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IBookmarkService
{
    public Task<bool> BookmarkArticleAsync(Guid articleId, Guid userId);
    public Task<bool> RemoveBookmarkAsync(Guid articleId, Guid userId);
    public Task<IEnumerable<Article>> GetBookmarkArticlesAsync(Guid userId);
    public Task<bool> IsArticleBookmarkedByUser(Guid articleId, Guid userId);

}
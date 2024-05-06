using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IBookmarkService
{
    public Task<bool> BookmarkArticleAsync(Guid articleId, Guid userId);
    //public Task<bool> BookmarkArticleAsync(BookmarkDto bookmarkDto);
    public Task<bool> RemoveBookmarkAsync(Guid articleId, Guid userId);
    //public Task<IEnumerable<Article>> GetBookmarkArticlesAsync(Guid userId);
    public Task<IEnumerable<ArticleDto>> GetBookmarkArticlesAsync(Guid userId);
    public Task<bool> IsArticleBookmarkedByUser(Guid articleId, Guid userId);
    //public Task<IEnumerable<Guid>> GetBookmarkedArticleIdsByUser(Guid userId);


}
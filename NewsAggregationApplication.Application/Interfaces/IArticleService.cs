using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IArticleService
{
    public Task<IEnumerable<ArticleDto>> GetArticlesAsync(Guid? userId);
    public Task<ArticleDto?> GetArticlesByIdAsync(Guid id , Guid userId);
    public Task AggregateFromSourceAsync(IEnumerable<string> rssLinks, CancellationToken cancellationToken);
    public Task<bool> DeleteArticleAsync(Guid id);

    //public Task<List<ArticleDto>> GetArticlesWithBookmarkStatus(Guid userId);
    

}
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Interfaces;

public interface IArticleService
{
    public Task<IEnumerable<ArticleDto>> GetArticlesAsync(Guid? userId);
    public Task<ArticleDto?> GetArticlesByIdAsync(Guid id , Guid userId);
    public Task AggregateFromSourceAsync( CancellationToken cancellationToken);
    public Task<bool> DeleteArticleAsync(Guid id);
    Task<IEnumerable<ArticleDto>> GetSortedArticlesByPositivityAsync(Guid? userId, bool sortByPositive);

    Task<IEnumerable<ArticleDto>> GetSortedArticlesByDate(Guid? userId, bool sortByNewest);




}
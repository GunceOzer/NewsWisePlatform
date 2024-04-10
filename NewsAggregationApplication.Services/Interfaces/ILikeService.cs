namespace NewsAggregationApplication.UI.Interfaces;

public interface ILikeService
{
    public Task<bool> LikeArticleAsync(Guid articleId, Guid userId);
    public Task<bool> UnlikeArticleAsync(Guid articleId, Guid userId);
}
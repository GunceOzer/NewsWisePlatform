using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Interfaces;

public interface ILikeService
{
   
    public Task<bool> LikeArticleAsync(LikeDto likeDto);
    public Task<bool> UnlikeArticleAsync(LikeDto likeDto);
}
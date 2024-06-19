using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Interfaces;

public interface ICommentService
{
   public  Task<bool> AddCommentAsync(CommentDto commentDto);
    public Task<IEnumerable<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId);
    public Task<bool> EditCommentAsync(CommentDto commentDto);
  
    public Task<bool> DeleteCommentAsync(Guid commentId);
}
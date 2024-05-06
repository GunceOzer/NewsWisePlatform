using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Interfaces;

public interface ICommentService
{
    public Task<CommentDto> AddCommentAsync(Guid articleId, Guid userId, string content,CancellationToken cancellationToken);
    public Task<List<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId);
   public Task<bool> EditCommentAsync(Guid commentId, Guid userId, string content, bool isAdmin);
    public Task<bool> DeleteCommentAsync(Guid commentId, Guid userId, bool isAdmin);
}
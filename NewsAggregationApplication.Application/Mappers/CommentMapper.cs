using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Models;
using Riok.Mapperly.Abstractions;

namespace NewsAggregationApplication.UI.Mappers;

[Mapper]
public partial class CommentMapper
{

    public partial CommentViewModel CommentDtoToCommentModel(CommentDto commentDto);

    public  CommentDto CommentToCommentDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            FullName = comment.User.FullName, 
            UserId = comment.UserId,
            ArticleId = comment.ArticleId
        };
    }
    public partial Comment CommentDtoToComment(CommentDto commentDto);
    public partial CommentDto CommentModelToCommentDto(CommentViewModel commentViewModel);

    public partial void UpdateCommentFromDto(CommentDto commentDto, Comment comment);
    public partial CommentDto EditCommentViewModelToCommentDto(EditCommentViewModel editCommentViewModel);

    
}
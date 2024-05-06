using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Models;
using Riok.Mapperly.Abstractions;

namespace NewsAggregationApplication.UI.Mappers;

[Mapper]
public partial class CommentMapper
{

    public partial CommentViewModel CommentDtoToCommentModel(CommentDto commentDto);

    public  CommentDto CommentModelToCommentDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            FullName = comment.User.FullName, 
            UserId = comment.UserId
        };
    }
}
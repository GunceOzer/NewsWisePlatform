using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Models;
using Riok.Mapperly.Abstractions;

namespace NewsAggregationApplication.UI.Mappers;

[Mapper]
public partial class LikeMapper
{
    
    public partial LikeDto LikeToLikeDto(Like like);
    public partial Like LikeDtoToLike(LikeDto likeDto);
    public partial LikeModel LikeDtoToLikeModel(LikeDto likeDto);
    public partial LikeDto LikeModelToLikeDto(LikeModel likeModel);
}
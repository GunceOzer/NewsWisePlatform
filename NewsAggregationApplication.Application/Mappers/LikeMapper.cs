using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.DTOs;
using NewsAggregationApplication.UI.Models;
using Riok.Mapperly.Abstractions;

namespace NewsAggregationApplication.UI.Mappers;

[Mapper]
public partial class LikeMapper
{
    
    public partial LikeModel LikeDtoToLikeModel(LikeDto dto);
    
    public partial LikeDto LikeToLikeDto(Like like);
}
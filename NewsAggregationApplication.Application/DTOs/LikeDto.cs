using NewsAggregationApplication.Data.Entities;

namespace NewsAggregationApplication.UI.DTOs;

public class LikeDto
{
    //public Guid Id { get; set; }
    //public ArticleDto Article { get; set; } 
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
}
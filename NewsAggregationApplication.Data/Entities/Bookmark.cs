namespace NewsAggregationApplication.Data.Entities;

public class Bookmark
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }

    
    public Article Article { get; set; }
    public User User { get; set; }
}
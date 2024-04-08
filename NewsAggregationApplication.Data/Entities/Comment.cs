namespace NewsAggregationApplication.Data.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
    
    public Article Article { get; set; }
    public User User { get; set; }
}
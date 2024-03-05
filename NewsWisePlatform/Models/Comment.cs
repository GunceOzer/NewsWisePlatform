namespace NewsWisePlatform.Models;

public class Comment
{
    public Guid CommentID { get; set; }
    public Guid NewsItemID { get; set; }
    public Guid UserID { get; set; }
    public string Content { get; set; }
    public DateTime PostDate { get; set; }
    public float SentimentScore { get; set; }
    public CommentFlag Flag { get; set; }
    public NewsItem NewsItem { get; set; }
    public User User { get; set; }
}
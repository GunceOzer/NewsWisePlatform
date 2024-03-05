namespace NewsWisePlatform.Models;

public class NewsItemLike
{
    public Guid NewsItemLikeID { get; set; }
    public Guid NewsItemID { get; set; }
    public Guid UserID { get; set; }
    public DateTime LikedAt { get; set; }
    public User User { get; set; }
    public NewsItem NewsItem { get; set; }
}
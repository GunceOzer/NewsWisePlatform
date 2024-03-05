namespace NewsWisePlatform.Models;

public class NewsItemShare
{
    public Guid ShareID { get; set; }
    public Guid NewsItemID { get; set; }
    public Guid UserID { get; set; }
    public DateTime SharedAt { get; set; }
    public NewsItem NewsItem { get; set; }
    public User User { get; set; }
}
namespace NewsWisePlatform.Models;

public class NewsItemTopic
{
    public Guid NewsItemID { get; set; }
    public Guid TopicID { get; set; }
    
    public Topic Topic { get; set; }
    public NewsItem NewsItem { get; set; }
}
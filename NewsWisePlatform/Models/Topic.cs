namespace NewsWisePlatform.Models;

public class Topic
{
    public Guid TopicID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<NewsItem> NewsItems { get; set; }
    public ICollection<NewsItemTopic> NewsItemTopics { get; set; }
}
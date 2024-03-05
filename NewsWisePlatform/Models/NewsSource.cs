namespace NewsWisePlatform.Models;

public class NewsSource
{
    public Guid SourceID { get; set; }
    public string Name { get; set; }
    public string URL { get; set; }
    public bool IsActive { get; set; } //checks if the news source is still active 
}
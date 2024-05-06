namespace NewsAggregationApplication.UI.DTOs;

public class ArticleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; } 
    public DateTime PublishedDate { get; set; }
    public int LikesCount { get; set; }
    public string SourceUrl { get; set; }
    public string UrlToImage { get; set; }
    
    public bool IsBookmarked { get; set; }
    
}
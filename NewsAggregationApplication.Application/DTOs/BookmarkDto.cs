namespace NewsAggregationApplication.UI.DTOs;

public class BookmarkDto
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
    public bool IsBookmarked { get; set; }

    
}
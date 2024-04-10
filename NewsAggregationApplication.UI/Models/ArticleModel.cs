using NewsAggregationApplication.UI.DTOs;

namespace NewsAggregationApplication.UI.Models;

public class ArticleModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public DateTime PublicationDate { get; set; }
    public string SourceUrl { get; set; }
    public int? LikesCount { get; set; }
    public bool IsBookmarked { get; set; }
    
    public List<CommentViewModel> Comments { get; set; }
}
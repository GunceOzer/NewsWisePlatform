using System.ComponentModel.DataAnnotations;

namespace NewsAggregationApplication.Data.Entities;

public class Article
{
    [Key]
    public Guid Id { get; set; }

    public string Description { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public string SourceUrl { get; set; }
    public DateTime PublishedDate { get; set; }
    public float? PositivityScore { get; set; }
    public string? UrlToImage { get; set; }
    public Guid SourceId { get; set; }
    
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Bookmark> Bookmarks { get; set; }
}
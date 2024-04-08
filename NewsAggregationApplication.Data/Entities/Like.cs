using System.ComponentModel.DataAnnotations;

namespace NewsAggregationApplication.Data.Entities;

public class Like
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public Guid ArticleId { get; set; }
    [Required]
    public Guid UserId { get; set; }

    // Navigation properties
    public Article Article { get; set; }
    public User User { get; set; }
}


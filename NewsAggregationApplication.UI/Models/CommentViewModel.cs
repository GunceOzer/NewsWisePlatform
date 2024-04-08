using System.ComponentModel.DataAnnotations;

namespace NewsAggregationApplication.UI.Models;

public class CommentViewModel
{
    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    [StringLength(500)]
    public string Content { get; set; }
}
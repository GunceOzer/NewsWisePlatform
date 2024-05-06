using System.ComponentModel.DataAnnotations;

namespace NewsAggregationApplication.UI.Models;

public class EditCommentViewModel
{ 
    [Required]
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }

    [Required]
    [StringLength(500)]
    public string Content { get; set; }
    
}
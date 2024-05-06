using System.ComponentModel.DataAnnotations;
namespace NewsAggregationApplication.UI.Models;

public class CommentViewModel
{
    [Required]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    [Required]
    [StringLength(500)]
    public string Content { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }


}
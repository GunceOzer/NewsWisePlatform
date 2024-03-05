namespace NewsWisePlatform.Models;

public class CommentFlag
{
    public Guid CommentFlagID { get; set; }
    public Guid CommentID { get; set; }
    public Guid? UserID { get; set; } //the user who flagged the comment
    public string Reason { get; set; }
    public DateTime FlaggedAt { get; set; }
    public Comment Comment { get; set; }
    public User User { get; set; }
}
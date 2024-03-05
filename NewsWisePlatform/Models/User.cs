namespace NewsWisePlatform.Models;

public class User
{
    public Guid UserID { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime LastLoginDate { get; set; }
    
    public ICollection<UserPreference> UserPreferences { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ExternalLogin ExternalLogin { get; set; }
    public ICollection<NewsItemBookmark> NewsItemBookmarks { get; set; }
    public ICollection<NewsItemLike> Likes { get; set; }
    public ICollection<NewsItemShare> NewsItemShares { get; set; }
    
}
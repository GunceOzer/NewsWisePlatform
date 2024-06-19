using Microsoft.AspNetCore.Identity;

namespace NewsAggregationApplication.Data.Entities;

public class User:IdentityUser<Guid>
{
    public string FullName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastNotifiedDate { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Bookmark> Bookmarks { get; set; }
}
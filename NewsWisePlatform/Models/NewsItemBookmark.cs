namespace NewsWisePlatform.Models;

public class NewsItemBookmark
{
    public Guid BookmarkID { get; set; }
    public Guid NewsItemID { get; set; }
    public Guid UserID { get; set; }
    public DateTime SavedAt { get; set; } // the order the saved items by date
    
    //navigation properties
    public NewsItem NewsItem { get; set; }
    public User User { get; set; }
}
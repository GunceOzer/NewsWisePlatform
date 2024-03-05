namespace NewsWisePlatform.Models;

public class NewsItem
{
    public Guid NewsItemID { get; set; }
    public string Title { get; set; }
    public string Content { get; set; } // The full content if available
    public string Summary { get; set; } // The description from the API
    public string Author { get; set; } // New field for the author
    public Guid SourceID { get; set; } // Foreign key to NewsSource table
    public string SourceURL { get; set; } // The URL to the full article
    public DateTime PublishedDate { get; set; }
    public float PositivityScore { get; set; } 
    public ICollection<Topic> Topics { get; set; }//??
    public ICollection<Comment> Comments { get; set; }
    public ICollection<NewsItemLike> Likes { get; set; }
    public ICollection<MediaAttachment> MediaAttachments { get; set; }
    public ICollection<NewsItemBookmark> Bookmarks { get; set; }
    public ICollection<NewsItemShare> Shares { get; set; }
    
    public ICollection<NewsItemTopic> NewsItemTopics { get; set; }

}
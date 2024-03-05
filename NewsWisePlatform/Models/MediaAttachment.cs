namespace NewsWisePlatform.Models;

public class MediaAttachment
{
    public Guid MediaAttachmentID { get; set; }
    public Guid NewsItemID { get; set; }
    public string URL { get; set; }//could be an image or video
    public string MediaType { get; set; }//Image or Video
    public NewsItem NewsItem { get; set; }
}
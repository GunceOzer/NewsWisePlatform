namespace NewsWisePlatform.Models;

public class EmailNotification
{
    public Guid NotificationID { get; set; }
    public Guid UserID { get; set; }
    public DateTime SentDate { get; set; }
    //public string NotificationType { get; set; }
}
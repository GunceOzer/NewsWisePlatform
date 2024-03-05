namespace NewsWisePlatform.Models;

public class Admin
{
    public Guid AdminID { get; set; }
    public Guid UserID { get; set; }
    public string Role { get; set; }
    public string Permissions { get; set; }
    public User User { get; set; }
}
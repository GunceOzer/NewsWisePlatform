namespace NewsWisePlatform.Models;

public class ExternalLogin
{
    public Guid ExternalLoginID { get; set; }
    public Guid UserID { get; set; }
    public string Provider { get; set; }//Google , Facebook
    public string ProviderKey { get; set; }//Token or unique identifier from the provider
    public User User { get; set; }
}
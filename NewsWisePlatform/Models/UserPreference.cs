namespace NewsWisePlatform.Models;

public class UserPreference
{
    
    public Guid PreferenceID { get; set; }
    public Guid UserId { get; set; }
    public float PositivityPreference { get; set; }
    
}
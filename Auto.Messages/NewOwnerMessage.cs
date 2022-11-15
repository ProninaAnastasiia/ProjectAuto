namespace Auto.Messages;

public class NewOwnerMessage
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string VehicleName { get; set; }
    public string Email { get; set; }
    public DateTime ListedAtUtc { get; set; }
}
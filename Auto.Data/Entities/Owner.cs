namespace Auto.Data.Entities;

public class Owner
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string VehicleCode { get; set; }
    public string Email { get; set; }

    public virtual Vehicle Vehicle { get; set; }
}
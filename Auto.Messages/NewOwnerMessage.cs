namespace Auto.Messages;

public class NewOwnerMessage
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string VehicleCode { get; set; }
    public string Email { get; set; }
    public DateTime ListedAtUtc { get; set; }
}

public class NewOwnerVehiclePriceMessage : NewOwnerMessage {
    public string Registration { get; set; }
    public string ModelCode { get; set; }
    public string Color { get; set; }
    public int Year { get; set; }
    public int Price { get; set; }
    public string CurrencyCode { get; set; }

    public NewOwnerVehiclePriceMessage() {
    }

    public NewOwnerVehiclePriceMessage(NewOwnerMessage owner, string vehicleModelCode,
        string vehicleColor, int vehicleYear, int price, string currencyCode) {
        this.FirstName = owner.FirstName;
        this.LastName = owner.LastName;
        this.Email = owner.Email;
        this.Registration = owner.VehicleCode;
        this.ModelCode = vehicleModelCode;
        this.Color = vehicleColor;
        this.Year = vehicleYear;
        this.Price = price;
        this.CurrencyCode = currencyCode;
    }
}
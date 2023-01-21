using Auto.Data;
using Grpc.Core;

namespace Auto.PricingServer.Services;

public class PricerService : Pricer.PricerBase
{
    private readonly IAutoDatabase _db;

    public PricerService(IAutoDatabase db)
    {
        _db = db;
    }

    public override Task<PriceReply> GetVehiclePrice(PriceRequest request, ServerCallContext context)
    {
        return Task.FromResult(GetVehicle(request.VehicleCode));
    }

    public PriceReply GetVehicle(string registration)
    {
        var vehicle = _db.FindVehicle(registration);
        return new PriceReply
        {
            Registration = vehicle?.Registration,
            VehicleModelCode = vehicle?.ModelCode,
            VehicleColor = vehicle?.Color,
            VehicleYear = vehicle.Year,
            CurrencyCode = "RUB",
            Price = 4000000
        };
    }
}
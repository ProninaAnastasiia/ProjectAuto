using Auto.Data.Entities;
using GraphQL.Types;

namespace Auto.Website.GraphQL.GraphTypes;

public sealed class VehicleGraphType : ObjectGraphType<Vehicle>
{
    public VehicleGraphType()
    {
        Name = "vehicle";
        Field(c => c.VehicleModel, false, typeof(ModelGraphType))
            .Description("Модель автомобиля");
        Field(c => c.Registration);
        Field(c => c.Color);
        Field(c => c.Year);
    }
}
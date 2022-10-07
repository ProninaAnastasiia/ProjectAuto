using Auto.Data.Entities;
using GraphQL.Types;

namespace Auto.Website.GraphQL.GraphTypes;

public class OwnerGraphType: ObjectGraphType<Owner> {
    public OwnerGraphType() {
        Name = "owner";
        Field(c => c.Vehicle, nullable: true, type: typeof(VehicleGraphType))
            .Description("Автомобиль");
        Field(c => c.FirstName);
        Field(c => c.LastName);
        Field(c => c.PhoneNumber);
        Field(c => c.Email);
    }
}
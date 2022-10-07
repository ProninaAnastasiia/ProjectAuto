using Auto.Website.GraphQL.GraphTypes;
using Auto.Data.Entities;
using Auto.Data;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Mutations;

public class AutoMutation: ObjectGraphType
{
    private readonly IAutoDatabase _db;

    public AutoMutation(IAutoDatabase db)
    {
        this._db = db;
        
        Field<VehicleGraphType>(
            "createVehicle",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "registration"},
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "color"},
                new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "year"},
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "modelCode"}
            ),
            resolve: context =>
            {
                var registration = context.GetArgument<string>("registration");
                var color = context.GetArgument<string>("color");
                var year = context.GetArgument<int>("year");
                var modelCode = context.GetArgument<string>("modelCode");

                var vehicleModel = db.FindModel(modelCode);
                var vehicle = new Vehicle
                {
                    Registration = registration,
                    Color = color,
                    Year = year,
                    VehicleModel = vehicleModel,
                    ModelCode = vehicleModel.Code
                };
                _db.CreateVehicle(vehicle);
                return vehicle;
            }
        );
        
        Field<OwnerGraphType>(
            "createOwner",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "firstName"},
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "lastName"},
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "phoneNumber"},
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "vehicleCode"},
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "email"}
            ),
            resolve: context =>
            {
                var firstName = context.GetArgument<string>("firstName");
                var lastName = context.GetArgument<string>("lastName");
                var phoneNumber = context.GetArgument<string>("phoneNumber");
                var vehicleCode = context.GetArgument<string>("vehicleCode");
                var email = context.GetArgument<string>("email");

                var vehicle = db.FindVehicle(vehicleCode);
                var owner = new Owner
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    Email = email,
                    VehicleCode = vehicle.Registration,
                    Vehicle = vehicle
                };
                _db.CreateOwner(owner);
                return owner;
            }
        );
        
        
    }
}
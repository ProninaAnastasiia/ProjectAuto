﻿using System;
using System.Collections.Generic;
using System.Linq;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queires;

public class AutoQuery : ObjectGraphType
{
    private readonly IAutoDatabase _db;

    public AutoQuery(IAutoDatabase db)
    {
        _db = db;

        Field<ListGraphType<VehicleGraphType>>("Vehicles", "Запрос, возвращающий все автомобили",
            resolve: GetAllVehicles);

        Field<VehicleGraphType>("Vehicle", "Запрос к конкретному автомобилю",
            new QueryArguments(MakeNonNullStringArgument("registration", "Номер машины")),
            GetVehicle);

        Field<ListGraphType<VehicleGraphType>>("VehiclesByColor", "Запрос, возвращающий все машины с выбранным цветом",
            new QueryArguments(MakeNonNullStringArgument("color", "Имя цвета")),
            GetVehiclesByColor);

        Field<ListGraphType<OwnerGraphType>>("Owners", "Запрос, возвращающий всех владельцев",
            resolve: GetAllOwners);

        Field<OwnerGraphType>("Owner", "Запрос к конкретному владельцу",
            new QueryArguments(MakeNonNullStringArgument("email", "Email владельца")),
            GetOwner);

        Field<ListGraphType<OwnerGraphType>>("OwnersByLastName",
            "Запрос, возвращающий всех владельцев с определенной фамилией",
            new QueryArguments(MakeNonNullStringArgument("lastName", "Фамилия")),
            GetOwnersByLastName);
    }

    private QueryArgument MakeNonNullStringArgument(string name, string description)
    {
        return new QueryArgument<NonNullGraphType<StringGraphType>>
        {
            Name = name, Description = description
        };
    }

    private IEnumerable<Vehicle> GetAllVehicles(IResolveFieldContext<object> context)
    {
        return _db.ListVehicles();
    }

    private Vehicle GetVehicle(IResolveFieldContext<object> context)
    {
        var registration = context.GetArgument<string>("registration");
        return _db.FindVehicle(registration);
    }

    private IEnumerable<Vehicle> GetVehiclesByColor(IResolveFieldContext<object> context)
    {
        var color = context.GetArgument<string>("color");
        var vehicles = _db.ListVehicles()
            .Where(v => v.Color.Contains(color, StringComparison.InvariantCultureIgnoreCase));
        return vehicles;
    }

    private IEnumerable<Owner> GetAllOwners(IResolveFieldContext<object> context)
    {
        return _db.ListOwners();
    }

    private Owner GetOwner(IResolveFieldContext<object> context)
    {
        var email = context.GetArgument<string>("email");
        return _db.FindOwner(email);
    }

    private IEnumerable<Owner> GetOwnersByLastName(IResolveFieldContext<object> context)
    {
        var lastName = context.GetArgument<string>("lastName");
        var owners = _db.ListOwners()
            .Where(v => v.LastName.Contains(lastName, StringComparison.InvariantCultureIgnoreCase));
        return owners;
    }
}
using Auto.Messages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace Auto.Notifier;

public static class Program
{
    private static readonly IConfigurationRoot config = ReadConfiguration();
    private static HubConnection hub;

    public static async Task Main(string[] args)
    {
        hub = new HubConnectionBuilder().WithUrl(config.GetConnectionString("SIGNALR_HUB_URL")).Build();
        await hub.StartAsync();
        Console.WriteLine("Hub started!");
        using var bus = RabbitHutch.CreateBus(config.GetConnectionString("AutoRabbitMQ"));
        Console.WriteLine("Connected to bus; Listening for NewOwnerVehiclePriceMessage");
        var subscriberId = $"Auto.Notifier@{Environment.MachineName}";
        await bus.PubSub.SubscribeAsync<NewOwnerVehiclePriceMessage>(subscriberId, HandleNewOwnerVehiclePriceMessage);
        Console.ReadLine();
    }

    private static async void HandleNewOwnerVehiclePriceMessage(NewOwnerVehiclePriceMessage novpm)
    {
        var message = $"New owner {novpm.FirstName} {novpm.LastName} has vehicle {novpm.Registration} that costs {novpm.Price} {novpm.CurrencyCode}";
        await hub.SendAsync("NotifyWebUsers", "Auto.Notifier", message);
        Console.WriteLine($"Sent: {message}");
    }
    
    private static IConfigurationRoot ReadConfiguration()
    {
        var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }

}
using System.Text.Json;
using Auto.Messages;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Auto.Notifier;

public static class Program
{
    private const string SIGNALR_HUB_URL = "http://localhost:5000/hub";
    private static HubConnection hub;

    public static async Task Main(string[] args)
    {
        hub = new HubConnectionBuilder().WithUrl(SIGNALR_HUB_URL).Build();
        await hub.StartAsync();
        Console.WriteLine("Hub started!");
        var amqp ="amqp://user:rabbitmq@localhost:5672";
        using var bus = RabbitHutch.CreateBus(amqp);
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

}
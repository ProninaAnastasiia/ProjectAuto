using Auto.Messages;
using Auto.PricingServer;
using EasyNetQ;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

class Program {
    private static Pricer.PricerClient grpcClient;
    private static readonly IConfigurationRoot config = ReadConfiguration();
    private static IBus bus;
    static async Task Main(string[] args) {
        Console.WriteLine("Starting Auto.PricingClient");
        bus = RabbitHutch.CreateBus(config.GetConnectionString("AutoRabbitMQ"));
        Console.WriteLine("Connected to bus; Listening for newVehicleMessages");
        using var channel = GrpcChannel.ForAddress(config.GetConnectionString("gRPC_URL"));
        grpcClient = new Pricer.PricerClient(channel);
        Console.WriteLine($"Connected to gRPC on {config.GetConnectionString("gRPC_URL")}!");
        var subscriberId = $"Auto.PricingClient@{Environment.MachineName}";
        await bus.PubSub.SubscribeAsync<NewOwnerMessage>(subscriberId, HandleNewOwnerVehicleMessage);
        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();
    }

    private static async Task HandleNewOwnerVehicleMessage(NewOwnerMessage message) {
        var priceRequest = new PriceRequest() {
            VehicleCode = message.VehicleCode
        };
        var priceReply = await grpcClient.GetVehiclePriceAsync(priceRequest);
        Console.WriteLine($"Owner {message.FirstName} {message.LastName}, email: {message.Email} with vehicle " +
                          $"{priceReply.Registration}, model:{priceReply.VehicleModelCode}, color: {priceReply.VehicleColor}," +
                          $" year: {priceReply.VehicleYear} costs {priceReply.Price} {priceReply.CurrencyCode}");
        var newOwnerVehiclePriceMessage = new NewOwnerVehiclePriceMessage(message, priceReply.VehicleModelCode,
            priceReply.VehicleColor, priceReply.VehicleYear ,priceReply.Price, priceReply.CurrencyCode);
        await bus.PubSub.PublishAsync(newOwnerVehiclePriceMessage);
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
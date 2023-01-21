using Auto.Messages;
using Auto.PricingServer;
using EasyNetQ;
using Grpc.Net.Client;
class Program {
    private static Pricer.PricerClient grpcClient;
    private static IBus bus;
    static async Task Main(string[] args) {
        Console.WriteLine("Starting Auto.PricingClient");
        var amqp ="amqp://user:rabbitmq@localhost:5672";
        bus = RabbitHutch.CreateBus(amqp);
        Console.WriteLine("Connected to bus; Listening for newVehicleMessages");
        using var channel = GrpcChannel.ForAddress("https://localhost:7175");
        grpcClient = new Pricer.PricerClient(channel);
        Console.WriteLine($"Connected to gRPC on localhost:7175!");
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
}
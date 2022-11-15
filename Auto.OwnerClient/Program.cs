using Auto.OwnerServer;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:7026");
var grpcClient = new OwnerService.OwnerServiceClient(channel);
Console.WriteLine("Введите email владельца:");
while (true)
{
    var email = Console.ReadLine();
    var request = new OwnerRequest
    {
        Email = email
    };
    try
    {
        var reply = grpcClient.GetOwnerByEmail(request);
        Console.WriteLine(
            $"Владелец: {reply.FirstName} {reply.LastName}, номер телефона: {reply.PhoneNumber}, автомобиль: {reply.VehicleCode} \n");
    }
    catch (Exception e)
    {
        Console.WriteLine("Неверно введен email, попробуйте снова.");
    }
}
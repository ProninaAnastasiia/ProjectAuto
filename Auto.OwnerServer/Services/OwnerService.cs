using Auto.Data;
using Grpc.Core;

namespace Auto.OwnerServer.Services;

public class OwnerService : OwnerServer.OwnerService.OwnerServiceBase
{
    private readonly IAutoDatabase _db;

    public OwnerService(IAutoDatabase db)
    {
        _db = db;
    }

    public override Task<OwnerReply> GetOwnerByEmail(OwnerRequest request, ServerCallContext context)
    {
        return Task.FromResult(GetOwner(request.Email));
    }

    public OwnerReply GetOwner(string email)
    {
        var owner = _db.FindOwner(email);
        return new OwnerReply
        {
            FirstName = owner?.FirstName,
            LastName = owner?.LastName,
            PhoneNumber = owner?.PhoneNumber,
            VehicleCode = owner?.VehicleCode
        };
    }
}
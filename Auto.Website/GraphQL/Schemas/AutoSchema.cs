using Auto.Data;
using Auto.Website.GraphQL.Mutations;
using Auto.Website.GraphQL.Queires;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Schemas;

public class AutoSchema : Schema
{
    public AutoSchema(IAutoDatabase db)
    {
        Query = new AutoQuery(db);
        Mutation = new AutoMutation(db);
    }
}
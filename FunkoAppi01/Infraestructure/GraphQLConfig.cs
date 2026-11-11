using FunkoAppi01.GraphQL;
using HotChocolate.Execution.Configuration;

namespace FunkoAppi01.Infraestructure;

public static class GraphQLConfig
{
    /*
    public static IRequestExecutorBuilder AddGraphQLConfig(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        return services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddSubscriptionType<Subscription>()
            .AddInMemorySubscriptions()
            .AddType<FunkoType>()
            .AddType<CategoriaType>()
            .AddType<ProveedorType>()
            .AddTypeExtension<FunkoResolvers>()
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .ModifyRequestOptions(options =>
                {
                    options.IncludeExceptionDetails = environment.IsDevelopment();
                }
            );
       
    }

    public static void MapGraphQLEndpoints(this WebApplication app)
    {
        app.MapGraphQL("/graphql");
        app.MapGraphQL();
    }
    */
}
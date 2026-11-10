using FunkoAppi01.Dto;
using FunkoAppi01.Service;

namespace FunkoAppi01.GraphQL;

public class Query
{
    public async Task<IEnumerable<FunkoDto>> GetFunkoAsy(
        [Service] IServiceFunko service)
    {
        return await service.GetAllAsync();
    }

    public async Task<FunkoDto> GetFunkoById(long id, [Service] IServiceFunko service)
    {
        var resultado = await service.GetByIdAsync(id);

        if (resultado.IsFailure)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage(resultado.Error.Message)
                    .SetCode("NOT_FOUND")
                    .Build()
            );
        }
        return resultado.Value;

        
    }
    
    public async Task<IEnumerable<CategoriaDto>> GetCategoriaAsync(
        [Service] IServiceCategoria service)
    {
        return await service.GetAllAsync();
    }
    
    [HotChocolate.Authorization.Authorize(Roles = new[] {"ADMIN"})]
    public async Task<IEnumerable<ProveedorDto>> GetFunkoAsync(
        [Service] IServiceProveedor service)

    {
        return await service.GetAllAsync();
    }
    
}
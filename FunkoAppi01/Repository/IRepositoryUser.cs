using FunkoAppi01.Entity;

namespace FunkoAppi01.Repository;

public interface IRepositoryUser
{
    Task<UserEntity?> FindByEmailAsync(string email);
    Task<UserEntity> CreateAsync(UserEntity user);
    Task UpdateAsync(UserEntity user);
}

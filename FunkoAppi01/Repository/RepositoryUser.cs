using FunkoAppi01.Entity;
using Microsoft.EntityFrameworkCore;

namespace FunkoAppi01.Repository;

/// <summary>
/// Repositorio de usuarios con Entity Framework Core.
/// </summary>
public class RepositoryUser(AppDbContext context) : IRepositoryUser
{
    public async Task<UserEntity?> FindByEmailAsync(string email) =>
        await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<UserEntity> CreateAsync(UserEntity user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(UserEntity user) => await context.SaveChangesAsync();
}

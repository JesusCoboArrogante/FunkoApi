using FunkoAppi01.Entity;
using Microsoft.EntityFrameworkCore;

namespace FunkoAppi01.Infraestructure;

/// <summary>
/// Crea los usuarios de prueba (admin y user) si la tabla está vacía.
/// Necesario porque el registro público siempre asigna el rol USER:
/// sin esto no habría forma de probar un endpoint protegido para ADMIN.
/// </summary>
public static class UserSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        context.Users.AddRange(
            new UserEntity
            {
                Username = "admin",
                Email = "admin@funko.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", 11),
                Role = UserRoles.Admin,
                CreatedAt = DateTime.UtcNow
            },
            new UserEntity
            {
                Username = "user",
                Email = "user@funko.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123", 11),
                Role = UserRoles.User,
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;

namespace FunkoAppi01.Entity;

/// <summary>
/// Contexto de Entity Framework Core para la base de datos local de Funkos.
/// El proveedor (SQLite o PostgreSQL) se decide al registrarlo, según el perfil.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<FunkoEntity> Funkos => Set<FunkoEntity>();
    public DbSet<CategoriaEntity> Categorias => Set<CategoriaEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoriaEntity>(entity =>
        {
            entity.ToTable("categorias");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        modelBuilder.Entity<FunkoEntity>(entity =>
        {
            entity.ToTable("funkos");

            entity.HasKey(f => f.Id);

            entity.Property(f => f.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            entity.Property(f => f.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)");
            entity.Property(f => f.Stock).HasColumnName("stock");
            entity.Property(f => f.IsDeleted).HasColumnName("is_deleted");
            entity.Property(f => f.CreatedAt).HasColumnName("created_at");
            entity.Property(f => f.UpdatedAt).HasColumnName("updated_at");

            // "FALSE" en vez de "0": SQLite lo acepta desde la 3.23 y es la unica
            // forma de comparar con una columna boolean en PostgreSQL (que no admite
            // comparar boolean con integer).
            entity.HasIndex(f => f.Nombre).IsUnique().HasFilter("\"is_deleted\" = FALSE");

            entity.HasOne(f => f.Categoria)
                .WithMany(c => c.Funkos)
                .HasForeignKey(f => f.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Los Funkos borrados siguen en la tabla (borrado lógico); por defecto se ocultan.
            entity.HasQueryFilter(f => !f.IsDeleted);
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(u => u.Role).HasColumnName("role").HasMaxLength(50).IsRequired();
            entity.Property(u => u.IsDeleted).HasColumnName("is_deleted");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            entity.Property(u => u.LastLoginAt).HasColumnName("last_login_at");

            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}

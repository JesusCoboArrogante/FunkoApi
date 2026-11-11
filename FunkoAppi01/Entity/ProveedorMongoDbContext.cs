
using FunkoAppi01.IdGenerator;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FunkoAppi01.Entity;

public class ProveedorMongoDbContext(DbContextOptions<ProveedorMongoDbContext> options) : DbContext(options)
{
    
    public DbSet<ProveedorEntity> Proveedores => Set<ProveedorEntity>();
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProveedorEntity>(entity =>
            {
                entity.ToCollection("proveedores");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                    .HasValueGenerator<YoutubeIdValueGenerator>()
                    .ValueGeneratedOnAdd();
            }
        );


    }
}
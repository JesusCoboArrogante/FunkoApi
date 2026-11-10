using FunkoAppi01.Models;
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
        modelBuilder.Entity<Proveedor>().ToCollection("proveedores");
        
    }
}
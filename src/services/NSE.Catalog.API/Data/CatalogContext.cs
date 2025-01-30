
using Microsoft.EntityFrameworkCore;

using NSE.Catalog.API.Models;
using NSE.Core.Data;

namespace NSE.Catalog.API.Data;

public class CatalogContext(DbContextOptions<CatalogContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
        {
            property.SetColumnType("varchar(100)");
        }


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<bool> Commit() => await base.SaveChangesAsync() > 0;
}

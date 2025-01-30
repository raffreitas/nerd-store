using Microsoft.EntityFrameworkCore;

using NSE.Catalog.API.Models;
using NSE.Core.Data;

namespace NSE.Catalog.API.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogContext _context;

    public ProductRepository(CatalogContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<IEnumerable<Product>> GetAll()
        => await _context.Products.AsNoTracking().ToListAsync();

    public async Task<Product?> GetById(Guid id)
         => await _context.Products.FindAsync(id);

    public void Add(Product product)
        => _context.Add(product);

    public void Update(Product product)
        => _context.Update(product);

    public void Dispose()
        => _context.Dispose();
}

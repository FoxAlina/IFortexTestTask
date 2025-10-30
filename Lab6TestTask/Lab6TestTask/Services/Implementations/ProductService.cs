using Lab6TestTask.Data;
using Lab6TestTask.Models;
using Lab6TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab6TestTask.Services.Implementations;

/// <summary>
/// ProductService.
/// Implement methods here.
/// </summary>
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;

    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetProductAsync()
    {
        return await _dbContext.Products.OrderByDescending(product => product.Price).FirstOrDefaultAsync(product => product.Status == Enums.ProductStatus.Reserved);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        DateTime targetDate = new DateTime(2025, 1, 1);
        return await _dbContext.Products.Where(product => product.Quantity >= 1000 && product.ReceivedDate >= targetDate).ToListAsync();
    }
}

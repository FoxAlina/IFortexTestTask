using Lab6TestTask.Data;
using Lab6TestTask.Models;
using Lab6TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab6TestTask.Services.Implementations;

/// <summary>
/// WarehouseService.
/// Implement methods here.
/// </summary>
public class WarehouseService : IWarehouseService
{
    private readonly ApplicationDbContext _dbContext;

    public WarehouseService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Warehouse?> GetWarehouseAsync()
    {
        return await _dbContext.Warehouses.GroupJoin(
            _dbContext.Products, 
            warehouse => warehouse.WarehouseId, 
            product => product.WarehouseId, 
            (selectedWarehouse, selectedProducts) => 
            new 
            {
                selectedWarehouse,
                TotalQuantity = selectedProducts
                    .Where(product => product.Status == Enums.ProductStatus.ReadyForDistribution)
                    .Sum(product => product.Quantity)
            })
        .OrderByDescending(groupedWarehouse => groupedWarehouse.TotalQuantity)
        .Select(sortResult => sortResult.selectedWarehouse)
        .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Warehouse>> GetWarehousesAsync()
    {
        DateTime quaterStartDate = new DateTime(2025, 4, 1);
        DateTime quaterEndDate = new DateTime(2025, 6, 30);

        return await _dbContext.Warehouses.Join(
                _dbContext.Products, 
                warehouse => warehouse.WarehouseId, 
                product => product.WarehouseId, 
                (selectedWarehouse, selectedProduct) => new { selectedWarehouse, selectedProduct })
            .Where(joinedData => joinedData.selectedProduct.ReceivedDate >= quaterStartDate && joinedData.selectedProduct.ReceivedDate <= quaterEndDate)
            .Select(joinedData => joinedData.selectedWarehouse).ToListAsync();
    }
}

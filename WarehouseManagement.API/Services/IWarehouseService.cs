using WarehouseManagement.API.Models.RequestDto;

namespace WarehouseManagement.API.Services
{
    public interface IWarehouseService
    {
        Task<bool> CheckStockAsync(int warehouseId, int productId, int requiredQty);
        Task<bool> DeductStockAsync(int warehouseId, int productId, int qty);
        Task<bool> AddStockAsync(int warehouseId, int productId, int qty);
        Task<string> TransferStockAsync(TransferDto transferDto);

        Task<IEnumerable<InventoryDto>>? GetInventoryByWarehouse(int warehouseId);
    }
}

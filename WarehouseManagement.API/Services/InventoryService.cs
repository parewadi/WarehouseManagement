using WarehouseManagement.API.Models.UnitOfWork;

namespace WarehouseManagement.API.Services
{
    public class InventoryService:IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<object>> SearchInventoryAsync(string? productName, string? warehouseName, string? pincode, string? code)
        {
            var inventories = await _unitOfWork.Inventories.GetWithIncludesAsync(
                includeProperties: "Product,Warehouse");

            var query = inventories.AsQueryable();
            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(i => i.Product.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase));
            }

            if(!string.IsNullOrEmpty(warehouseName))
            {
                query = query.Where(i => i.Warehouse.WarehouseName.Contains(warehouseName, StringComparison.OrdinalIgnoreCase));
            }
            if(!string.IsNullOrEmpty(pincode))
            {
                query = query.Where(i => i.Warehouse.Pincode.Contains(pincode, StringComparison.OrdinalIgnoreCase));
            }   

            if(!string.IsNullOrEmpty(code))
            {
                query = query.Where(i => i.Warehouse.Code.Contains(code, StringComparison.OrdinalIgnoreCase));
            }
            return query.Select(i => new
            {
                i.InventoryId,
                ProductName = i.Product.ProductName,
                WarehouseName = i.Warehouse.WarehouseName,
                WarehouseCode = i.Warehouse.Code,
                i.Quantity,
                i.Warehouse.Pincode,
                i.Warehouse.Location
            }).ToList();



        }
    }
}

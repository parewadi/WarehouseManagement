using Microsoft.EntityFrameworkCore;
using WarehouseManagement.API.Data;
using WarehouseManagement.API.Models.ReportDto;

namespace WarehouseManagement.API.Services
{
    public class ReportService:IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync(int? warehouseId = null)
        {
            var query = _context.TblInventory
                .Include(i => i.Warehouse)
                .Include(i => i.Product)
                .AsQueryable();

            if (warehouseId.HasValue)
                query = query.Where(i => i.WarehouseId == warehouseId.Value);

            var result = await query
                .GroupBy(i => new { i.WarehouseId, i.Warehouse.WarehouseName })
                .Select(g => new InventoryReportDto
                {
                    WarehouseId = g.Key.WarehouseId,
                    WarehouseName = g.Key.WarehouseName,
                    Items = g.Select(x => new InventoryItemDto
                    {
                        ProductId = x.ProductId,
                        ProductName = x.Product.ProductName,
                        Quantity = x.Quantity
                    }).ToList()
                }).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<OrderReportDto>> GetOrderReportAsync(int? salesPersonId = null, int? warehouseId = null)
        {
            var query = _context.TblOrder
                .Include(o => o.Items).ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (salesPersonId.HasValue)
                query = query.Where(o => o.SalesPersonId == salesPersonId.Value);

            if (warehouseId.HasValue)
                query = query.Where(o => o.AssignedWarehouseId == warehouseId.Value);

            var result = await query.Select(o => new OrderReportDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName,
                AssignedWarehouseName = o.AssignedWarehouse.WarehouseName,
                SalesPersonName = o.SalesPerson.UserName,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,
               
            }).ToListAsync();

            return result;
        }

         







    }
}

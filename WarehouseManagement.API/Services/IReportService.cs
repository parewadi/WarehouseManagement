using WarehouseManagement.API.Models.ReportDto;

namespace WarehouseManagement.API.Services
{
    public interface IReportService
    {
        Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync(int? warehouseId = null);
        Task<IEnumerable<OrderReportDto>> GetOrderReportAsync(int? salesPersonId = null, int? warehouseId = null);
    }
}

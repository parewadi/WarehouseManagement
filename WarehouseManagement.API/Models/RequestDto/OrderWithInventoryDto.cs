using WarehouseManagement.API.Models.ResponseDto;

namespace WarehouseManagement.API.Models.RequestDto
{
    public class OrderWithInventoryDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string WarehouseName { get; set; } = string.Empty;
        public string SalesPerson { get; set; } = string.Empty;

        // Reuse existing OrderItemDto (no modification needed)
        public List<OrderItemDto> Items { get; set; } = new();

        // Extra map for product-level stock info (does not touch OrderItemDto)
        public Dictionary<int, List<WarehouseStockDto>> ProductStockMap { get; set; } = new();
    }
}

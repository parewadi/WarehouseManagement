namespace WarehouseManagement.API.Models.RequestDto
{
    public class WarehouseOrderViewDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string WarehouseName { get; set; } = string.Empty;
        public string SalesPerson { get; set; } = string.Empty;

        public List<WarehouseOrderItemViewDto> Items { get; set; } = new();
    }
}

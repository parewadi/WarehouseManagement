namespace WarehouseManagement.API.Models.RequestDto
{
    public class WarehouseOrderItemViewDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantityRequested { get; set; }

        public List<WarehouseStockDto> WarehousesWithStock { get; set; } = new();
    }
}

namespace WarehouseManagement.API.Models.RequestDto
{
    public class WarehouseStockDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int AvailableQty { get; set; }
    }
}

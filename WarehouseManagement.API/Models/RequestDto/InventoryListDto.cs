namespace WarehouseManagement.API.Models.RequestDto
{
    public class InventoryListDto
    {
        public int InventoryId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}

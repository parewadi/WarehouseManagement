namespace WarehouseManagement.API.Models.ReportDto
{
    public class InventoryReportDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public List<InventoryItemDto> Items { get; set; }
    }
}
